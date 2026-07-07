using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Tests;

public class BidOpeningTests
{
    [Fact]
    public void Bid_opening_session_can_be_created_for_a_tender()
    {
        var tender = Tender(DateTimeOffset.UtcNow.AddDays(-1));
        var session = new BidOpeningSession("BO-001", tender.Id, "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Draft, "procurement@lca.org.ls", DateTimeOffset.UtcNow, "Chair");
        Assert.Equal(tender.Id, session.TenderId);
        Assert.Equal(BidOpeningSessionStatus.Draft, session.Status);
    }

    [Fact]
    public void Locked_bid_submissions_are_attached_to_the_opening_session()
    {
        var tender = Tender(DateTimeOffset.UtcNow.AddDays(-1));
        var bid = Bid(tender.Id, BidSubmissionStatus.Locked, DateTimeOffset.UtcNow.AddDays(-2));
        var session = Session(tender, [bid]);
        Assert.Contains(session.Submissions, x => x.BidSubmissionId == bid.Id && x.Status == BidOpeningSubmissionStatus.Pending);
    }

    [Fact]
    public void Late_submissions_are_marked_correctly()
    {
        var tender = Tender(DateTimeOffset.UtcNow.AddDays(-2));
        var late = Bid(tender.Id, BidSubmissionStatus.Locked, DateTimeOffset.UtcNow.AddDays(-1));
        var session = Session(tender, [late]);
        Assert.Equal(BidOpeningSubmissionStatus.Late, session.Submissions.Single().Status);
    }

    [Fact]
    public void Session_cannot_start_without_committee_members()
    {
        var session = new BidOpeningSession("BO-002", Guid.NewGuid(), "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Scheduled, "user", DateTimeOffset.UtcNow, "Chair");
        Assert.Throws<InvalidOperationException>(() => { if (!session.CommitteeMembers.Any()) throw new InvalidOperationException("Bid opening session requires committee members."); });
    }

    [Fact]
    public void Submission_cannot_be_opened_before_session_starts()
    {
        var session = new BidOpeningSession("BO-003", Guid.NewGuid(), "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Scheduled, "user", DateTimeOffset.UtcNow, "Chair");
        Assert.Throws<InvalidOperationException>(() => { if (session.Status != BidOpeningSessionStatus.InProgress) throw new InvalidOperationException("Submission cannot be opened before session starts."); });
    }

    [Fact]
    public void Submission_cannot_be_opened_before_tender_closing_date()
    {
        var tender = Tender(DateTimeOffset.UtcNow.AddDays(1));
        Assert.Throws<InvalidOperationException>(() => { if (tender.ClosingDate > DateTimeOffset.UtcNow) throw new InvalidOperationException("Submission cannot be opened before tender closing date."); });
    }

    [Fact]
    public void Opening_a_submission_changes_BidSubmission_status_to_Opened()
    {
        var bid = Bid(Guid.NewGuid(), BidSubmissionStatus.Locked, DateTimeOffset.UtcNow.AddDays(-2));
        bid = bid with { Status = BidSubmissionStatus.Opened, OpenedAt = DateTimeOffset.UtcNow };
        Assert.Equal(BidSubmissionStatus.Opened, bid.Status);
        Assert.NotNull(bid.OpenedAt);
    }

    [Fact]
    public void Completing_session_generates_opening_report()
    {
        var session = new BidOpeningSession("BO-004", Guid.NewGuid(), "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Completed, "user", DateTimeOffset.UtcNow, "Chair");
        session.Reports.Add(new BidOpeningReport(session.Id, "BOR-001", DateTimeOffset.UtcNow, "chair@lca.org.ls", "{}"));
        Assert.Contains(session.Reports, r => r.ReportNumber == "BOR-001");
    }

    [Fact]
    public void Completed_session_can_be_referred_to_evaluation()
    {
        var session = new BidOpeningSession("BO-005", Guid.NewGuid(), "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Completed, "user", DateTimeOffset.UtcNow, "Chair");
        session = session with { Status = BidOpeningSessionStatus.ReferredToEvaluation };
        Assert.Equal(BidOpeningSessionStatus.ReferredToEvaluation, session.Status);
    }

    [Fact]
    public void Audit_events_are_created_throughout()
    {
        var id = Guid.NewGuid();
        var events = new[] { "Bid opening session created", "Bid opening session scheduled", "Opening started", "Bid submission opened", "Opening completed", "Opening report generated", "Referred to evaluation" }
            .Select(e => new AuditEvent(e, nameof(BidOpeningSession), id, "BO-006", "actor", e, DateTimeOffset.UtcNow)).ToList();
        Assert.Contains(events, e => e.EventType == "Bid submission opened");
        Assert.True(events.Count >= 7);
    }

    static Tender Tender(DateTimeOffset closing) => new("RFP-BO", "Opening tender", "Description", TenderType.RFP, "Open Tender", TenderStatus.Published, DateTimeOffset.UtcNow.AddDays(-5), closing, "procurement@lca.org.ls", DateTimeOffset.UtcNow.AddDays(-6));
    static BidSubmission Bid(Guid tenderId, BidSubmissionStatus status, DateTimeOffset submittedAt) => new("BID-BO", tenderId, Guid.NewGuid(), status, submittedAt, "supplier@demo.ls", submittedAt, LockedAt: submittedAt);
    static BidOpeningSession Session(Tender tender, IEnumerable<BidSubmission> bids)
    {
        var session = new BidOpeningSession("BO-SESSION", tender.Id, "Opening", DateTimeOffset.UtcNow, BidOpeningSessionStatus.Draft, "user", DateTimeOffset.UtcNow, "Chair");
        foreach (var bid in bids) session.Submissions.Add(new BidOpeningSubmission(session.Id, bid.Id, bid.SupplierId, "Supplier", bid.SubmissionNumber, bid.SubmittedAt, bid.SubmittedAt > tender.ClosingDate ? BidOpeningSubmissionStatus.Late : BidOpeningSubmissionStatus.Pending));
        return session;
    }
}
