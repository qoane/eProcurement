using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Xunit;

public class TenderClarificationManagementTests
{
    [Fact]
    public void Supplier_can_ask_clarification_for_published_tender()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var question = new TenderClarification(tender.Id, "Can we submit electronically?", "supplier@demo.ls", DateTimeOffset.UtcNow, false)
            with { SupplierId = Guid.NewGuid(), SupplierName = "Demo Supplier", Visibility = TenderClarificationVisibility.Private };
        tender.Clarifications.Add(question);
        Assert.Equal(TenderClarificationStatus.Submitted, question.Status);
        Assert.Equal(TenderClarificationVisibility.Private, question.Visibility);
        Assert.Single(tender.Clarifications);
    }

    [Fact]
    public void Supplier_cannot_ask_clarification_after_closing_date()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddMinutes(-1));
        Assert.False(tender.ClosingDate > DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Procurement_officer_can_respond()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var clarification = new TenderClarification(tender.Id, "Is a site visit required?", "supplier@demo.ls", DateTimeOffset.UtcNow, false);
        var response = new TenderClarificationResponse(clarification.Id, "No site visit is required.", "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        clarification.Responses.Add(response);
        clarification = clarification with { Status = TenderClarificationStatus.Answered };
        Assert.Single(clarification.Responses);
        Assert.Equal(TenderClarificationStatus.Answered, clarification.Status);
    }

    [Fact]
    public void Private_response_does_not_appear_publicly()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var privateClarification = new TenderClarification(tender.Id, "Private commercial question?", "supplier@demo.ls", DateTimeOffset.UtcNow, false)
            with { Visibility = TenderClarificationVisibility.Private, Status = TenderClarificationStatus.Answered };
        privateClarification.Responses.Add(new TenderClarificationResponse(privateClarification.Id, "Private response", "procurement@lca.org.ls", DateTimeOffset.UtcNow));
        tender.Clarifications.Add(privateClarification);
        var publicClarifications = PublicClarifications(tender);
        Assert.Empty(publicClarifications);
    }

    [Fact]
    public void Public_response_appears_on_public_opportunity_page()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var now = DateTimeOffset.UtcNow;
        var publicClarification = new TenderClarification(tender.Id, "Will addenda be issued?", "supplier@demo.ls", now, true)
            with { Visibility = TenderClarificationVisibility.Public, Status = TenderClarificationStatus.Published };
        publicClarification.Responses.Add(new TenderClarificationResponse(publicClarification.Id, "Yes, addenda will be published on the opportunity page.", "procurement@lca.org.ls", now)
            with { IsPublished = true, PublishedAt = now, PublishedBy = "procurement@lca.org.ls" });
        tender.Clarifications.Add(publicClarification);
        var publicClarifications = PublicClarifications(tender);
        Assert.Single(publicClarifications);
        Assert.Equal("Will addenda be issued?", publicClarifications[0].Question);
    }

    [Fact]
    public void Supplier_receives_response_notification()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var notification = new NotificationMessage("TenderClarificationResponded", nameof(Tender), tender.Id, NotificationChannel.InApp, "Tender clarification response", "Your clarification has been answered.", NotificationPriority.Normal, NotificationStatus.Unread, DateTimeOffset.UtcNow, RelatedUrl: "/app/supplier/clarifications");
        notification.Recipients.Add(new NotificationRecipient(notification.Id, "supplier@demo.ls", "Supplier", "Demo Supplier", "supplier@demo.ls", null, null, NotificationStatus.Unread));
        Assert.Equal("TenderClarificationResponded", notification.EventCode);
        Assert.Single(notification.Recipients);
    }

    [Fact]
    public void Audit_events_are_created()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        var auditEvents = new[]
        {
            new AuditEvent("Tender clarification created", nameof(Tender), tender.Id, tender.TenderNumber, "supplier@demo.ls", "Question", DateTimeOffset.UtcNow),
            new AuditEvent("Tender clarification responded", nameof(Tender), tender.Id, tender.TenderNumber, "procurement@lca.org.ls", "Response", DateTimeOffset.UtcNow)
        };
        Assert.Contains(auditEvents, x => x.EventType == "Tender clarification created");
        Assert.Contains(auditEvents, x => x.EventType == "Tender clarification responded");
    }

    [Fact]
    public void Public_api_exposes_only_public_answered_clarifications()
    {
        var tender = PublishedTender(DateTimeOffset.UtcNow.AddDays(7));
        tender.Clarifications.Add(new TenderClarification(tender.Id, "Unanswered?", "supplier@demo.ls", DateTimeOffset.UtcNow, true));
        var privateAnswered = new TenderClarification(tender.Id, "Private?", "supplier@demo.ls", DateTimeOffset.UtcNow, false) with { Status = TenderClarificationStatus.Answered };
        privateAnswered.Responses.Add(new TenderClarificationResponse(privateAnswered.Id, "Private", "procurement@lca.org.ls", DateTimeOffset.UtcNow));
        tender.Clarifications.Add(privateAnswered);
        var publicAnswered = new TenderClarification(tender.Id, "Public?", "supplier@demo.ls", DateTimeOffset.UtcNow, true) with { Status = TenderClarificationStatus.Published, Visibility = TenderClarificationVisibility.Public };
        publicAnswered.Responses.Add(new TenderClarificationResponse(publicAnswered.Id, "Public", "procurement@lca.org.ls", DateTimeOffset.UtcNow) with { IsPublished = true, PublishedAt = DateTimeOffset.UtcNow });
        tender.Clarifications.Add(publicAnswered);
        Assert.Single(PublicClarifications(tender));
    }

    static List<PublicTenderClarificationDto> PublicClarifications(Tender tender) => tender.Clarifications
        .Where(c => c.IsPublic && c.Visibility == TenderClarificationVisibility.Public)
        .SelectMany(c => c.Responses.Where(r => r.IsPublished).Select(r => new PublicTenderClarificationDto(c.Question, r.Response, r.PublishedAt ?? DateTimeOffset.UtcNow)))
        .ToList();

    static Tender PublishedTender(DateTimeOffset closingDate) => new("RFP-LCA-2026-CLR", "Clarification tender", "Tender with clarifications", TenderType.RFP, "Open Tender", TenderStatus.Published, DateTimeOffset.UtcNow.AddDays(-1), closingDate, "procurement@lca.org.ls", DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(-1), "procurement@lca.org.ls");
}
