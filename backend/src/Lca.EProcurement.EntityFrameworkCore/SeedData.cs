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

        var mainNavigation = await db.NavigationDefinitions.Include(x => x.Items).SingleOrDefaultAsync(x => x.Code == "MAIN", cancellationToken);
        if (mainNavigation is null)
        {
            mainNavigation = new NavigationDefinition("MAIN", "Main navigation", "Administrator-configured sidebar navigation for ProcuraFlow.", Status: MetadataStatus.Active, CreatedBy: "system");
            db.NavigationDefinitions.Add(mainNavigation);
            await db.SaveChangesAsync(cancellationToken);
        }

        async Task<NavigationItem> UpsertNavigationItem(string code, string label, string itemType, string? url, string icon, int displayOrder, Guid? parentId = null, bool isCollapsible = false, bool isExpandedByDefault = true, string permissionsJson = @"[]")
        {
            var item = await db.NavigationItems.SingleOrDefaultAsync(x => x.NavigationDefinitionId == mainNavigation.Id && x.Code == code, cancellationToken);
            if (item is null)
            {
                item = new NavigationItem(mainNavigation.Id, code, label, itemType, url, icon, displayOrder, parentId, isCollapsible, isExpandedByDefault, permissionsJson, IsVisible: true);
                db.NavigationItems.Add(item);
                await db.SaveChangesAsync(cancellationToken);
                return item;
            }

            db.Entry(item).CurrentValues[nameof(NavigationItem.Label)] = label;
            db.Entry(item).CurrentValues[nameof(NavigationItem.ItemType)] = itemType;
            db.Entry(item).CurrentValues[nameof(NavigationItem.Url)] = url;
            db.Entry(item).CurrentValues[nameof(NavigationItem.Icon)] = icon;
            db.Entry(item).CurrentValues[nameof(NavigationItem.DisplayOrder)] = displayOrder;
            db.Entry(item).CurrentValues[nameof(NavigationItem.ParentId)] = parentId;
            db.Entry(item).CurrentValues[nameof(NavigationItem.IsCollapsible)] = isCollapsible;
            db.Entry(item).CurrentValues[nameof(NavigationItem.IsExpandedByDefault)] = isExpandedByDefault;
            db.Entry(item).CurrentValues[nameof(NavigationItem.PermissionsJson)] = permissionsJson;
            db.Entry(item).CurrentValues[nameof(NavigationItem.VisibilityRule)] = string.Empty;
            db.Entry(item).CurrentValues[nameof(NavigationItem.IsVisible)] = true;
            await db.SaveChangesAsync(cancellationToken);
            return item;
        }

        async Task AddNavigationLink(Guid parentId, string code, string label, string url, string icon, int displayOrder, string permissionsJson = @"[]") =>
            await UpsertNavigationItem(code, label, "Link", url, icon, displayOrder, parentId, permissionsJson: permissionsJson);

        var overview = await UpsertNavigationItem("overview", "Overview", "Group", null, "LayoutDashboard", 10, isCollapsible: true);
        await AddNavigationLink(overview.Id, "dashboard", "Dashboard", "/app/dashboard", "Gauge", 10);

        var procurement = await UpsertNavigationItem("procurement", "Procurement", "Group", null, "BriefcaseBusiness", 20, isCollapsible: true);
        await AddNavigationLink(procurement.Id, "suppliers", "Suppliers", "/app/suppliers", "Users", 10, @"[""SupplierManagement.View""]");
        await AddNavigationLink(procurement.Id, "supplier-registration", "Supplier Registration", "/app/suppliers/register", "UserPlus", 20);
        await AddNavigationLink(procurement.Id, "supplier-verification", "Supplier Verification", "/app/suppliers/verification", "UserCheck", 30);
        await AddNavigationLink(procurement.Id, "annual-planning", "Annual Planning", "/app/planning", "ClipboardList", 40);
        await AddNavigationLink(procurement.Id, "budgets", "Budgets", "/app/budgets", "WalletCards", 50);
        await AddNavigationLink(procurement.Id, "cost-centres", "Cost Centres", "/app/cost-centres", "Building2", 60);
        await AddNavigationLink(procurement.Id, "procurement-categories", "Procurement Categories", "/app/procurement-categories", "Tags", 70);
        await AddNavigationLink(procurement.Id, "requisitions", "Requisitions", "/app/requisitions", "ClipboardCheck", 80);
        await AddNavigationLink(procurement.Id, "tenders", "Tenders", "/app/tenders", "ScrollText", 90, @"[""Tender.View""]");
        await AddNavigationLink(procurement.Id, "workflow-tasks", "Workflow Tasks", "/app/tasks", "ListTodo", 100);
        await AddNavigationLink(procurement.Id, "audit-explorer", "Audit Explorer", "/app/audit", "SearchCheck", 110);

        var sourcing = await UpsertNavigationItem("sourcing", "Sourcing", "Group", null, "Landmark", 30, isCollapsible: true);
        await AddNavigationLink(sourcing.Id, "tender-management", "Tender Management", "/app/tenders", "ScrollText", 10);
        await AddNavigationLink(sourcing.Id, "clarifications", "Clarifications", "/app/tenders/clarifications", "MessagesSquare", 20);
        await AddNavigationLink(sourcing.Id, "bid-submissions", "Bid Submissions", "/app/bids", "FileUp", 30);
        await AddNavigationLink(sourcing.Id, "bid-opening", "Bid Opening", "/app/bid-opening", "UnlockKeyhole", 40);
        await AddNavigationLink(sourcing.Id, "evaluation", "Evaluation", "/app/evaluation", "Scale", 50);
        await AddNavigationLink(sourcing.Id, "awards", "Awards", "/app/awards", "Award", 60);

        var manage = await UpsertNavigationItem("manage", "Manage", "Group", null, "FolderKanban", 40, isCollapsible: true);
        await AddNavigationLink(manage.Id, "purchase-orders", "Purchase Orders", "/app/purchase-orders", "ShoppingCart", 10);
        await AddNavigationLink(manage.Id, "contracts", "Contracts", "/app/contracts", "FileSignature", 20);

        var insights = await UpsertNavigationItem("insights", "Insights", "Group", null, "ChartNoAxesCombined", 50, isCollapsible: true);
        await AddNavigationLink(insights.Id, "reporting", "Reporting", "/app/reporting", "BarChart3", 10);
        await AddNavigationLink(insights.Id, "dashboards", "Dashboards", "/app/dashboards", "LayoutDashboard", 20);

        var studio = await UpsertNavigationItem("procuraflow-studio", "ProcuraFlow Studio", "Group", null, "Blocks", 60, isCollapsible: true);
        await AddNavigationLink(studio.Id, "studio-applications", "Applications", "/app/studio/applications", "AppWindow", 10);
        await AddNavigationLink(studio.Id, "pages", "Pages", "/app/studio/pages", "PanelTop", 20, @"[""Studio.Pages""]");
        await AddNavigationLink(studio.Id, "entities", "Entities", "/app/studio/entities", "Database", 30, @"[""Studio.Entities""]");
        await AddNavigationLink(studio.Id, "studio-components", "Components", "/app/studio/components", "Component", 40);
        await AddNavigationLink(studio.Id, "studio-navigation", "Navigation", "/app/studio/navigation", "Navigation", 50);
        await AddNavigationLink(studio.Id, "workflows", "Workflows", "/app/workflows/designer", "Workflow", 60, @"[""Workflow.Admin""]");
        await AddNavigationLink(studio.Id, "rules", "Business Rules", "/app/rules", "ShieldCheck", 70, @"[""Rules.Admin""]");
        await AddNavigationLink(studio.Id, "forms", "Dynamic Forms", "/app/forms", "FormInput", 80);
        await AddNavigationLink(studio.Id, "configuration", "Configuration", "/app/configuration", "Settings2", 90);

        var system = await UpsertNavigationItem("system", "System", "Group", null, "Settings", 70, isCollapsible: true);
        await AddNavigationLink(system.Id, "security", "Security", "/app/security", "LockKeyhole", 10);
        await AddNavigationLink(system.Id, "users", "Users", "/app/users", "UserCog", 20);
        await AddNavigationLink(system.Id, "roles-permissions", "Roles and Permissions", "/app/roles", "Shield", 30);
        await AddNavigationLink(system.Id, "integrations", "Integrations", "/app/integrations", "Cable", 40);
        await AddNavigationLink(system.Id, "settings", "Settings", "/app/settings", "SlidersHorizontal", 50);

        foreach (var obsoleteCode in new[] { "administration", "studio" })
        {
            var obsolete = await db.NavigationItems.SingleOrDefaultAsync(x => x.NavigationDefinitionId == mainNavigation.Id && x.Code == obsoleteCode, cancellationToken);
            if (obsolete is not null)
            {
                db.Entry(obsolete).CurrentValues[nameof(NavigationItem.IsVisible)] = false;
            }
        }
        await db.SaveChangesAsync(cancellationToken);

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

        foreach (var rule in new[] {
            new BusinessRuleDefinition("BID-TENDER-PUBLISHED", "Tender is published", nameof(BidSubmission), "BidSubmission.TenderIsPublished()", true, "Bid Submission", BusinessRuleStatus.Published, "Tender must be published.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BID-TENDER-NOT-CLOSED", "Tender has not closed", nameof(BidSubmission), "BidSubmission.TenderHasNotClosed()", true, "Bid Submission", BusinessRuleStatus.Published, "Tender must still be open.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BID-SUPPLIER-APPROVED", "Supplier is approved", nameof(BidSubmission), "BidSubmission.SupplierIsApproved()", true, "Bid Submission", BusinessRuleStatus.Published, "Supplier must be approved.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BID-DOCUMENTS-UPLOADED", "All required documents uploaded", nameof(BidSubmission), "BidSubmission.RequiredDocumentsUploaded()", true, "Bid Submission", BusinessRuleStatus.Published, "All required documents must be uploaded.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BID-DECLARATION-ACCEPTED", "Mandatory declaration accepted", nameof(BidSubmission), "BidSubmission.MandatoryDeclarationAccepted()", true, "Bid Submission", BusinessRuleStatus.Published, "Mandatory declaration must be accepted.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BID-BEFORE-CLOSING", "Submission before closing date", nameof(BidSubmission), "BidSubmission.SubmissionBeforeClosingDate()", true, "Bid Submission", BusinessRuleStatus.Published, "Submission must be before closing date.", DateTimeOffset.UtcNow, "system")
        }) if (!await db.BusinessRuleDefinitions.AnyAsync(x => x.Code == rule.Code, cancellationToken)) db.BusinessRuleDefinitions.Add(rule);
        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "BID-SUBMISSION-WORKFLOW", cancellationToken))
        {
            var wf = new WorkflowDefinition("BID-SUBMISSION-WORKFLOW", "Bid Submission Workflow", nameof(BidSubmission));
            var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"ReadyForSubmission","Ready for Submission",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Supplier"), new(v.Id,"Submitted","Submitted",WorkflowNodeKind.Automatic), new(v.Id,"Locked","Locked",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"ProcurementOfficer"), new(v.Id,"Opened","Opened",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"EvaluationCommittee"), new(v.Id,"Evaluated","Evaluated",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"EvaluationCommittee"), new(v.Id,"Awarded","Awarded",WorkflowNodeKind.End,IsTerminal:true)]);
            v.Transitions.AddRange([new(v.Id,"Draft","MarkReady","Mark ready","ReadyForSubmission"), new(v.Id,"ReadyForSubmission","Submit","Submit bid","Submitted","BID-BEFORE-CLOSING"), new(v.Id,"Submitted","Lock","Lock submission","Locked"), new(v.Id,"Locked","Open","Open bid","Opened"), new(v.Id,"Opened","Evaluate","Evaluate bid","Evaluated"), new(v.Id,"Evaluated","Award","Award bid","Awarded")]);
            wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); db.WorkflowDefinitions.Add(wf); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "BID-SUBMISSION-FORM", cancellationToken))
        {
            var form = new FormDefinition("BID-SUBMISSION-FORM", "Bid Submission Form", nameof(BidSubmission)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            var sections = new[] { "Supplier Information", "Pricing", "Technical Proposal", "Commercial Proposal", "Declarations", "Attachments" };
            for (var i = 0; i < sections.Length; i++) v.Sections.Add(new FormSection(v.Id, sections[i].Replace(" ", ""), sections[i], i + 1));
            form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.DocumentRequirementSets.AnyAsync(x => x.Name == "Bid Submission Documents", cancellationToken)) { var set = new DocumentRequirementSet("Bid Submission Documents", "Required bid package documents with future vault, encryption and digital-signature metadata.", nameof(BidSubmission)); foreach (var d in new[] { "Technical Proposal", "Financial Proposal", "Company Registration", "Tax Clearance", "Bid Security", "Confidentiality Declaration" }) set.Requirements.Add(new DocumentRequirement(set.Id,d,true,1,5,".pdf,.doc,.docx,.xlsx",25_000_000)); db.DocumentRequirementSets.Add(set); await db.SaveChangesAsync(cancellationToken); }
        if (!await db.ApprovalMatrices.AnyAsync(x => x.Name == "Bid Submission Approval", cancellationToken)) { var m = new ApprovalMatrix("Bid Submission Approval", "Configurable bid submission review roles.", nameof(BidSubmission)); m.Steps.AddRange([new ApprovalStep(m.Id,"Supplier",1), new ApprovalStep(m.Id,"ProcurementOfficer",2), new ApprovalStep(m.Id,"EvaluationCommittee",3)]); db.ApprovalMatrices.Add(m); await db.SaveChangesAsync(cancellationToken); }
        var bidWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "BID-SUBMISSION-WORKFLOW").Select(x => x.Id).SingleAsync(cancellationToken); var bidFormId = await db.FormDefinitions.Where(x => x.Code == "BID-SUBMISSION-FORM").Select(x => x.Id).SingleAsync(cancellationToken); var bidDocId = await db.DocumentRequirementSets.Where(x => x.Name == "Bid Submission Documents").Select(x => x.Id).SingleAsync(cancellationToken); var bidMatrixId = await db.ApprovalMatrices.Where(x => x.Name == "Bid Submission Approval").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "BID-SUBMISSION", cancellationToken)) db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("BID-SUBMISSION", "Bid Submission", "Configurable supplier bid submission process.", nameof(BidSubmission), bidWorkflowId, bidFormId, bidDocId, bidMatrixId, BusinessProcessStatus.Published));
        foreach (var page in new[] { ("BID-SUBMISSION-LIST","Bid Submission List","/app/bids",PageType.DataGrid), ("BID-SUBMISSION-NEW","New Bid Submission","/app/bids/new",PageType.Form), ("BID-SUBMISSION-DETAIL","Bid Submission Detail","/app/bids/{id}",PageType.DetailPage), ("BID-DOCUMENTS","Bid Documents","/app/bids/{id}/documents",PageType.DetailPage), ("BID-TIMELINE","Bid Timeline","/app/bids/{id}/timeline",PageType.Timeline), ("BID-HISTORY","Bid History","/app/bids/{id}/history",PageType.Timeline) }) if (!await db.PageDefinitions.AnyAsync(x => x.Code == page.Item1, cancellationToken)) db.PageDefinitions.Add(new PageDefinition(page.Item1, page.Item2, "Bid submission page rendered by the UI Composition Engine.", null, page.Item4, page.Item3, "FileText", @"{""mode"":""Api"",""endpoint"":""/api/bids""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        if (!await db.NavigationItems.AnyAsync(x => x.Code == "bids", cancellationToken)) { var main = await db.NavigationDefinitions.FirstAsync(cancellationToken); db.NavigationItems.Add(new NavigationItem(main.Id, "bids", "Bid Submissions", "Link", "/app/bids", "FileText", 95)); }
        await db.SaveChangesAsync(cancellationToken);


        foreach (var rule in new[] {
            new BusinessRuleDefinition("BO-TENDER-CLOSED", "Tender closing date has passed", nameof(BidOpeningSession), "BidOpening.TenderClosingDateHasPassed()", true, "Bid Opening", BusinessRuleStatus.Published, "Tender closing date must have passed.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-TENDER-NOT-CANCELLED", "Tender is not cancelled", nameof(BidOpeningSession), "BidOpening.TenderIsNotCancelled()", true, "Bid Opening", BusinessRuleStatus.Published, "Tender must not be cancelled.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-HAS-COMMITTEE", "Bid opening session has committee", nameof(BidOpeningSession), "BidOpening.SessionHasCommittee()", true, "Bid Opening", BusinessRuleStatus.Published, "Opening committee is required.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-BID-LOCKED", "Bid submission is locked", "BidOpeningSubmission", "BidOpening.BidSubmissionIsLocked()", true, "Bid Opening", BusinessRuleStatus.Published, "Bid submission must be locked.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-BID-BEFORE-CLOSING", "Bid submission was submitted before closing date", "BidOpeningSubmission", "BidOpening.SubmissionBeforeClosingDate()", true, "Bid Opening", BusinessRuleStatus.Published, "Bid submission must be before closing date.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-BID-DOCUMENTS", "Bid submission has required documents", "BidOpeningSubmission", "BidOpening.RequiredDocumentsUploaded()", true, "Bid Opening", BusinessRuleStatus.Published, "Bid submission requires documents.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-BID-DECLARATIONS", "Bid submission has mandatory declarations", "BidOpeningSubmission", "BidOpening.MandatoryDeclarationAccepted()", true, "Bid Opening", BusinessRuleStatus.Published, "Bid submission requires mandatory declarations.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("BO-AUTHORIZED-COMMITTEE", "User is authorized opening committee member", "BidOpeningSubmission", "BidOpening.UserIsCommitteeMember()", true, "Bid Opening", BusinessRuleStatus.Published, "User must be an opening committee member.", DateTimeOffset.UtcNow, "system")
        }) if (!await db.BusinessRuleDefinitions.AnyAsync(x => x.Code == rule.Code, cancellationToken)) db.BusinessRuleDefinitions.Add(rule);
        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "BID-OPENING-WORKFLOW", cancellationToken))
        {
            var wf = new WorkflowDefinition("BID-OPENING-WORKFLOW", "Bid Opening Workflow", nameof(BidOpeningSession));
            var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Scheduled","Scheduled",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"OpeningCommittee"), new(v.Id,"InProgress","In Progress",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"OpeningCommittee"), new(v.Id,"Completed","Completed",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"ProcurementOfficer"), new(v.Id,"ReferredToEvaluation","Referred to Evaluation",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Cancelled","Cancelled",WorkflowNodeKind.End,IsTerminal:true)]);
            v.Transitions.AddRange([new(v.Id,"Draft","Schedule","Schedule","Scheduled"), new(v.Id,"Scheduled","StartOpening","Start opening","InProgress"), new(v.Id,"InProgress","OpenSubmission","Open submission","InProgress"), new(v.Id,"InProgress","CompleteOpening","Complete opening","Completed"), new(v.Id,"Completed","ReferToEvaluation","Refer to evaluation","ReferredToEvaluation"), new(v.Id,"Draft","Cancel","Cancel","Cancelled"), new(v.Id,"Scheduled","Cancel","Cancel","Cancelled")]);
            wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); db.WorkflowDefinitions.Add(wf); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "BID-OPENING-FORM", cancellationToken)) { var form = new FormDefinition("BID-OPENING-FORM", "Bid Opening Session Form", nameof(BidOpeningSession)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system"); var sections = new[] { "Session Details", "Committee", "Tender Information", "Opening Checklist", "Notes" }; for (var i=0;i<sections.Length;i++) v.Sections.Add(new FormSection(v.Id, sections[i].Replace(" ", ""), sections[i], i+1)); form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); await db.SaveChangesAsync(cancellationToken); }
        if (!await db.DocumentRequirementSets.AnyAsync(x => x.Name == "Opening Checklist Documents", cancellationToken)) { var set = new DocumentRequirementSet("Opening Checklist Documents", "Document requirements for bid opening checklist evidence.", nameof(BidOpeningSession)); set.Requirements.Add(new DocumentRequirement(set.Id,"Opening Minutes",true,1,1,".pdf,.docx",10_000_000)); db.DocumentRequirementSets.Add(set); await db.SaveChangesAsync(cancellationToken); }
        var boWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "BID-OPENING-WORKFLOW").Select(x => x.Id).SingleAsync(cancellationToken); var boFormId = await db.FormDefinitions.Where(x => x.Code == "BID-OPENING-FORM").Select(x => x.Id).SingleAsync(cancellationToken); var boDocsId = await db.DocumentRequirementSets.Where(x => x.Name == "Opening Checklist Documents").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "BID-OPENING", cancellationToken)) db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("BID-OPENING", "Bid Opening", "Controlled bid opening assembled from platform workflow, rules, forms, documents, audit and notification capabilities.", nameof(BidOpeningSession), boWorkflowId, boFormId, boDocsId, null, BusinessProcessStatus.Published));
        foreach (var page in new[] { ("BID-OPENING-LIST","Bid Opening List","/app/bid-opening",PageType.DataGrid), ("BID-OPENING-DETAIL","Bid Opening Detail","/app/bid-opening/{id}",PageType.DetailPage), ("BID-OPENING-NEW","New Bid Opening Session","/app/bid-opening/new",PageType.Form) }) if (!await db.PageDefinitions.AnyAsync(x => x.Code == page.Item1, cancellationToken)) db.PageDefinitions.Add(new PageDefinition(page.Item1, page.Item2, "Bid opening metadata page composed by the UI Composition Engine.", null, page.Item4, page.Item3, "UnlockKeyhole", @"{""mode"":""Api"",""endpoint"":""/api/bid-opening""}", Status: MetadataStatus.Active, CreatedBy: "system"));

        if (!await db.Suppliers.AnyAsync(x => x.ReferenceNumber == "SUP-BID-2026-0001", cancellationToken)) db.Suppliers.Add(new Supplier("SUP-BID-2026-0001", "Maseru Digital Bidders", SupplierStatus.Approved));
        if (!await db.Tenders.AnyAsync(x => x.TenderNumber == "RFP-LCA-2026-BID-001", cancellationToken))
        {
            var tender = new Tender("RFP-LCA-2026-BID-001", "Electronic bid submission pilot", "Published tender for bid submission demo.", TenderType.RFP, "Open Tender", TenderStatus.Published, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30), "procurement@lca.org.ls", DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(-1), "procurement@lca.org.ls");
            tender.Lots.Add(new TenderLot(tender.Id, "LOT-1", "Bid platform configuration", "Configure bid submission capability."));
            db.Tenders.Add(tender);
        }
        await db.SaveChangesAsync(cancellationToken);
        if (!await db.BidSubmissions.AnyAsync(x => x.SubmissionNumber == "BID-SUB-2026-0001", cancellationToken))
        {
            var supplier = await db.Suppliers.SingleAsync(x => x.ReferenceNumber == "SUP-BID-2026-0001", cancellationToken);
            var tender = await db.Tenders.Include(x => x.Lots).SingleAsync(x => x.TenderNumber == "RFP-LCA-2026-BID-001", cancellationToken);
            var bid = new BidSubmission("BID-SUB-2026-0001", tender.Id, supplier.Id, BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, "supplier@demo.ls");
            bid.Items.Add(new BidSubmissionItem(bid.Id, tender.Lots.FirstOrDefault()?.Id, "Configuration and support", 1, 250000, 250000, "Demo draft bid"));
            bid.Declarations.Add(new BidSubmissionDeclaration(bid.Id, "Confidentiality Declaration", true, "supplier@demo.ls", DateTimeOffset.UtcNow));
            bid.Versions.Add(new BidSubmissionVersion(bid.Id, 1, DateTimeOffset.UtcNow, "supplier@demo.ls"));
            bid.History.Add(new BidSubmissionHistory(bid.Id, "Submission created", "system", "Seed draft bid submission.", DateTimeOffset.UtcNow));
            db.BidSubmissions.Add(bid);
        }


        foreach (var rule in new[] {
            new BusinessRuleDefinition("EV-BID-OPENING-COMPLETED", "Bid opening completed", nameof(EvaluationSession), "Evaluation.BidOpeningCompleted()", true, "Evaluation", BusinessRuleStatus.Published, "Bid opening must be completed.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-BID-SUBMISSION-OPENED", "Bid submission opened", nameof(EvaluationSession), "Evaluation.BidSubmissionOpened()", true, "Evaluation", BusinessRuleStatus.Published, "Bid submissions must be opened.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-HAS-COMMITTEE", "Evaluation committee exists", nameof(EvaluationSession), "Evaluation.HasCommittee()", true, "Evaluation", BusinessRuleStatus.Published, "Evaluation committee is required.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-DECLARATIONS-ACCEPTED", "Evaluators accepted conflict declarations", nameof(EvaluationSession), "Evaluation.DeclarationsAccepted()", true, "Evaluation", BusinessRuleStatus.Published, "All evaluators must accept declarations.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-TECHNICAL-THRESHOLD", "Technical score meets threshold", nameof(EvaluationSession), "Evaluation.TechnicalThresholdMet()", true, "Evaluation", BusinessRuleStatus.Published, "Technical score must meet threshold.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-FINANCIAL-COMPLETED", "Financial score completed", nameof(EvaluationSession), "Evaluation.FinancialCompleted()", true, "Evaluation", BusinessRuleStatus.Published, "Financial score must be completed.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-CONSENSUS-COMPLETED", "Consensus completed", nameof(EvaluationSession), "Evaluation.ConsensusCompleted()", true, "Evaluation", BusinessRuleStatus.Published, "Consensus must be completed.", DateTimeOffset.UtcNow, "system"),
            new BusinessRuleDefinition("EV-RECOMMENDATION-EXISTS", "Recommendation exists before completion", nameof(EvaluationSession), "Evaluation.RecommendationExists()", true, "Evaluation", BusinessRuleStatus.Published, "Recommendation is required before completion.", DateTimeOffset.UtcNow, "system")
        }) if (!await db.BusinessRuleDefinitions.AnyAsync(x => x.Code == rule.Code, cancellationToken)) db.BusinessRuleDefinitions.Add(rule);
        if (!await db.WorkflowDefinitions.AnyAsync(x => x.Code == "EVALUATION-WORKFLOW", cancellationToken))
        {
            var wf = new WorkflowDefinition("EVALUATION-WORKFLOW", "Evaluation Workflow", nameof(EvaluationSession));
            var v = new WorkflowVersion(wf.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system");
            v.Nodes.AddRange([new(v.Id,"Draft","Draft",WorkflowNodeKind.Start,IsStart:true), new(v.Id,"Scheduled","Scheduled",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"EvaluationCommittee"), new(v.Id,"AdministrativeReview","Administrative Review",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"EvaluationCommittee"), new(v.Id,"TechnicalEvaluation","Technical Evaluation",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Evaluator"), new(v.Id,"FinancialEvaluation","Financial Evaluation",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Evaluator"), new(v.Id,"Consensus","Consensus",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Chairperson"), new(v.Id,"Recommendation","Recommendation",WorkflowNodeKind.Task,CreatesTask:true,DefaultAssignedRole:"Chairperson"), new(v.Id,"Completed","Completed",WorkflowNodeKind.Task), new(v.Id,"ReferredToAward","Referred to Award",WorkflowNodeKind.End,IsTerminal:true), new(v.Id,"Cancelled","Cancelled",WorkflowNodeKind.End,IsTerminal:true)]);
            v.Transitions.AddRange([new(v.Id,"Draft","Schedule","Schedule","Scheduled"), new(v.Id,"Scheduled","StartAdministrative","Start administrative","AdministrativeReview"), new(v.Id,"AdministrativeReview","PassAdministrative","Pass administrative","TechnicalEvaluation"), new(v.Id,"TechnicalEvaluation","CompleteTechnical","Complete technical","FinancialEvaluation"), new(v.Id,"FinancialEvaluation","CompleteFinancial","Complete financial","Consensus"), new(v.Id,"Consensus","RecordRecommendation","Record recommendation","Recommendation"), new(v.Id,"Recommendation","CompleteEvaluation","Complete evaluation","Completed"), new(v.Id,"Completed","ReferToAward","Refer to award","ReferredToAward"), new(v.Id,"Draft","Cancel","Cancel","Cancelled"), new(v.Id,"Scheduled","Cancel","Cancel","Cancelled")]);
            wf = wf with { PublishedVersionId = v.Id }; wf.Versions.Add(v); db.WorkflowDefinitions.Add(wf); await db.SaveChangesAsync(cancellationToken);
        }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "EVALUATION-SESSION-FORM", cancellationToken)) { var form = new FormDefinition("EVALUATION-SESSION-FORM", "Evaluation Session Form", nameof(EvaluationSession)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system"); foreach (var name in new[] { "Session Details", "Tender Information", "Committee", "Notes" }.Select((x,i)=>(x,i))) v.Sections.Add(new FormSection(v.Id, name.x.Replace(" ", ""), name.x, name.i+1)); form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "CONFLICT-DECLARATION-FORM", cancellationToken)) { var form = new FormDefinition("CONFLICT-DECLARATION-FORM", "Conflict of Interest Declaration Form", nameof(EvaluationDeclaration)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system"); foreach (var name in new[] { "Declaration", "Acceptance", "Signature Placeholder" }.Select((x,i)=>(x,i))) v.Sections.Add(new FormSection(v.Id, name.x.Replace(" ", ""), name.x, name.i+1)); form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); }
        if (!await db.FormDefinitions.AnyAsync(x => x.Code == "EVALUATION-SCORING-FORM", cancellationToken)) { var form = new FormDefinition("EVALUATION-SCORING-FORM", "Evaluation Scoring Form", nameof(EvaluationScore)); var v = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "system"); foreach (var name in new[] { "Administrative Review", "Technical Scoring", "Financial Scoring", "Comments" }.Select((x,i)=>(x,i))) v.Sections.Add(new FormSection(v.Id, name.x.Replace(" ", ""), name.x, name.i+1)); form = form with { ActiveVersionId = v.Id }; form.Versions.Add(v); db.FormDefinitions.Add(form); }
        await db.SaveChangesAsync(cancellationToken);
        var evWorkflowId = await db.WorkflowDefinitions.Where(x => x.Code == "EVALUATION-WORKFLOW").Select(x => x.Id).SingleAsync(cancellationToken); var evFormId = await db.FormDefinitions.Where(x => x.Code == "EVALUATION-SESSION-FORM").Select(x => x.Id).SingleAsync(cancellationToken);
        if (!await db.BusinessProcessDefinitions.AnyAsync(x => x.Code == "EVALUATION-MANAGEMENT", cancellationToken)) db.BusinessProcessDefinitions.Add(new BusinessProcessDefinition("EVALUATION-MANAGEMENT", "Evaluation Management", "Configurable tender evaluation assembled from platform workflow, rules, dynamic forms, audit and notification capabilities.", nameof(EvaluationSession), evWorkflowId, evFormId, null, null, BusinessProcessStatus.Published));
        foreach (var page in new[] { ("EVALUATION-LIST","Evaluation List","/app/evaluation",PageType.DataGrid), ("EVALUATION-DETAIL","Evaluation Detail","/app/evaluation/{id}",PageType.DetailPage), ("EVALUATION-NEW","New Evaluation Session","/app/evaluation/new",PageType.Form), ("EVALUATION-TEMPLATE-LIST","Evaluation Template List","/app/evaluation/templates",PageType.DataGrid), ("EVALUATION-TEMPLATE-DETAIL","Evaluation Template Detail","/app/evaluation/templates/{id}",PageType.DetailPage) }) if (!await db.PageDefinitions.AnyAsync(x => x.Code == page.Item1, cancellationToken)) db.PageDefinitions.Add(new PageDefinition(page.Item1, page.Item2, "Evaluation metadata page composed by the UI Composition Engine.", null, page.Item4, page.Item3, "ClipboardCheck", @"{""mode"":""Api"",""endpoint"":""/api/evaluations""}", Status: MetadataStatus.Active, CreatedBy: "system"));
        if (!await db.NavigationItems.AnyAsync(x => x.Code == "evaluation", cancellationToken)) { var main = await db.NavigationDefinitions.FirstAsync(cancellationToken); db.NavigationItems.Add(new NavigationItem(main.Id, "evaluation", "Evaluation", "Link", "/app/evaluation", "ClipboardCheck", 105)); }
        await db.SaveChangesAsync(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
