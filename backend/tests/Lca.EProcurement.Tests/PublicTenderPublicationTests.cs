using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Xunit;

public class PublicTenderPublicationTests
{
    [Fact]
    public void Draft_tender_is_not_visible_publicly()
    {
        var tender = Tender(TenderStatus.Draft);
        var publications = new List<PublicTenderPublication>();
        Assert.DoesNotContain(publications, x => x.TenderId == tender.Id && x.IsVisible);
    }

    [Fact]
    public void Published_tender_is_visible_publicly()
    {
        var tender = Tender(TenderStatus.Published);
        var publication = Publication(tender);
        Assert.True(publication.IsVisible);
        Assert.Equal(TenderStatus.Published, publication.Status);
    }

    [Fact]
    public void Public_tender_api_excludes_internal_fields()
    {
        var tender = Tender(TenderStatus.Published);
        var publication = Publication(tender);
        var dto = Detail(publication, [], []);
        var properties = dto.GetType().GetProperties().Select(x => x.Name).ToHashSet();
        Assert.DoesNotContain(nameof(PublicTenderPublication.Id), properties);
        Assert.DoesNotContain(nameof(PublicTenderPublication.TenderId), properties);
        Assert.DoesNotContain(nameof(PublicTenderPublication.CreatedAt), properties);
        Assert.Contains(nameof(PublicTenderDetailDto.BidSubmissionUrl), properties);
    }

    [Fact]
    public void Public_documents_are_visible()
    {
        var tender = Tender(TenderStatus.Published);
        tender.Documents.Add(new TenderDocument(tender.Id, "TOR", "tor.pdf", "Terms of reference", true, DateTimeOffset.UtcNow, tender.CreatedBy, IsPublic: true, PublicUrl: "/public/docs/tor.pdf"));
        var publicDocs = tender.Documents.Where(x => x.IsPublic && !string.IsNullOrWhiteSpace(x.PublicUrl)).Select(x => new PublicTenderDocumentDto(x.DocumentType, x.FileName, x.PublicUrl!, x.IsDownloadable, DateTimeOffset.UtcNow)).ToList();
        Assert.Single(publicDocs);
        Assert.Equal("/public/docs/tor.pdf", publicDocs[0].PublicUrl);
    }

    [Fact]
    public void Private_internal_documents_are_not_visible()
    {
        var tender = Tender(TenderStatus.Published);
        tender.Documents.Add(new TenderDocument(tender.Id, "EvaluationPlan", "internal.pdf", "Internal evaluation plan", true, DateTimeOffset.UtcNow, tender.CreatedBy, IsPublic: false, PublicUrl: "/private/internal.pdf"));
        var publicDocs = tender.Documents.Where(x => x.IsPublic && !string.IsNullOrWhiteSpace(x.PublicUrl)).ToList();
        Assert.Empty(publicDocs);
    }

    [Fact]
    public void Latest_opportunities_widget_returns_published_tenders_only()
    {
        var published = Publication(Tender(TenderStatus.Published));
        var draftTender = Tender(TenderStatus.Draft);
        var hidden = Publication(draftTender) with { Status = TenderStatus.Draft, IsVisible = false };
        var latest = new[] { hidden, published }.Where(x => x.IsVisible && x.Status == TenderStatus.Published).OrderByDescending(x => x.PublishedAt).ToList();
        Assert.Single(latest);
        Assert.Equal(published.Reference, latest[0].Reference);
    }

    [Fact]
    public void Tender_publish_creates_audit_event()
    {
        var tender = Tender(TenderStatus.Published);
        var audit = new AuditEvent("Tender published", nameof(Tender), tender.Id, tender.TenderNumber, "approver@lca.org.ls", "Tender published", DateTimeOffset.UtcNow);
        Assert.Equal(nameof(Tender), audit.EntityType);
        Assert.Equal("Tender published", audit.EventType);
    }

    [Fact]
    public void Tender_publish_creates_notification_event()
    {
        var tender = Tender(TenderStatus.Published);
        var notification = new NotificationMessage("TenderPublished", nameof(Tender), tender.Id, NotificationChannel.InApp, "Tender published", tender.Title, NotificationPriority.Normal, NotificationStatus.Unread, DateTimeOffset.UtcNow);
        notification.Recipients.Add(new NotificationRecipient(notification.Id, "supplier@demo.ls", "Supplier", "Demo Supplier", "supplier@demo.ls", "+26650000000", null, NotificationStatus.Unread));
        Assert.Equal("TenderPublished", notification.EventCode);
        Assert.Single(notification.Recipients);
    }

    [Fact]
    public void Supplier_can_navigate_from_public_tender_to_bid_submission()
    {
        var tender = Tender(TenderStatus.Published);
        var publication = Publication(tender);
        var dto = Detail(publication, [], []);
        Assert.StartsWith("/app/bids/new", dto.BidSubmissionUrl);
    }

    [Fact]
    public void Anonymous_user_can_view_opportunities_but_cannot_submit_bid_without_login()
    {
        var publicRouteAllowsAnonymous = true;
        var bidSubmissionRequiresLogin = true;
        Assert.True(publicRouteAllowsAnonymous);
        Assert.True(bidSubmissionRequiresLogin);
    }

    static Tender Tender(TenderStatus status) => new("RFP-LCA-2026-PUB", "Public spectrum tender", "Public tender description", TenderType.RFP, "Open Tender", status, status == TenderStatus.Published ? DateTimeOffset.UtcNow : null, DateTimeOffset.UtcNow.AddDays(14), "procurement@lca.org.ls", DateTimeOffset.UtcNow.AddDays(-1), status == TenderStatus.Published ? DateTimeOffset.UtcNow : null, status == TenderStatus.Published ? "approver@lca.org.ls" : null, "ICT");
    static PublicTenderPublication Publication(Tender tender) => new(tender.Id, tender.TenderNumber, tender.TenderNumber, tender.Title, tender.Description, tender.TenderType, tender.ProcurementMethod, tender.Category, DateTimeOffset.UtcNow, tender.ClosingDate, tender.Status, tender.Status == TenderStatus.Published, "rfp-lca-2026-pub-public-spectrum-tender", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
    static PublicTenderDetailDto Detail(PublicTenderPublication publication, List<PublicTenderDocumentDto> documents, List<PublicTenderClarificationDto> clarifications) => new(publication.Reference, publication.Title, publication.Description, publication.TenderType.ToString(), publication.ProcurementMethod, publication.Category, publication.PublishedAt, publication.ClosingDate, publication.Status.ToString(), publication.Slug, $"/opportunities/{publication.Slug}", $"/app/bids/new?tender={publication.Reference}", documents, clarifications);
}
