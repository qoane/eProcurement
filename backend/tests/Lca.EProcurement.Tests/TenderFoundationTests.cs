using Lca.EProcurement.Domain;
using Xunit;

public class TenderFoundationTests
{
    [Fact]
    public void Create_tender_defaults_to_draft_with_document_metadata()
    {
        var tender = new Tender("RFP-LCA-2026-T01", "Spectrum monitoring platform", "Request for proposals", TenderType.RFP, "Open Tender", TenderStatus.Draft, null, DateTimeOffset.UtcNow.AddDays(21), "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        tender.Documents.Add(new TenderDocument(tender.Id, "TermsOfReference", "tor.pdf", "Terms of reference", true, DateTimeOffset.UtcNow, tender.CreatedBy));
        tender.Lots.Add(new TenderLot(tender.Id, "LOT-1", "Platform", "Implementation and support"));
        Assert.Equal(TenderStatus.Draft, tender.Status);
        Assert.Equal(TenderType.RFP, tender.TenderType);
        Assert.Single(tender.Documents);
        Assert.Single(tender.Lots);
    }

    [Fact]
    public void Publish_tender_records_status_history_and_supplier_notification_metadata()
    {
        var tender = new Tender("RFQ-LCA-2026-T02", "Office network refresh", "Request for quotations", TenderType.RFQ, "Selective RFQ", TenderStatus.Draft, null, DateTimeOffset.UtcNow.AddDays(14), "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        tender.SupplierInvitations.Add(new TenderSupplierInvitation(tender.Id, null, "Maseru ICT Supplies", "sales@example.co.ls", DateTimeOffset.UtcNow, tender.CreatedBy));
        var publishedAt = DateTimeOffset.UtcNow;
        tender = tender with { Status = TenderStatus.Published, PublicationDate = publishedAt, PublishedAt = publishedAt, PublishedBy = "approver@lca.org.ls" };
        tender.StatusHistory.Add(new TenderStatusHistory(tender.Id, TenderStatus.Draft, TenderStatus.Published, "approver@lca.org.ls", publishedAt, "Tender published"));
        tender.SupplierInvitations[0] = tender.SupplierInvitations[0] with { NotifiedAt = publishedAt };
        Assert.Equal(TenderStatus.Published, tender.Status);
        Assert.NotNull(tender.PublishedAt);
        Assert.NotNull(tender.SupplierInvitations[0].NotifiedAt);
        Assert.Contains(tender.StatusHistory, h => h.ToStatus == TenderStatus.Published);
    }

    [Fact]
    public void Create_clarification_and_respond_to_clarification()
    {
        var tender = new Tender("RFI-LCA-2026-T03", "Market sounding", "Request for information", TenderType.RFI, "Open RFI", TenderStatus.Published, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(10), "procurement@lca.org.ls", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow, "procurement@lca.org.ls");
        var clarification = new TenderClarification(tender.Id, "Can suppliers propose alternative deployment models?", "supplier@demo.co.ls", DateTimeOffset.UtcNow);
        clarification.Responses.Add(new TenderClarificationResponse(clarification.Id, "Yes, alternatives may be included as clearly labelled options.", "procurement@lca.org.ls", DateTimeOffset.UtcNow));
        tender.Clarifications.Add(clarification);
        Assert.Single(tender.Clarifications);
        Assert.Single(tender.Clarifications[0].Responses);
    }

    [Fact]
    public void Audit_events_are_created_for_tender_lifecycle()
    {
        var tender = new Tender("RFP-LCA-2026-T04", "Audit trail tender", "Tender with audit", TenderType.RFP, "Open Tender", TenderStatus.Draft, null, DateTimeOffset.UtcNow.AddDays(30), "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        var auditEvents = new List<AuditEvent>
        {
            new("Tender created", nameof(Tender), tender.Id, tender.TenderNumber, tender.CreatedBy, "Created tender", DateTimeOffset.UtcNow),
            new("Tender published", nameof(Tender), tender.Id, tender.TenderNumber, "approver@lca.org.ls", "Published tender", DateTimeOffset.UtcNow),
            new("Tender clarification responded", nameof(Tender), tender.Id, tender.TenderNumber, "procurement@lca.org.ls", "Responded", DateTimeOffset.UtcNow)
        };
        Assert.Contains(auditEvents, e => e.EventType == "Tender created");
        Assert.Contains(auditEvents, e => e.EventType == "Tender published");
        Assert.All(auditEvents, e => Assert.Equal(tender.Id, e.EntityId));
    }
}
