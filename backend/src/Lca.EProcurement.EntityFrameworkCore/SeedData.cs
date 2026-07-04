using Lca.EProcurement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.EntityFrameworkCore;

public static class SeedData
{
    public static readonly string[] Roles = ["Supplier", "ProcurementOfficer", "Evaluator", "Approver", "FinanceUser", "Auditor", "Administrator"];
    public static readonly string[] Users = ["supplier@demo.co.ls", "procurement@lca.org.ls", "evaluator@lca.org.ls", "approver@lca.org.ls", "finance@lca.org.ls", "auditor@lca.org.ls", "admin@lca.org.ls"];
    public static List<SupplierCategory> Categories() => [new("ICT Equipment"), new("Consulting Services"), new("Office Supplies"), new("Telecommunications"), new("Facilities Management")];
    public static Supplier DemoSupplier(SupplierCategory category) => new("SUP-LCA-2026-0001", "Maseru ICT Supplies Pty Ltd", SupplierStatus.Draft) { Categories = [category] };
    public static List<BusinessRuleDefinition> Rules() => [new("SUP-HAS-REG", "Supplier must have company registration document", "Supplier", "Supplier.Documents.Any(DocumentType == \"CompanyRegistration\")"), new("SUP-HAS-TAX", "Supplier must have tax clearance document", "Supplier", "Supplier.Documents.Any(DocumentType == \"TaxClearance\")"), new("SUP-HAS-CATEGORY", "Supplier must be assigned at least one category before approval", "Supplier", "Supplier.Categories.Any()")];
    public static WorkflowDefinition SupplierOnboardingWorkflow()
    {
        var wf = new WorkflowDefinition("SUPPLIER-ONBOARDING", "Supplier onboarding", nameof(Supplier));
        var version = SupplierOnboardingVersion(wf.Id);
        wf = wf with { PublishedVersionId = version.Id };
        wf.Versions.Add(version);
        return wf;
    }

    private static WorkflowVersion SupplierOnboardingVersion(Guid workflowDefinitionId)
    {
        var version = new WorkflowVersion(workflowDefinitionId, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
        version.Nodes.AddRange([
            new(version.Id, "Submitted", "Submitted", WorkflowNodeKind.Start, IsStart: true),
            new(version.Id, "DocumentCheck", "Document Check", WorkflowNodeKind.Task, CreatesTask: true, DefaultAssignedRole: "ProcurementOfficer"),
            new(version.Id, "Verification", "Verification", WorkflowNodeKind.Task, CreatesTask: true, DefaultAssignedRole: "Evaluator"),
            new(version.Id, "Approval", "Approval", WorkflowNodeKind.Task, CreatesTask: true, DefaultAssignedRole: "Approver"),
            new(version.Id, "Approved", "Approved", WorkflowNodeKind.End, IsTerminal: true),
            new(version.Id, "Rejected", "Rejected", WorkflowNodeKind.End, IsTerminal: true)
        ]);
        version.Transitions.AddRange([
            new(version.Id, "Submitted", "Submit", "Submit for verification", "DocumentCheck"),
            new(version.Id, "DocumentCheck", "DocumentsAccepted", "Documents accepted", "Verification", "SUP-HAS-REG"),
            new(version.Id, "Verification", "TaxVerified", "Tax verified", "Approval", "SUP-HAS-TAX"),
            new(version.Id, "Approval", "Approve", "Approve", "Approved", "SUP-HAS-CATEGORY"),
            new(version.Id, "Approval", "Reject", "Reject", "Rejected")
        ]);
        return version;
    }

    public static async Task SeedAsync(EProcurementDbContext db, CancellationToken cancellationToken = default)
    {
        foreach (var role in Roles)
            if (!await db.SeedMetadata.AnyAsync(x => x.Kind == "Role" && x.Code == role, cancellationToken)) db.SeedMetadata.Add(new("Role", role, role));
        foreach (var user in Users)
            if (!await db.SeedMetadata.AnyAsync(x => x.Kind == "DemoUser" && x.Code == user, cancellationToken)) db.SeedMetadata.Add(new("DemoUser", user, user));
        await db.SaveChangesAsync(cancellationToken);

        foreach (var category in Categories())
            if (!await db.SupplierCategories.AnyAsync(x => x.Name == category.Name, cancellationToken)) db.SupplierCategories.Add(category);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var rule in Rules())
            if (!await db.BusinessRuleDefinitions.AnyAsync(x => x.Code == rule.Code, cancellationToken)) db.BusinessRuleDefinitions.Add(rule);

        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "SUPPLIER-ONBOARDING", cancellationToken)) db.WorkflowDefinitions.Add(SupplierOnboardingWorkflow());
        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();

        if (!await db.WorkflowMappings.AnyAsync(x => x.EntityType == nameof(Supplier) && x.ActionCode == "Submit", cancellationToken)) db.WorkflowMappings.Add(new WorkflowMapping(nameof(Supplier), "Submit", "SUPPLIER-ONBOARDING"));
        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();

        var supplierWorkflow = await db.WorkflowDefinitions
            .AsNoTracking()
            .Include(x => x.Versions).ThenInclude(x => x.Nodes)
            .Include(x => x.Versions).ThenInclude(x => x.Transitions)
            .SingleAsync(x => x.Code == "SUPPLIER-ONBOARDING", cancellationToken);
        var supplierVersion = supplierWorkflow.Versions.FirstOrDefault(x => x.Id == supplierWorkflow.PublishedVersionId)
            ?? supplierWorkflow.Versions.FirstOrDefault(x => x.Status == WorkflowVersionStatus.Published)
            ?? supplierWorkflow.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault();
        if (supplierVersion is null)
        {
            supplierVersion = SupplierOnboardingVersion(supplierWorkflow.Id);
            db.WorkflowVersions.Add(supplierVersion);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }
        var supplierVersionId = supplierVersion.Id;
        var templateVersion = SupplierOnboardingVersion(supplierWorkflow.Id);
        foreach (var node in templateVersion.Nodes)
            if (!supplierVersion.Nodes.Any(x => x.Code == node.Code)) db.WorkflowNodes.Add(node with { WorkflowVersionId = supplierVersionId });
        foreach (var transition in templateVersion.Transitions)
            if (!supplierVersion.Transitions.Any(x => x.FromNodeCode == transition.FromNodeCode && x.ActionCode == transition.ActionCode)) db.WorkflowTransitions.Add(transition with { WorkflowVersionId = supplierVersionId });
        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();

        var publishedAt = supplierVersion.PublishedAt ?? DateTimeOffset.UtcNow;
        var publishedBy = string.IsNullOrWhiteSpace(supplierVersion.PublishedBy) ? "system" : supplierVersion.PublishedBy;
        await db.WorkflowDefinitions
            .Where(x => x.Id == supplierWorkflow.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.PublishedVersionId, supplierVersion.Id), cancellationToken);
        await db.WorkflowVersions
            .Where(x => x.Id == supplierVersion.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, WorkflowVersionStatus.Published)
                .SetProperty(x => x.PublishedAt, publishedAt)
                .SetProperty(x => x.PublishedBy, publishedBy), cancellationToken);
        var supplierTransitions = await db.WorkflowTransitions.AsNoTracking().Where(x => x.WorkflowVersionId == supplierVersionId).ToListAsync(cancellationToken);
        var configuredEffects = new Dictionary<string, string>
        {
            ["Submit"] = "Submitted",
            ["DocumentsAccepted"] = "UnderVerification",
            ["TaxVerified"] = "UnderVerification",
            ["Approve"] = "Approved",
            ["Reject"] = "Rejected"
        };
        foreach (var transition in supplierTransitions)
        {
            if (configuredEffects.TryGetValue(transition.ActionCode, out var status) && !await db.WorkflowTransitionEffects.AnyAsync(x => x.TriggerTransitionId == transition.Id && x.EntityType == nameof(Supplier) && x.PropertyName == nameof(Supplier.Status), cancellationToken))
                db.WorkflowTransitionEffects.Add(new WorkflowTransitionEffect(nameof(Supplier), nameof(Supplier.Status), status, transition.Id));
        }
        await db.SaveChangesAsync(cancellationToken);

        if (!await db.Suppliers.AnyAsync(x => x.ReferenceNumber == "SUP-LCA-2026-0001", cancellationToken))
        {
            var category = await db.SupplierCategories.SingleAsync(x => x.Name == "ICT Equipment", cancellationToken);
            var supplier = DemoSupplier(category) with { Status = SupplierStatus.UnderVerification };
            supplier.Documents.Add(new SupplierDocument(supplier.Id, "CompanyRegistration", "registration.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
            supplier.Documents.Add(new SupplierDocument(supplier.Id, "TaxClearance", "tax.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
            db.Suppliers.Add(supplier);
            db.AuditEvents.Add(new AuditEvent("Seeded demo supplier", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, "system", "Demo supplier inserted by idempotent seed", DateTimeOffset.UtcNow));
        }
        await db.SaveChangesAsync(cancellationToken);
    }
}
