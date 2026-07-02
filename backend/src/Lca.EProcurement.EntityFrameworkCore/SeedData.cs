using Lca.EProcurement.Domain;
namespace Lca.EProcurement.EntityFrameworkCore;

public static class SeedData
{
    public static readonly string[] Roles = ["Supplier", "ProcurementOfficer", "Evaluator", "Approver", "FinanceUser", "Auditor", "Administrator"];
    public static readonly string[] Users = ["supplier@demo.co.ls", "procurement@lca.org.ls", "evaluator@lca.org.ls", "approver@lca.org.ls", "finance@lca.org.ls", "auditor@lca.org.ls", "admin@lca.org.ls"];
    public static List<SupplierCategory> Categories() => [new("ICT Equipment"), new("Consulting Services"), new("Office Supplies"), new("Telecommunications"), new("Facilities Management")];
    public static Supplier DemoSupplier(SupplierCategory category) => new("SUP-LCA-2026-0001", "Maseru ICT Supplies Pty Ltd", SupplierStatus.Draft) { Categories = [category] };
    public static List<BusinessRuleDefinition> Rules() => [new("SUP-HAS-REG", "Supplier must have company registration document", "Supplier", "HasDocument:CompanyRegistration"), new("SUP-HAS-TAX", "Supplier must have tax clearance document", "Supplier", "HasDocument:TaxClearance"), new("SUP-HAS-CATEGORY", "Supplier must be assigned at least one category before approval", "Supplier", "HasAtLeastOneCategory")];
    public static WorkflowDefinition SupplierOnboardingWorkflow()
    {
        var wf = new WorkflowDefinition("SUPPLIER-ONBOARDING", "Supplier onboarding");
        wf.Steps.AddRange([new(wf.Id, "Submitted", "Submitted", false), new(wf.Id, "DocumentCheck", "Document Check", true), new(wf.Id, "Verification", "Verification", true), new(wf.Id, "Approval", "Approval", true), new(wf.Id, "Approved", "Approved", false), new(wf.Id, "Rejected", "Rejected", false)]);
        wf.Transitions.AddRange([new(wf.Id, "Submitted", "SubmitForVerification", "DocumentCheck"), new(wf.Id, "DocumentCheck", "DocumentsAccepted", "Verification", "SUP-HAS-REG"), new(wf.Id, "Verification", "TaxVerified", "Approval", "SUP-HAS-TAX"), new(wf.Id, "Approval", "Approve", "Approved", "SUP-HAS-CATEGORY"), new(wf.Id, "Approval", "Reject", "Rejected")]);
        return wf;
    }
}
