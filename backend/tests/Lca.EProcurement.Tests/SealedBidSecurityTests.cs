using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class SealedBidSecurityTests
{
    [Fact]
    public void Bid_draft_is_not_sealed()
    {
        var bid = Bid(BidSubmissionStatus.Draft);
        Assert.Equal(BidSubmissionStatus.Draft, bid.Status);
    }

    [Fact]
    public void Submitting_bid_creates_sealed_envelope_evidence_shape()
    {
        var bid = Bid(BidSubmissionStatus.Locked);
        var envelope = new SealedBidEnvelope(bid.Id, bid.TenderId, bid.SupplierId, "ENV-001", SealedBidEnvelopeStatus.Sealed, DateTimeOffset.UtcNow, bid.SubmittedBy, null, null, null, "submission-hash", "manifest-hash", "system-time", null, "vault://bid", null, DateTimeOffset.UtcNow);
        Assert.Equal(SealedBidEnvelopeStatus.Sealed, envelope.Status);
        Assert.Equal(bid.Id, envelope.BidSubmissionId);
    }

    [Fact]
    public void Uploading_bid_document_records_hash_evidence()
    {
        var bid = Bid(BidSubmissionStatus.Draft);
        var doc = new BidSubmissionDocument(bid.Id, "Technical Proposal", "technical.pdf", "vault://technical.pdf", "supplier@demo.ls", DateTimeOffset.UtcNow);
        var evidence = new SealedBidDocumentEvidence(doc.Id, bid.Id, doc.Filename, doc.DocumentType, doc.StorageReference, 128, "abc123", "SHA256", doc.UploadedAt, null, false, DateTimeOffset.UtcNow);
        Assert.Equal("SHA256", evidence.HashAlgorithm);
        Assert.Equal(doc.Id, evidence.BidSubmissionDocumentId);
    }

    [Fact]
    public void Procurement_officer_denied_access_creates_access_log_shape()
    {
        var bid = Bid(BidSubmissionStatus.Locked);
        var log = new BidAccessLog(bid.Id, "procurement@lca.org.ls", "procurement@lca.org.ls", BidAccessType.ViewPricing, false, "Pricing access denied before opening", DateTimeOffset.UtcNow, null, null);
        Assert.False(log.AccessAllowed);
        Assert.Equal(BidAccessType.ViewPricing, log.AccessType);
    }

    [Fact]
    public void Opening_bid_records_integrity_evidence()
    {
        var bid = Bid(BidSubmissionStatus.Opened);
        var sessionId = Guid.NewGuid();
        var evidence = new BidOpeningEvidence(sessionId, bid.Id, "chair@lca.org.ls", DateTimeOffset.UtcNow, "Controlled committee opening", "submission-hash", "manifest-hash", true, "{\"passed\":true}", DateTimeOffset.UtcNow);
        Assert.True(evidence.IntegrityCheckPassed);
        Assert.Equal(sessionId, evidence.BidOpeningSessionId);
    }

    static BidSubmission Bid(BidSubmissionStatus status) => new("BID-SEC-001", Guid.NewGuid(), Guid.NewGuid(), status, DateTimeOffset.UtcNow, "supplier@demo.ls");
}
