using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class DocumentManagementTests
{
    [Fact]
    public void Document_record_links_to_any_procurement_entity()
    {
        var entityId = Guid.NewGuid();
        var document = new DocumentRecord("DOC-001", "Supplier", entityId, "TaxClearance", "Tax Clearance", null, DocumentClassification.Internal, DocumentStatus.Active, null, false, false, "supplier@demo.ls", DateTimeOffset.UtcNow);
        Assert.Equal("Supplier", document.EntityType);
        Assert.Equal(entityId, document.EntityId);
    }

    [Fact]
    public void Versioning_shape_marks_current_version_and_hash_evidence()
    {
        var documentId = Guid.NewGuid();
        var version = new DocumentVersion(documentId, 1, "2026/07/14/file.pdf", "tax.pdf", "application/pdf", ".pdf", 1200, "LocalFileSystem", "2026/07/14/file.pdf", "abc123", "SHA256", "supplier@demo.ls", DateTimeOffset.UtcNow, true);
        Assert.True(version.IsCurrent);
        Assert.Equal("SHA256", version.HashAlgorithm);
    }

    [Fact]
    public void Public_download_requires_public_classification_and_active_status()
    {
        var document = new DocumentRecord("DOC-002", "Tender", Guid.NewGuid(), "RFP", "Public RFP", null, DocumentClassification.Public, DocumentStatus.Active, null, true, false, "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        Assert.True(document.IsPublic);
        Assert.Equal(DocumentClassification.Public, document.Classification);
        Assert.Equal(DocumentStatus.Active, document.Status);
    }

    [Fact]
    public void Sealed_bid_document_classification_is_available_for_access_control()
    {
        var document = new DocumentRecord("DOC-003", "BidSubmission", Guid.NewGuid(), "TechnicalProposal", "Technical Proposal", null, DocumentClassification.SealedBid, DocumentStatus.Active, null, false, true, "supplier@demo.ls", DateTimeOffset.UtcNow);
        Assert.True(document.IsConfidential);
        Assert.Equal(DocumentClassification.SealedBid, document.Classification);
    }

    [Fact]
    public void Virus_scan_placeholder_can_record_skipped_development_result()
    {
        var result = new DocumentVirusScanResult(Guid.NewGuid(), "NoOpDevelopmentScanner", VirusScanStatus.Skipped, DateTimeOffset.UtcNow, "Skipped", null, "Development placeholder");
        Assert.Equal(VirusScanStatus.Skipped, result.ScanStatus);
    }
}
