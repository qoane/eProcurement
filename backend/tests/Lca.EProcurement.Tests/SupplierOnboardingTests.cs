using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Xunit;

public class SupplierOnboardingTests
{
    [Fact] public void Supplier_approval_path_uses_workflow_rules_and_audit()
    {
        var category = SeedData.Categories()[0];
        var supplier = SeedData.DemoSupplier(category) with { Status = SupplierStatus.Submitted };
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "CompanyRegistration", "registration.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "TaxClearance", "tax.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        var audit = new InMemoryAuditSink();
        var ruleLogs = new List<BusinessRuleExecutionLog>();
        var rules = new BusinessRulesEngine(SeedData.Rules(), ruleLogs, audit);
        var workflow = SeedData.SupplierOnboardingWorkflow();
        var engine = new WorkflowEngine([workflow], [], [], [], rules, audit);
        var instance = engine.Start("SUPPLIER-ONBOARDING", supplier, "supplier@demo.co.ls");
        instance = engine.Move(instance, supplier, "SubmitForVerification", "procurement@lca.org.ls");
        instance = engine.Move(instance, supplier, "DocumentsAccepted", "procurement@lca.org.ls");
        instance = engine.Move(instance, supplier, "TaxVerified", "procurement@lca.org.ls");
        instance = engine.Move(instance, supplier, "Approve", "approver@lca.org.ls");
        Assert.Equal("Approved", instance.CurrentStepCode);
        Assert.Equal(3, ruleLogs.Count);
        Assert.Contains(audit.Events, e => e.EventType == "Workflow started");
        Assert.Contains(audit.Events, e => e.EventType == "Rule evaluated");
    }
}
