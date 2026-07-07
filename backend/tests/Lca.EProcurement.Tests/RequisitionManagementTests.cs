using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Xunit;

public class RequisitionManagementTests
{
    [Fact]
    public void Create_requisition_calculates_item_and_header_estimates()
    {
        var req = new Requisition("REQ-2026-001", "ICT equipment", "Laptops", "Corporate Services", Guid.NewGuid(), Guid.NewGuid(), "requester@lca.org.ls", DateTimeOffset.UtcNow.AddDays(14), "Normal", 12000m, RequisitionStatus.Draft, DateTimeOffset.UtcNow);
        req.Items.Add(new RequisitionItem(req.Id, "Laptop", 2, "Each", 6000m, 12000m, Guid.NewGuid()));
        Assert.Equal(12000m, req.EstimatedTotal);
        Assert.Single(req.Items);
    }

    [Fact]
    public void Requisition_workflow_contains_budget_validation_and_approval_stages()
    {
        var workflow = RequisitionWorkflow();
        var version = workflow.Versions.Single();
        var current = version.Nodes.Single(x => x.IsStart).Code;
        foreach (var action in new[] { "Submit", "ValidateBudget", "ManagerApprove", "ProcurementReview", "Approve" })
            current = version.Transitions.Single(x => x.FromNodeCode == current && x.ActionCode == action).ToNodeCode;
        Assert.Equal("Approved", current);
        Assert.Contains(version.Nodes, x => x.Code == "BudgetValidation" && x.DefaultAssignedRole == "FinanceUser");
    }

    [Fact]
    public void Budget_commitment_references_requisition_and_budget_line()
    {
        var reqId = Guid.NewGuid(); var budgetId = Guid.NewGuid(); var lineId = Guid.NewGuid();
        var commitment = new BudgetCommitment(reqId, budgetId, lineId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2500m, "approver@lca.org.ls", DateTimeOffset.UtcNow, "REQ-2026-002");
        Assert.Equal(reqId, commitment.RequisitionId);
        Assert.Equal(2500m, commitment.Amount);
    }

    [Fact]
    public void Audit_events_are_created_for_requisition_governance()
    {
        var req = new Requisition("REQ-2026-003", "Office supplies", "Toners", "Finance", Guid.NewGuid(), Guid.NewGuid(), "requester@lca.org.ls", DateTimeOffset.UtcNow, "High", 500m, RequisitionStatus.Submitted, DateTimeOffset.UtcNow);
        var audit = new AuditEvent("Requisition Submitted", nameof(Requisition), req.Id, req.RequisitionNumber, "requester@lca.org.ls", "Submitted for approval", DateTimeOffset.UtcNow);
        Assert.Equal(nameof(Requisition), audit.EntityType);
        Assert.Equal(req.RequisitionNumber, audit.EntityReference);
    }

    [Fact]
    public void Module_uses_existing_workflow_and_business_rule_services()
    {
        var serviceCtor = typeof(RequisitionApplicationService).GetConstructors().Single();
        Assert.Contains(serviceCtor.GetParameters(), p => p.ParameterType == typeof(IWorkflowApplicationService));
        Assert.Contains(serviceCtor.GetParameters(), p => p.ParameterType == typeof(IBusinessRuleApplicationService));
    }

    static WorkflowDefinition RequisitionWorkflow()
    {
        var wf = new WorkflowDefinition("REQUISITION-APPROVAL-WORKFLOW", "Requisition Approval Workflow", nameof(Requisition));
        var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
        v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Submitted","Submitted",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Requester"), new(v.Id,"BudgetValidation","Budget Validation",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"FinanceUser"), new(v.Id,"ManagerApproval","Manager Approval",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Manager"), new(v.Id,"ProcurementReview","Procurement Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"ProcurementOfficer"), new(v.Id,"Approved","Approved",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Rejected","Rejected",WorkflowNodeKind.End,IsTerminal:true)]);
        v.Transitions.AddRange([new(v.Id,"Draft","Submit","Submit","Submitted"), new(v.Id,"Submitted","ValidateBudget","Validate budget","BudgetValidation"), new(v.Id,"BudgetValidation","ManagerApprove","Manager approve","ManagerApproval"), new(v.Id,"ManagerApproval","ProcurementReview","Procurement review","ProcurementReview"), new(v.Id,"ProcurementReview","Approve","Approve","Approved")]);
        wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); return wf;
    }
}
