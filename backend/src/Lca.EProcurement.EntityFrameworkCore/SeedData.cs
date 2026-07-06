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

        if (!await db.DocumentRequirementSets.AnyAsync(x => x.Name == "Supplier Documents", cancellationToken))
        {
            var set = new DocumentRequirementSet("Supplier Documents", "Configuration-driven document requirements for supplier registration.", nameof(Supplier));
            set.Requirements.Add(new DocumentRequirement(set.Id, "CompanyRegistration", true, 1, 1, ".pdf,.png,.jpg", 10_485_760, "SUP-HAS-REG"));
            set.Requirements.Add(new DocumentRequirement(set.Id, "TaxClearance", true, 1, 1, ".pdf,.png,.jpg", 10_485_760, "SUP-HAS-TAX"));
            db.DocumentRequirementSets.Add(set);
        }
        if (!await db.ApprovalMatrices.AnyAsync(x => x.Name == "Standard Supplier Approval", cancellationToken))
        {
            var matrix = new ApprovalMatrix("Standard Supplier Approval", "Reusable approval chain for supplier onboarding.", nameof(Supplier));
            matrix.Steps.Add(new ApprovalStep(matrix.Id, "ProcurementOfficer", 1));
            matrix.Steps.Add(new ApprovalStep(matrix.Id, "Evaluator", 2));
            matrix.Steps.Add(new ApprovalStep(matrix.Id, "Approver", 3, RuleCode: "SUP-HAS-CATEGORY"));
            db.ApprovalMatrices.Add(matrix);
        }
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
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "SUPPLIER-REGISTRATION-FORM", cancellationToken))
        {
            var form = new FormDefinition("SUPPLIER-REGISTRATION-FORM", "Supplier Registration", nameof(Supplier));
            var version = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            var profile = new FormSection(version.Id, "profile", "Organisation profile", 1);
            profile.Fields.Add(new FormField(profile.Id, "legalName", "Legal name", "text", 1, true));
            profile.Fields.Add(new FormField(profile.Id, "registrationNumber", "Registration number", "text", 2, true));
            var contact = new FormSection(version.Id, "contact", "Primary contact", 2);
            contact.Fields.Add(new FormField(contact.Id, "contactEmail", "Contact email", "email", 1, true));
            contact.Fields.Add(new FormField(contact.Id, "contactPhone", "Contact phone", "text", 2, true));
            version.Sections.AddRange([profile, contact]);
            form = form with { ActiveVersionId = version.Id };
            form.Versions.Add(version);
            db.FormDefinitions.Add(form);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }
        var activeWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "SUPPLIER-ONBOARDING").Select(x => x.Id).SingleAsync(cancellationToken);
        var activeFormId = await db.FormDefinitions.Where(x => x.Code == "SUPPLIER-REGISTRATION-FORM").Select(x => x.Id).SingleAsync(cancellationToken);
        var documentSetId = await db.DocumentRequirementSets.Where(x => x.Name == "Supplier Documents").Select(x => x.Id).SingleAsync(cancellationToken);
        var approvalMatrixId = await db.ApprovalMatrices.Where(x => x.Name == "Standard Supplier Approval").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "SUPPLIER-REGISTRATION", cancellationToken))
            db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("SUPPLIER-REGISTRATION", "Supplier Registration", "End-to-end supplier onboarding assembled from workflow, form, document, rule, and approval configuration.", nameof(Supplier), activeWorkflowId, activeFormId, documentSetId, approvalMatrixId, BusinessProcessStatus.Published));
        await db.SaveChangesAsync(cancellationToken);
        if (!await db.Applications.AnyAsync(x => x.Code == "PROCUREMENT", cancellationToken))
            db.Applications.Add(new Application("PROCUREMENT", "Procurement", "Procurement workspace containing governed source-to-contract modules.", "Briefcase", "LCA Indigo", "/app/suppliers", "/app", @"[""Supplier Management"",""Requisitions"",""Tenders"",""Evaluation"",""Contracts"",""Reports"",""Studio""]", Status: MetadataStatus.Active, CreatedBy: "system"));
        await db.SaveChangesAsync(cancellationToken);


        if (!await db.NavigationDefinitions.AnyAsync(x => x.Code == "MAIN", cancellationToken))
        {
            var nav = new NavigationDefinition("MAIN", "Main navigation", "Administrator-configured sidebar navigation for ProcuraFlow.", Status: MetadataStatus.Active, CreatedBy: "system");
            var procurement = new NavigationItem(nav.Id, "procurement", "Procurement", "Group", null, "BriefcaseBusiness", 10, IsCollapsible: true);
            procurement.Children.Add(new NavigationItem(nav.Id, "suppliers", "Suppliers", "Link", "/app/suppliers", "Users", 10, procurement.Id, PermissionsJson: @"[""SupplierManagement.View""]"));
            procurement.Children.Add(new NavigationItem(nav.Id, "tenders", "Tenders", "Link", "/app/tenders", "ScrollText", 20, procurement.Id, PermissionsJson: @"[""Tender.View""]"));
            var administration = new NavigationItem(nav.Id, "administration", "Administration", "Group", null, "Settings", 20, IsCollapsible: true);
            administration.Children.Add(new NavigationItem(nav.Id, "workflows", "Workflows", "Link", "/app/workflows/designer", "Workflow", 10, administration.Id, PermissionsJson: @"[""Workflow.Admin""]"));
            administration.Children.Add(new NavigationItem(nav.Id, "rules", "Rules", "Link", "/app/rules", "ShieldCheck", 20, administration.Id, PermissionsJson: @"[""Rules.Admin""]"));
            var studio = new NavigationItem(nav.Id, "studio", "Studio", "Group", null, "Blocks", 30, IsCollapsible: true);
            studio.Children.Add(new NavigationItem(nav.Id, "pages", "Pages", "Link", "/app/studio/pages", "PanelTop", 10, studio.Id, PermissionsJson: @"[""Studio.Pages""]"));
            studio.Children.Add(new NavigationItem(nav.Id, "entities", "Entities", "Link", "/app/studio/entities", "Database", 20, studio.Id, PermissionsJson: @"[""Studio.Entities""]"));
            studio.Children.Add(new NavigationItem(nav.Id, "dashboards", "Dashboards", "Link", "/app/dashboards", "LayoutDashboard", 30, studio.Id, PermissionsJson: @"[""Studio.Dashboards""]"));
            nav.Items.AddRange([procurement, administration, studio]);
            db.NavigationDefinitions.Add(nav);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }

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
