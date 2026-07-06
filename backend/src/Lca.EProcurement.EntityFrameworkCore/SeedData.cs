using Lca.EProcurement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.EntityFrameworkCore;

public static class SeedData
{
    public static readonly string[] Roles = ["Supplier", "ProcurementOfficer", "Evaluator", "Approver", "FinanceUser", "Auditor", "Administrator"];
    public static readonly string[] Users = ["supplier@demo.co.ls", "procurement@lca.org.ls", "evaluator@lca.org.ls", "approver@lca.org.ls", "finance@lca.org.ls", "auditor@lca.org.ls", "admin@lca.org.ls"];
    public static List<SupplierCategory> Categories() => [new("ICT Equipment"), new("Consulting Services"), new("Office Supplies"), new("Telecommunications"), new("Facilities Management")];
    public static Supplier DemoSupplier(SupplierCategory category) => new("SUP-LCA-2026-0001", "Maseru ICT Supplies Pty Ltd", SupplierStatus.Draft) { Categories = [category] };
    public static List<BusinessRuleDefinition> Rules() => [new("SUP-HAS-REG", "Supplier must have company registration document", "Supplier", "Supplier.Documents.Any(DocumentType == \"CompanyRegistration\")"), new("SUP-HAS-TAX", "Supplier must have tax clearance document", "Supplier", "Supplier.Documents.Any(DocumentType == \"TaxClearance\")"), new("SUP-HAS-CATEGORY", "Supplier must be assigned at least one category before approval", "Supplier", "Supplier.Categories.Any()", Category: "Eligibility", Status: BusinessRuleStatus.Published, FailureMessage: "At least one supplier category is required.", PublishedAt: DateTimeOffset.UtcNow, PublishedBy: "seed")];
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


    public static List<ComponentDefinition> ComponentLibrary()
    {
        static ComponentDefinition Component(string code, string name, string category, string renderer, string properties, string icon, string tags) => new(
            code,
            name,
            $"Reusable {name.ToLowerInvariant()} component with governed configuration metadata.",
            category,
            renderer,
            properties,
            @"[{""code"":""change"",""name"":""Value changed"",""description"":""Raised when the component value or state changes.""}]",
            @"[{""code"":""required"",""name"":""Required"",""expression"":""value != null && value != ''"",""message"":""A value is required.""}]",
            $@"{{""icon"":""{icon}"",""supportsBinding"":true,""supportsResponsiveLayout"":true,""allowedRegions"":[""main"",""sidebar"",""modal""],""tags"":{tags}}}",
            Status: MetadataStatus.Active,
            CreatedBy: "system");
        const string textProps = @"[{""code"":""label"",""name"":""Label"",""dataType"":""string"",""required"":true},{""code"":""placeholder"",""name"":""Placeholder"",""dataType"":""string""},{""code"":""helpText"",""name"":""Help text"",""dataType"":""string""},{""code"":""required"",""name"":""Required"",""dataType"":""boolean"",""defaultValue"":""false""}]";
        const string choiceProps = @"[{""code"":""label"",""name"":""Label"",""dataType"":""string"",""required"":true},{""code"":""options"",""name"":""Options"",""dataType"":""array"",""required"":true},{""code"":""defaultValue"",""name"":""Default value"",""dataType"":""string""}]";
        const string containerProps = @"[{""code"":""title"",""name"":""Title"",""dataType"":""string""},{""code"":""collapsible"",""name"":""Collapsible"",""dataType"":""boolean"",""defaultValue"":""false""},{""code"":""components"",""name"":""Child components"",""dataType"":""array""}]";
        return [
            Component("TEXTBOX", "Textbox", "Inputs", "Textbox", textProps, "TextCursorInput", @"[""form"",""text""]"),
            Component("TEXTAREA", "Textarea", "Inputs", "Textarea", textProps, "AlignLeft", @"[""form"",""long-text""]"),
            Component("DROPDOWN", "Dropdown", "Choices", "Dropdown", choiceProps, "ListFilter", @"[""form"",""choice""]"),
            Component("LOOKUP", "Lookup", "Choices", "Lookup", choiceProps, "Search", @"[""form"",""reference-data""]"),
            Component("AUTOCOMPLETE", "Autocomplete", "Choices", "Autocomplete", choiceProps, "ListPlus", @"[""form"",""search""]"),
            Component("CHECKBOX", "Checkbox", "Choices", "Checkbox", choiceProps, "SquareCheck", @"[""form"",""boolean""]"),
            Component("RADIO", "Radio", "Choices", "Radio", choiceProps, "CircleDot", @"[""form"",""choice""]"),
            Component("DATE", "Date", "Date & Time", "Date", textProps, "Calendar", @"[""form"",""date""]"),
            Component("DATETIME", "DateTime", "Date & Time", "DateTime", textProps, "CalendarClock", @"[""form"",""date-time""]"),
            Component("MONEY", "Money", "Numeric", "Money", textProps, "BadgeDollarSign", @"[""form"",""currency""]"),
            Component("PERCENTAGE", "Percentage", "Numeric", "Percentage", textProps, "Percent", @"[""form"",""numeric""]"),
            Component("PHONE", "Phone", "Inputs", "Phone", textProps, "Phone", @"[""form"",""contact""]"),
            Component("EMAIL", "Email", "Inputs", "Email", textProps, "Mail", @"[""form"",""contact""]"),
            Component("FILE-UPLOAD", "File Upload", "Content", "FileUpload", @"[{""code"":""allowedExtensions"",""name"":""Allowed extensions"",""dataType"":""string""},{""code"":""maximumFileSize"",""name"":""Maximum file size"",""dataType"":""number""},{""code"":""multiple"",""name"":""Allow multiple files"",""dataType"":""boolean""}]", "Upload", @"[""document"",""evidence""]"),
            Component("RICH-TEXT", "Rich Text", "Content", "RichText", textProps, "Pilcrow", @"[""content"",""formatted-text""]"),
            Component("DATA-GRID", "Data Grid", "Data Display", "DataGrid", @"[{""code"":""datasource"",""name"":""Datasource"",""dataType"":""object"",""required"":true},{""code"":""columns"",""name"":""Columns"",""dataType"":""array"",""required"":true},{""code"":""pageSize"",""name"":""Page size"",""dataType"":""number"",""defaultValue"":""10""}]", "Table", @"[""table"",""records""]"),
            Component("TIMELINE", "Timeline", "Data Display", "Timeline", @"[{""code"":""items"",""name"":""Items"",""dataType"":""array""},{""code"":""orientation"",""name"":""Orientation"",""dataType"":""string"",""defaultValue"":""vertical""}]", "Milestone", @"[""history"",""events""]"),
            Component("CHART", "Chart", "Analytics", "Chart", @"[{""code"":""chartType"",""name"":""Chart type"",""dataType"":""string"",""required"":true},{""code"":""dataset"",""name"":""Dataset"",""dataType"":""object"",""required"":true}]", "ChartNoAxesCombined", @"[""analytics"",""visualization""]"),
            Component("TABS", "Tabs", "Layout", "Tabs", containerProps, "PanelTop", @"[""layout"",""container""]"),
            Component("ACCORDION", "Accordion", "Layout", "Accordion", containerProps, "ListCollapse", @"[""layout"",""container""]"),
            Component("CARD", "Card", "Layout", "Card", containerProps, "PanelTopOpen", @"[""layout"",""surface""]"),
            Component("BUTTON", "Button", "Actions", "Button", @"[{""code"":""label"",""name"":""Label"",""dataType"":""string"",""required"":true},{""code"":""variant"",""name"":""Variant"",""dataType"":""string"",""defaultValue"":""primary""},{""code"":""actionCode"",""name"":""Action code"",""dataType"":""string""}]", "MousePointerClick", @"[""action"",""command""]"),
            Component("WORKFLOW-STATUS", "Workflow Status", "Workflow", "WorkflowStatus", @"[{""code"":""workflowCode"",""name"":""Workflow code"",""dataType"":""string""},{""code"":""statusField"",""name"":""Status field"",""dataType"":""string""}]", "Workflow", @"[""workflow"",""status""]"),
            Component("AUDIT-TIMELINE", "Audit Timeline", "Audit", "AuditTimeline", @"[{""code"":""entityType"",""name"":""Entity type"",""dataType"":""string""},{""code"":""entityId"",""name"":""Entity id binding"",""dataType"":""string""}]", "History", @"[""audit"",""timeline""]"),
            Component("SIGNATURE", "Signature", "Content", "Signature", @"[{""code"":""signerRole"",""name"":""Signer role"",""dataType"":""string""},{""code"":""requireTimestamp"",""name"":""Require timestamp"",""dataType"":""boolean"",""defaultValue"":""true""}]", "Signature", @"[""approval"",""signature""]"),
            Component("COMMENTS", "Comments", "Collaboration", "Comments", @"[{""code"":""threadKey"",""name"":""Thread key"",""dataType"":""string""},{""code"":""allowAttachments"",""name"":""Allow attachments"",""dataType"":""boolean"",""defaultValue"":""false""}]", "MessagesSquare", @"[""collaboration"",""comments""]")
        ];
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



        foreach (var component in ComponentLibrary())
            if (!await db.ComponentDefinitions.AnyAsync(x => x.Code == component.Code, cancellationToken)) db.ComponentDefinitions.Add(component);
        await db.SaveChangesAsync(cancellationToken);


        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "SUPPLIER-LIST", cancellationToken))
            db.PageDefinitions.Add(new PageDefinition(
                "SUPPLIER-LIST",
                "Suppliers",
                "Enterprise supplier master data composed from page, layout, component, data source, and permission metadata.",
                PageType.DataGrid,
                @"{""entity"":""Supplier"",""mode"":""Api"",""endpoint"":""/api/suppliers"",""keyField"":""referenceNumber""}",
                @"{""template"":""DataGrid"",""columns"":12,""density"":""Comfortable"",""regions"":[""main""]}",
                @"[{""code"":""register"",""label"":""Register supplier"",""kind"":""Button"",""actionCode"":""register""}]",
                @"[{""code"":""register"",""label"":""Register supplier"",""kind"":""Primary"",""target"":""/app/suppliers/register""},{""code"":""open-detail"",""label"":""Open detail"",""kind"":""Row"",""target"":""/app/suppliers/{referenceNumber}""}]",
                @"[{""code"":""search"",""label"":""Search suppliers"",""field"":""legalName"",""operator"":""Contains""}]",
                @"[{""code"":""reference"",""label"":""Reference"",""field"":""referenceNumber"",""displayOrder"":10,""sortable"":true,""searchable"":true},{""code"":""supplier"",""label"":""Supplier"",""field"":""legalName"",""displayOrder"":20,""sortable"":true,""searchable"":true},{""code"":""status"",""label"":""Status"",""field"":""status"",""displayOrder"":30,""sortable"":true,""searchable"":true},{""code"":""categories"",""label"":""Categories"",""field"":""categories"",""displayOrder"":40}]",
                @"[{""code"":""supplier-grid"",""name"":""Supplier grid"",""componentType"":""DataGrid"",""region"":""main"",""displayOrder"":10,""configuration"":{""componentDefinitionCode"":""DATA-GRID""}}]",
                @"[{""role"":""ProcurementOfficer"",""access"":""View""},{""role"":""Administrator"",""access"":""Manage""}]",
                @"{""route"":""/app/suppliers"",""parentRoute"":""/app"",""menuGroup"":""Procurement"",""showInNavigation"":true}",
                Status: MetadataStatus.Active,
                CreatedBy: "system"));
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
