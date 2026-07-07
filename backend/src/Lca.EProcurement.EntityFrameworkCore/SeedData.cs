using Lca.EProcurement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.EntityFrameworkCore;

public static class SeedData
{
    public static readonly string[] Roles = ["Supplier", "ProcurementOfficer", "Evaluator", "Approver", "FinanceUser", "Auditor", "Administrator"];
    public static readonly string[] Users = ["supplier@demo.co.ls", "procurement@lca.org.ls", "evaluator@lca.org.ls", "approver@lca.org.ls", "finance@lca.org.ls", "auditor@lca.org.ls", "admin@lca.org.ls"];
    public static List<SupplierCategory> Categories() => [new("ICT Equipment"), new("Consulting Services"), new("Office Supplies"), new("Telecommunications"), new("Facilities Management")];
    public static Supplier DemoSupplier(SupplierCategory category) => new("SUP-LCA-2026-0001", "Maseru ICT Supplies Pty Ltd", SupplierStatus.Draft) { Categories = [category] };
    public static Tender SampleTender()
    {
        var tender = new Tender("RFP-LCA-2026-0001", "National Broadband Quality of Service Monitoring Platform", "Sample RFP tender for a standards-based monitoring platform, implementation services, support, and knowledge transfer.", TenderType.RFP, "Open Tender", TenderStatus.Published, DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(30), "system", DateTimeOffset.UtcNow.AddDays(-5), DateTimeOffset.UtcNow.AddDays(-2), "system");
        tender.Lots.Add(new TenderLot(tender.Id, "LOT-1", "QoS monitoring platform", "Software platform, probes, implementation, support, and training."));
        tender.Documents.Add(new TenderDocument(tender.Id, "TermsOfReference", "qos-monitoring-tor.pdf", "Terms of reference and scope of services.", true, DateTimeOffset.UtcNow.AddDays(-5), "system"));
        tender.Documents.Add(new TenderDocument(tender.Id, "PricingSchedule", "pricing-schedule.xlsx", "Structured pricing schedule template.", true, DateTimeOffset.UtcNow.AddDays(-5), "system"));
        tender.StatusHistory.Add(new TenderStatusHistory(tender.Id, TenderStatus.Draft, TenderStatus.Published, "system", DateTimeOffset.UtcNow.AddDays(-2), "Seed tender published"));
        return tender;
    }
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
            Component("COMMENTS", "Comments", "Collaboration", "Comments", @"[{""code"":""threadKey"",""name"":""Thread key"",""dataType"":""string""},{""code"":""allowAttachments"",""name"":""Allow attachments"",""dataType"":""boolean"",""defaultValue"":""false""}]", "MessagesSquare", @"[""collaboration"",""comments""]"),
            Component("CONFIGURED-REGISTRATION", "Configured Registration", "Composition", "ConfiguredRegistration", @"[{""code"":""configurationEndpoint"",""name"":""Configuration endpoint"",""dataType"":""string"",""required"":true},{""code"":""submitEndpoint"",""name"":""Submit endpoint"",""dataType"":""string"",""required"":true},{""code"":""referencePrefix"",""name"":""Reference prefix"",""dataType"":""string""},{""code"":""actor"",""name"":""Default actor"",""dataType"":""string""}]", "Blocks", @"[""application"",""business-process"",""dynamic-form"",""workflow"",""rules"",""documents"",""approval""]")
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
        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "TENDER-PUBLISHING", cancellationToken))
        {
            var wf = new WorkflowDefinition("TENDER-PUBLISHING", "Tender Publishing", nameof(Tender));
            var version = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            version.Nodes.AddRange([new(version.Id, "Draft", "Draft", WorkflowNodeKind.Start, IsStart: true), new(version.Id, "ProcurementReview", "Procurement Review", WorkflowNodeKind.Task, CreatesTask: true, DefaultAssignedRole: "ProcurementOfficer"), new(version.Id, "ApprovedForPublication", "Approved for Publication", WorkflowNodeKind.Task, CreatesTask: true, DefaultAssignedRole: "Approver"), new(version.Id, "Published", "Published", WorkflowNodeKind.Task), new(version.Id, "Closed", "Closed", WorkflowNodeKind.End, IsTerminal: true), new(version.Id, "Cancelled", "Cancelled", WorkflowNodeKind.End, IsTerminal: true)]);
            version.Transitions.AddRange([new(version.Id, "Draft", "SubmitForReview", "Submit for procurement review", "ProcurementReview"), new(version.Id, "ProcurementReview", "Approve", "Approve for publication", "ApprovedForPublication"), new(version.Id, "ApprovedForPublication", "Publish", "Publish", "Published"), new(version.Id, "Published", "Close", "Close", "Closed"), new(version.Id, "Draft", "Cancel", "Cancel", "Cancelled"), new(version.Id, "Published", "Cancel", "Cancel", "Cancelled")]);
            wf = wf with { PublishedVersionId = version.Id }; wf.Versions.Add(version); db.WorkflowDefinitions.Add(wf);
        }
        await db.SaveChangesAsync(cancellationToken);
        var tenderWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "TENDER-PUBLISHING").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "TENDER-FORM", cancellationToken))
        {
            var form = new FormDefinition("TENDER-FORM", "Tender Form", nameof(Tender));
            var version = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            var basics = new FormSection(version.Id, "basics", "Tender basics", 1);
            basics.Fields.Add(new FormField(basics.Id, "tenderNumber", "Tender number", "text", 1, true)); basics.Fields.Add(new FormField(basics.Id, "title", "Title", "text", 2, true)); basics.Fields.Add(new FormField(basics.Id, "tenderType", "Tender type", "select", 3, true));
            var dates = new FormSection(version.Id, "dates", "Publication and closing", 2); dates.Fields.Add(new FormField(dates.Id, "closingDate", "Closing date", "date", 1, true));
            version.Sections.AddRange([basics, dates]); form = form with { ActiveVersionId = version.Id }; form.Versions.Add(version); db.FormDefinitions.Add(form);
        }
        if (!await db.DocumentRequirementSets.AnyAsync(x => x.Name == "Tender Document Requirements", cancellationToken))
        {
            var set = new DocumentRequirementSet("Tender Document Requirements", "Required tender document metadata for RFIs, RFQs and RFPs.", nameof(Tender));
            set.Requirements.Add(new DocumentRequirement(set.Id, "TermsOfReference", true, 1, 1, ".pdf,.docx", 20_971_520));
            set.Requirements.Add(new DocumentRequirement(set.Id, "PricingSchedule", true, 1, 1, ".xlsx,.pdf", 10_485_760));
            db.DocumentRequirementSets.Add(set);
        }
        await db.SaveChangesAsync(cancellationToken);
        var tenderFormId = await db.FormDefinitions.Where(x => x.Code == "TENDER-FORM").Select(x => x.Id).SingleAsync(cancellationToken);
        var tenderDocsId = await db.DocumentRequirementSets.Where(x => x.Name == "Tender Document Requirements").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "TENDER-MANAGEMENT", cancellationToken)) db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("TENDER-MANAGEMENT", "Tender Management", "Tender and sourcing foundation covering RFI, RFQ, RFP creation, publishing, supplier notification, documents, and clarifications.", nameof(Tender), tenderWorkflowId, tenderFormId, tenderDocsId, null, BusinessProcessStatus.Published));
        if (!await db.Tenders.AnyAsync(x => x.TenderNumber == "RFP-LCA-2026-0001", cancellationToken)) db.Tenders.Add(SampleTender());
        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "TENDER-LIST", cancellationToken)) db.PageDefinitions.Add(new PageDefinition("TENDER-LIST", "Tenders", "Tender list page backed by the real tender API.", null, PageType.DataGrid, "/app/tenders", "ScrollText", @"{""entity"":""Tender"",""mode"":""Api"",""endpoint"":""/api/tenders"",""keyField"":""id""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "TENDER-DETAIL", cancellationToken)) db.PageDefinitions.Add(new PageDefinition("TENDER-DETAIL", "Tender detail", "Tender detail and clarification workspace backed by real APIs.", null, PageType.DetailPage, "/app/tenders/{id}", "ScrollText", @"{""entity"":""Tender"",""mode"":""Api"",""endpoint"":""/api/tenders/{id}"",""keyField"":""id""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "TENDER-NEW", cancellationToken)) db.PageDefinitions.Add(new PageDefinition("TENDER-NEW", "New tender", "Tender creation page backed by the real tender API.", null, PageType.Form, "/app/tenders/new", "FilePlus", @"{""entity"":""Tender"",""mode"":""Api"",""endpoint"":""/api/tenders"",""keyField"":""id""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        await db.SaveChangesAsync(cancellationToken);


        foreach (var component in ComponentLibrary())
            if (!await db.ComponentDefinitions.AnyAsync(x => x.Code == component.Code, cancellationToken)) db.ComponentDefinitions.Add(component);
        await db.SaveChangesAsync(cancellationToken);


        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "SUPPLIER-LIST", cancellationToken))
            db.PageDefinitions.Add(new PageDefinition(
                "SUPPLIER-LIST",
                "Suppliers",
                "Enterprise supplier master data composed from page, layout, component, data source, and permission metadata.",
                null,
                PageType.DataGrid,
                "/app/suppliers",
                "Users",
                @"{""entity"":""Supplier"",""mode"":""Api"",""endpoint"":""/api/suppliers"",""keyField"":""referenceNumber""}",
                null,
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

        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "SUPPLIER-REGISTRATION", cancellationToken))
            db.PageDefinitions.Add(new PageDefinition(
                "SUPPLIER-REGISTRATION",
                "Supplier registration",
                "Registration page assembled by ProcuraFlow from application, business process, dynamic form, workflow, rule, approval, document, navigation, and UI composition metadata.",
                null,
                PageType.Form,
                "/app/suppliers/register",
                "UserPlus",
                @"{""entity"":""Supplier"",""mode"":""Configuration"",""endpoint"":""/api/suppliers/registration/configuration"",""keyField"":""referenceNumber""}",
                null,
                @"{""template"":""ConfiguredRegistration"",""columns"":12,""density"":""Comfortable"",""regions"":[""main"",""sidebar""]}",
                @"[{""code"":""process"",""label"":""Supplier Registration business process"",""kind"":""Metadata"",""actionCode"":""SUPPLIER-REGISTRATION""}]",
                @"[{""code"":""back-to-suppliers"",""label"":""Back to suppliers"",""kind"":""Secondary"",""target"":""/app/suppliers""}]",
                @"[]",
                @"[]",
                @"[{""code"":""supplier-registration-composition"",""name"":""Supplier registration composition"",""componentType"":""ConfiguredRegistration"",""region"":""main"",""displayOrder"":10,""configuration"":{""componentDefinitionCode"":""CONFIGURED-REGISTRATION"",""applicationCode"":""PROCUREMENT"",""businessProcessCode"":""SUPPLIER-REGISTRATION"",""configurationEndpoint"":""/api/suppliers/registration/configuration"",""submitEndpoint"":""/api/suppliers/registration"",""referencePrefix"":""SUP-LCA"",""actor"":""supplier@demo.co.ls""}}]",
                @"[{""role"":""Supplier"",""access"":""Submit""},{""role"":""Administrator"",""access"":""Manage""}]",
                @"{""route"":""/app/suppliers/register"",""parentRoute"":""/app/suppliers"",""menuGroup"":""Procurement"",""showInNavigation"":true}",
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



        if (!await db.FinancialYears.AnyAsync(x => x.Code == "FY2026-2027", cancellationToken)) db.FinancialYears.Add(new FinancialYear("FY2026-2027", new DateTimeOffset(2026,4,1,0,0,0,TimeSpan.Zero), new DateTimeOffset(2027,3,31,23,59,59,TimeSpan.Zero)));
        foreach (var cc in new[] { new CostCentre("LCA-ICT", "ICT", "Corporate Services"), new CostCentre("LCA-FIN", "Finance", "Finance"), new CostCentre("LCA-OPS", "Operations", "Operations") }) if (!await db.CostCentres.AnyAsync(x => x.Code == cc.Code, cancellationToken)) db.CostCentres.Add(cc);
        foreach (var pc in new[] { new ProcurementCategory("ICT", "ICT Equipment"), new ProcurementCategory("CONSULT", "Consulting Services"), new ProcurementCategory("OFFICE", "Office Supplies") }) if (!await db.ProcurementCategories.AnyAsync(x => x.Code == pc.Code, cancellationToken)) db.ProcurementCategories.Add(pc);
        await db.SaveChangesAsync(cancellationToken);

        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "ANNUAL-PROCUREMENT-PLAN", cancellationToken))
        {
            var wf = new WorkflowDefinition("ANNUAL-PROCUREMENT-PLAN", "Annual Procurement Plan", nameof(AnnualProcurementPlan));
            var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Submitted","Submitted",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Procurement Officer"), new(v.Id,"ProcurementReview","Procurement Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Procurement Officer"), new(v.Id,"FinanceReview","Finance Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Finance User"), new(v.Id,"Approved","Approved",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Rejected","Rejected",WorkflowNodeKind.End,IsTerminal:true)]);
            v.Transitions.AddRange([new(v.Id,"Draft","Submit","Submit","Submitted"), new(v.Id,"Submitted","ProcurementReview","Start procurement review","ProcurementReview"), new(v.Id,"ProcurementReview","FinanceReview","Send to finance","FinanceReview"), new(v.Id,"FinanceReview","Approve","Approve","Approved"), new(v.Id,"Submitted","Reject","Reject","Rejected"), new(v.Id,"ProcurementReview","Reject","Reject","Rejected"), new(v.Id,"FinanceReview","Reject","Reject","Rejected")]);
            wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); db.WorkflowDefinitions.Add(wf); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "ANNUAL-PROCUREMENT-PLAN-FORM", cancellationToken))
        {
            var form = new FormDefinition("ANNUAL-PROCUREMENT-PLAN-FORM", "Annual Procurement Plan Form", nameof(AnnualProcurementPlan)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system"); var sct = new FormSection(v.Id,"plan","Plan details",1); sct.Fields.AddRange([new FormField(sct.Id,"planNumber","Plan number","text",1,true), new FormField(sct.Id,"title","Title","text",2,true), new FormField(sct.Id,"department","Department","text",3,true)]); v.Sections.Add(sct); form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.DocumentRequirementSets.AnyAsync(x => x.Name == "Annual Procurement Plan Documents", cancellationToken)) { var set = new DocumentRequirementSet("Annual Procurement Plan Documents", "Optional planning governance documents.", nameof(AnnualProcurementPlan)); set.Requirements.Add(new DocumentRequirement(set.Id,"Budget motivation",false,0,5,".pdf,.doc,.docx",10_485_760)); set.Requirements.Add(new DocumentRequirement(set.Id,"Supporting documents",false,0,10,".pdf,.doc,.docx,.xlsx",10_485_760)); db.DocumentRequirementSets.Add(set); await db.SaveChangesAsync(cancellationToken); }
        foreach (var rule in new[] {
            new BusinessRuleDefinition("REQ-BUDGET-EXISTS", "Requisition budget exists", nameof(Requisition), "Requisition.Items.Any()", true, "Requisition", BusinessRuleStatus.Published, "A matching budget must exist.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("REQ-BUDGET-AVAILABLE", "Requisition budget available", nameof(Requisition), "Requisition.Items.Any()", true, "Requisition", BusinessRuleStatus.Published, "Budget must be available.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("REQ-HAS-ITEMS", "Requisition has items", nameof(Requisition), "Requisition.Items.Any()", true, "Requisition", BusinessRuleStatus.Published, "Requisition must include at least one item.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("REQ-ITEMS-HAVE-ESTIMATES", "Requisition items have estimates", nameof(Requisition), "Requisition.ItemsHaveEstimates()", true, "Requisition", BusinessRuleStatus.Published, "Every item must include quantity and estimated unit price.", DateTimeOffset.UtcNow, "system")
        }) if (!await db.BusinessRuleDefinitions.AnyAsync(x => x.Code == rule.Code, cancellationToken)) db.BusinessRuleDefinitions.Add(rule);

        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "REQUISITION-APPROVAL-WORKFLOW", cancellationToken))
        {
            var wf = new WorkflowDefinition("REQUISITION-APPROVAL-WORKFLOW", "Requisition Approval Workflow", nameof(Requisition));
            var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Submitted","Submitted",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Requester"), new(v.Id,"BudgetValidation","Budget Validation",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"FinanceUser"), new(v.Id,"ManagerApproval","Manager Approval",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Manager"), new(v.Id,"ProcurementReview","Procurement Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"ProcurementOfficer"), new(v.Id,"Approved","Approved",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Rejected","Rejected",WorkflowNodeKind.End,IsTerminal:true)]);
            v.Transitions.AddRange([new(v.Id,"Draft","Submit","Submit","Submitted","REQ-HAS-ITEMS"), new(v.Id,"Submitted","ValidateBudget","Validate budget","BudgetValidation","REQ-ITEMS-HAVE-ESTIMATES"), new(v.Id,"BudgetValidation","ManagerApprove","Manager approve","ManagerApproval"), new(v.Id,"ManagerApproval","ProcurementReview","Procurement review","ProcurementReview"), new(v.Id,"ProcurementReview","Approve","Approve","Approved"), new(v.Id,"Submitted","Reject","Reject","Rejected"), new(v.Id,"BudgetValidation","Reject","Reject","Rejected"), new(v.Id,"ManagerApproval","Reject","Reject","Rejected"), new(v.Id,"ProcurementReview","Reject","Reject","Rejected")]);
            wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); db.WorkflowDefinitions.Add(wf); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.ApprovalMatrices.AnyAsync(x => x.Name == "Requisition Approval", cancellationToken)) { var m = new ApprovalMatrix("Requisition Approval", "Requester, manager, finance, procurement and approver matrix.", nameof(Requisition)); m.Steps.AddRange([new ApprovalStep(m.Id,"Requester",1), new ApprovalStep(m.Id,"Manager",2), new ApprovalStep(m.Id,"FinanceUser",3), new ApprovalStep(m.Id,"ProcurementOfficer",4), new ApprovalStep(m.Id,"Approver",5)]); db.ApprovalMatrices.Add(m); }
        if (!await db.PageDefinitions.AnyAsync(x => x.Code == "REQUISITION-LIST", cancellationToken)) db.PageDefinitions.Add(new PageDefinition("REQUISITION-LIST", "Requisitions", "Internal requisition list", null, PageType.DataGrid, "/app/requisitions", "ClipboardCheck", @"{""mode"":""Api""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        if (!await db.ApprovalMatrices.AnyAsync(x => x.Name == "Annual Procurement Plan Approval", cancellationToken)) { var m = new ApprovalMatrix("Annual Procurement Plan Approval", "Configurable approval chain for annual plans.", nameof(AnnualProcurementPlan)); m.Steps.AddRange([new ApprovalStep(m.Id,"Procurement Officer",1), new ApprovalStep(m.Id,"Finance User",2), new ApprovalStep(m.Id,"Approver",3)]); db.ApprovalMatrices.Add(m); await db.SaveChangesAsync(cancellationToken); }
        var planWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "ANNUAL-PROCUREMENT-PLAN").Select(x => x.Id).SingleAsync(cancellationToken); var planFormId = await db.FormDefinitions.Where(x => x.Code == "ANNUAL-PROCUREMENT-PLAN-FORM").Select(x => x.Id).SingleAsync(cancellationToken); var planDocId = await db.DocumentRequirementSets.Where(x => x.Name == "Annual Procurement Plan Documents").Select(x => x.Id).SingleAsync(cancellationToken); var planMatrixId = await db.ApprovalMatrices.Where(x => x.Name == "Annual Procurement Plan Approval").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "ANNUAL-PROCUREMENT-PLAN", cancellationToken)) db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("ANNUAL-PROCUREMENT-PLAN", "Annual Procurement Plan", "Configuration-driven annual planning process.", nameof(AnnualProcurementPlan), planWorkflowId, planFormId, planDocId, planMatrixId, BusinessProcessStatus.Published));
        var planningPages = new[] { ("ANNUAL-PLAN-LIST","Annual Procurement Plans List","/app/planning",PageType.DataGrid), ("ANNUAL-PLAN-DETAIL","Annual Procurement Plan Detail","/app/planning/{id}",PageType.DetailPage), ("ANNUAL-PLAN-NEW","New Annual Procurement Plan","/app/planning/new",PageType.Form), ("BUDGET-LIST","Budget List","/app/budgets",PageType.DataGrid), ("BUDGET-DETAIL","Budget Detail","/app/budgets/{id}",PageType.DetailPage), ("COST-CENTRES","Cost Centres","/app/cost-centres",PageType.DataGrid), ("PROCUREMENT-CATEGORIES","Procurement Categories","/app/procurement-categories",PageType.DataGrid) };
        foreach (var page in planningPages) if (!await db.PageDefinitions.AnyAsync(x => x.Code == page.Item1, cancellationToken)) db.PageDefinitions.Add(new PageDefinition(page.Item1, page.Item2, "Planning and budget foundation page definition composed by the UI Composition Engine.", null, page.Item4, page.Item3, "ClipboardList", @"{""mode"":""Api""}", Status: MetadataStatus.Active, CreatedBy: "system"));
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
