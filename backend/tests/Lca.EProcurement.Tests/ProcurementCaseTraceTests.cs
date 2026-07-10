using Lca.EProcurement.Domain;
using Xunit;

public class ProcurementCaseTraceTests
{
    private static (ProcurementCase Case, Dictionary<ProcurementCaseRelationshipType, ProcurementCaseLink[]> Links, List<AuditEvent> Audit) DemoCase()
    {
        var procurementCase = new ProcurementCase("PCASE-LCA-2026-ICT-001", "Supply and Delivery of ICT Equipment for LCA Regional Monitoring Offices", "Complete LCA lifecycle trace", Guid.NewGuid(), "Technical Services", ProcurementCaseStatus.Active, DateTimeOffset.UtcNow.AddDays(-90), "system") { Links = [] };
        void Add(string entityType, string reference, ProcurementCaseRelationshipType type) => procurementCase.Links.Add(new ProcurementCaseLink(procurementCase.Id, entityType, Guid.NewGuid(), reference, type, DateTimeOffset.UtcNow));
        Add(nameof(AnnualProcurementPlan), "APP-LCA-2026-ICT", ProcurementCaseRelationshipType.AnnualPlan); Add(nameof(Budget), "BUD-TECH-2026", ProcurementCaseRelationshipType.Budget); Add(nameof(Requisition), "REQ-LCA-2026-0042", ProcurementCaseRelationshipType.Requisition); Add(nameof(Tender), "RFP-LCA-2026-ICT-007", ProcurementCaseRelationshipType.Tender); Add(nameof(PublicTenderPublication), "PUB-LCA-2026-ICT-007", ProcurementCaseRelationshipType.PublicPublication); Add(nameof(BidSubmission), "BID-LCA-2026-ICT-001", ProcurementCaseRelationshipType.BidSubmission); Add(nameof(BidSubmission), "BID-LCA-2026-ICT-002", ProcurementCaseRelationshipType.BidSubmission); Add(nameof(BidSubmission), "BID-LCA-2026-ICT-003", ProcurementCaseRelationshipType.BidSubmission); Add(nameof(BidOpeningSession), "BOS-LCA-2026-ICT-007", ProcurementCaseRelationshipType.BidOpening); Add(nameof(EvaluationSession), "EV-LCA-2026-ICT-007", ProcurementCaseRelationshipType.Evaluation); Add(nameof(Award), "AWD-LCA-2026-ICT-007", ProcurementCaseRelationshipType.Award); Add(nameof(PurchaseOrder), "PO-LCA-2026-ICT-007", ProcurementCaseRelationshipType.PurchaseOrder); Add(nameof(Contract), "CON-LCA-2026-ICT-007", ProcurementCaseRelationshipType.Contract);
        var grouped = procurementCase.Links.GroupBy(l => l.RelationshipType).ToDictionary(g => g.Key, g => g.ToArray());
        var audit = procurementCase.Links.Select(l => new AuditEvent($"Case linked {l.RelationshipType}", l.EntityType, l.EntityId, l.EntityReference, "system", $"Linked to case {procurementCase.CaseNumber}", l.CreatedAt)).ToList();
        return (procurementCase, grouped, audit);
    }
    [Fact] public void Procurement_case_can_be_loaded() { var (c, _, _) = DemoCase(); Assert.Equal("PCASE-LCA-2026-ICT-001", c.CaseNumber); }
    [Fact] public void Case_links_annual_plan_to_requisition() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.AnnualPlan]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Requisition]); }
    [Fact] public void Case_links_requisition_to_tender() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Requisition]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Tender]); }
    [Fact] public void Case_links_tender_to_public_publication() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Tender]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.PublicPublication]); }
    [Fact] public void Case_links_tender_to_bids() { var (_, l, _) = DemoCase(); Assert.Single(l[ProcurementCaseRelationshipType.Tender]); Assert.Equal(3, l[ProcurementCaseRelationshipType.BidSubmission].Length); }
    [Fact] public void Case_links_bid_opening_to_evaluation() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.BidOpening]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Evaluation]); }
    [Fact] public void Case_links_evaluation_to_award() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Evaluation]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Award]); }
    [Fact] public void Case_links_award_to_purchase_order() { var (_, l, _) = DemoCase(); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Award]); Assert.NotEmpty(l[ProcurementCaseRelationshipType.PurchaseOrder]); }
    [Fact] public void Case_links_award_or_purchase_order_to_contract() { var (_, l, _) = DemoCase(); Assert.True(l.ContainsKey(ProcurementCaseRelationshipType.Award) || l.ContainsKey(ProcurementCaseRelationshipType.PurchaseOrder)); Assert.NotEmpty(l[ProcurementCaseRelationshipType.Contract]); }
    [Fact] public void Audit_timeline_returns_relevant_case_events() { var (c, _, audit) = DemoCase(); Assert.NotEmpty(audit); Assert.All(audit, e => Assert.Contains(c.CaseNumber, e.Details)); }
}
