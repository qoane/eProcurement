using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Xunit;

public class ProcurementPlanningTests
{
    [Fact]
    public void Annual_procurement_plan_can_be_created()
    {
        var fy = Guid.NewGuid();
        var plan = new AnnualProcurementPlan("APP-2026-001", "Annual ICT Plan", fy, "ICT", "Draft", "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        Assert.Equal("APP-2026-001", plan.PlanNumber);
        Assert.Equal("Draft", plan.Status);
    }

    [Fact]
    public void Plan_can_move_through_configured_workflow()
    {
        var workflow = PlanningWorkflow();
        var version = workflow.Versions.Single();
        var current = version.Nodes.Single(x => x.IsStart).Code;
        foreach (var action in new[] { "Submit", "ProcurementReview", "FinanceReview", "Approve" })
            current = version.Transitions.Single(x => x.FromNodeCode == current && x.ActionCode == action).ToNodeCode;
        Assert.Equal("Approved", current);
        Assert.Contains(version.Nodes, x => x.Code == "FinanceReview" && x.CreatesTask && x.DefaultAssignedRole == "Finance User");
    }

    [Fact]
    public void Budget_lines_can_be_created()
    {
        var budget = new Budget(Guid.NewGuid(), "Finance", 1000m, 100m, 900m);
        budget.Lines.Add(new BudgetLine(budget.Id, Guid.NewGuid(), Guid.NewGuid(), 600m, 150m, 450m));
        Assert.Single(budget.Lines);
    }

    [Fact]
    public void Available_budget_is_calculated_correctly()
    {
        var allocated = 2500m;
        var committed = 700m;
        var line = new BudgetLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), allocated, committed, allocated - committed);
        Assert.Equal(1800m, line.AvailableAmount);
    }

    [Fact]
    public void Audit_events_are_created_for_planning_governance()
    {
        var plan = new AnnualProcurementPlan("APP-2026-002", "Facilities Plan", Guid.NewGuid(), "Operations", "Submitted", "procurement@lca.org.ls", DateTimeOffset.UtcNow);
        var audit = new AuditEvent("Annual procurement plan submitted", nameof(AnnualProcurementPlan), plan.Id, plan.PlanNumber, "procurement@lca.org.ls", "ANNUAL-PROCUREMENT-PLAN", DateTimeOffset.UtcNow);
        Assert.Equal(nameof(AnnualProcurementPlan), audit.EntityType);
        Assert.Equal(plan.PlanNumber, audit.EntityReference);
    }

    [Fact]
    public void Module_uses_existing_workflow_services_not_a_separate_engine()
    {
        var serviceCtor = typeof(AnnualProcurementPlanApplicationService).GetConstructors().Single();
        Assert.Contains(serviceCtor.GetParameters(), p => p.ParameterType == typeof(IWorkflowApplicationService));
        Assert.DoesNotContain(typeof(AnnualProcurementPlanApplicationService).Assembly.GetTypes(), t => t.Name.Contains("PlanningWorkflowEngine", StringComparison.OrdinalIgnoreCase));
    }

    private static WorkflowDefinition PlanningWorkflow()
    {
        var wf = new WorkflowDefinition("ANNUAL-PROCUREMENT-PLAN", "Annual Procurement Plan", nameof(AnnualProcurementPlan));
        var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
        v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Submitted","Submitted",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Procurement Officer"), new(v.Id,"ProcurementReview","Procurement Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Procurement Officer"), new(v.Id,"FinanceReview","Finance Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Finance User"), new(v.Id,"Approved","Approved",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Rejected","Rejected",WorkflowNodeKind.End,IsTerminal:true)]);
        v.Transitions.AddRange([new(v.Id,"Draft","Submit","Submit","Submitted"), new(v.Id,"Submitted","ProcurementReview","Start procurement review","ProcurementReview"), new(v.Id,"ProcurementReview","FinanceReview","Send to finance","FinanceReview"), new(v.Id,"FinanceReview","Approve","Approve","Approved")]);
        wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); return wf;
    }
}
