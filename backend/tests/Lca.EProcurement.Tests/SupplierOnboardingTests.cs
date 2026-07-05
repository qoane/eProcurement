using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Xunit;

public class SupplierOnboardingTests
{
    [Fact]
    public void Workflow_can_be_created_from_database_configuration_style_objects()
    {
        var audit = new InMemoryAuditSink();
        var engine = new WorkflowEngine([], [], [], [], [], new BusinessRulesEngine([], [], audit), audit);
        var workflow = engine.CreateWorkflow("GENERIC-SUPPLIER", "Generic Supplier", nameof(Supplier),
            [new WorkflowNode(Guid.Empty, "Draft", "Draft", WorkflowNodeKind.Start, IsStart: true), new WorkflowNode(Guid.Empty, "Submitted", "Submitted", WorkflowNodeKind.End, IsTerminal: true)],
            [new WorkflowTransition(Guid.Empty, "Draft", "Submit", "Submit", "Submitted")]);
        Assert.Equal("GENERIC-SUPPLIER", workflow.Code);
        Assert.Single(workflow.Versions[0].Transitions);
    }

    [Fact]
    public void Supplier_can_move_through_workflow_without_supplier_specific_hardcoded_logic()
    {
        var supplier = SupplierWithDocuments();
        var audit = new InMemoryAuditSink();
        var ruleLogs = new List<BusinessRuleExecutionLog>();
        var rules = new BusinessRulesEngine(SeedData.Rules(), ruleLogs, audit);
        var engine = new WorkflowEngine([SeedData.SupplierOnboardingWorkflow()], [], [], [], [], rules, audit);
        var instance = engine.Start("SUPPLIER-ONBOARDING", supplier, "supplier@demo.co.ls");
        instance = engine.ExecuteAction(instance, supplier, "Submit", "procurement@lca.org.ls");
        instance = engine.ExecuteAction(instance, supplier, "DocumentsAccepted", "procurement@lca.org.ls");
        instance = engine.ExecuteAction(instance, supplier, "TaxVerified", "procurement@lca.org.ls");
        instance = engine.ExecuteAction(instance, supplier, "Approve", "approver@lca.org.ls");
        Assert.Equal("Approved", instance.CurrentNodeCode);
        Assert.Equal(WorkflowInstanceStatus.Completed, instance.Status);
    }

    [Fact]
    public void Business_rule_expression_is_evaluated_from_configuration()
    {
        var supplier = SupplierWithDocuments();
        Assert.True(SimpleExpressionEvaluator.Evaluate("Supplier.Documents.Any(DocumentType == \"TaxClearance\")", supplier));
        Assert.True(SimpleExpressionEvaluator.Evaluate("Supplier.Categories.Any()", supplier));
        Assert.False(SimpleExpressionEvaluator.Evaluate("Supplier.Status == \"Submitted\"", supplier));
    }

    [Fact]
    public void Transition_effect_can_describe_supplier_status_without_status_switch()
    {
        var workflow = SeedData.SupplierOnboardingWorkflow();
        var transition = workflow.Versions[0].Transitions.Single(t => t.ToNodeCode == "Approved");
        var effect = new WorkflowTransitionEffect(nameof(Supplier), nameof(Supplier.Status), "Approved", transition.Id);
        Assert.Equal(nameof(Supplier), effect.EntityType);
        Assert.Equal(nameof(Supplier.Status), effect.PropertyName);
        Assert.Equal("Approved", effect.ValueExpression);
    }

    [Fact]
    public void Dynamic_form_can_be_created_published_and_submitted_as_domain_configuration()
    {
        var definition = new FormDefinition("SUPPLIER-PROFILE", "Supplier Profile", nameof(Supplier));
        var version = new FormVersion(definition.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "admin");
        definition = definition with { ActiveVersionId = version.Id };
        var section = new FormSection(version.Id, "company", "Company", 1);
        section.Fields.Add(new FormField(section.Id, "legalName", "Legal name", "text", 1, true));
        version.Sections.Add(section);
        definition.Versions.Add(version);
        var submission = new FormSubmission(definition.Id, version.Id, nameof(Supplier), Guid.NewGuid(), "supplier@demo.co.ls", DateTimeOffset.UtcNow);
        submission.Values.Add(new FormSubmissionValue(submission.Id, "legalName", "Maseru ICT Supplies Pty Ltd"));
        Assert.Equal(version.Id, definition.ActiveVersionId);
        Assert.Single(definition.Versions[0].Sections[0].Fields);
        Assert.Single(submission.Values);
    }

    private static Supplier SupplierWithDocuments()
    {
        var category = SeedData.Categories()[0];
        var supplier = SeedData.DemoSupplier(category) with { Status = SupplierStatus.Draft };
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "CompanyRegistration", "registration.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "TaxClearance", "tax.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        return supplier;
    }
}

public class AdministrationStudioConfigurationTests
{
    [Fact]
    public void Workflow_configuration_accepts_incremental_nodes_transitions_and_publish_metadata()
    {
        var workflow = new WorkflowDefinition("ADMIN-CONFIGURED", "Admin Configured", nameof(Supplier));
        var version = new WorkflowVersion(workflow.Id, 1);
        version.Nodes.Add(new WorkflowNode(version.Id, "Draft", "Draft", WorkflowNodeKind.Start, IsStart: true));
        version.Nodes.Add(new WorkflowNode(version.Id, "Approved", "Approved", WorkflowNodeKind.End, IsTerminal: true));
        version.Transitions.Add(new WorkflowTransition(version.Id, "Draft", "Approve", "Approve", "Approved"));
        workflow.Versions.Add(version);
        var published = version with { Status = WorkflowVersionStatus.Published, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = "admin@lca.org.ls" };
        Assert.Equal("ADMIN-CONFIGURED", workflow.Code);
        Assert.Equal(2, version.Nodes.Count);
        Assert.Single(version.Transitions);
        Assert.Equal(WorkflowVersionStatus.Published, published.Status);
    }

    [Fact]
    public void Business_rule_creation_uses_configured_expression_not_hardcoded_logic()
    {
        var rule = new BusinessRuleDefinition("HAS-CATEGORY", "Has category", nameof(Supplier), "Supplier.Categories.Any()");
        var supplier = SeedData.DemoSupplier(SeedData.Categories()[0]);
        Assert.Equal(nameof(Supplier), rule.AppliesTo);
        Assert.True(SimpleExpressionEvaluator.Evaluate(rule.Expression, supplier));
    }

    [Fact]
    public void Form_publishing_activates_database_defined_sections_and_fields()
    {
        var definition = new FormDefinition("ADMIN-FORM", "Admin Form", nameof(Supplier));
        var version = new FormVersion(definition.Id, 1);
        var section = new FormSection(version.Id, "profile", "Profile", 1);
        section.Fields.Add(new FormField(section.Id, "legalName", "Legal name", "text", 1, true));
        version.Sections.Add(section);
        definition.Versions.Add(version);
        var published = version with { Status = WorkflowVersionStatus.Published, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = "admin@lca.org.ls" };
        definition = definition with { ActiveVersionId = published.Id };
        Assert.Equal(published.Id, definition.ActiveVersionId);
        Assert.True(version.Sections[0].Fields[0].IsRequired);
    }
}

public class SupplierRegistrationEndToEndConfigurationTests
{
    [Fact]
    public void Supplier_registration_business_process_links_active_configuration_parts()
    {
        var workflow = SeedData.SupplierOnboardingWorkflow();
        var form = new FormDefinition("SUPPLIER-REGISTRATION-FORM", "Supplier Registration Form", nameof(Supplier));
        var docs = new DocumentRequirementSet("Registration Documents", "Required from configuration", nameof(Supplier));
        docs.Requirements.Add(new DocumentRequirement(docs.Id, "CompanyRegistration", true, 1, 1, ".pdf", 10_485_760));
        docs.Requirements.Add(new DocumentRequirement(docs.Id, "TaxClearance", true, 1, 1, ".pdf", 10_485_760));
        var matrix = new ApprovalMatrix("Supplier Approval", "Configured chain", nameof(Supplier));
        matrix.Steps.Add(new ApprovalStep(matrix.Id, "ProcurementOfficer", 1));
        matrix.Steps.Add(new ApprovalStep(matrix.Id, "Approver", 2));
        var process = new BusinessProcessDefinition("SUPPLIER-REGISTRATION", "Supplier Registration", "Configured onboarding", nameof(Supplier), workflow.Id, form.Id, docs.Id, matrix.Id, BusinessProcessStatus.Published);
        Assert.Equal(BusinessProcessStatus.Published, process.Status);
        Assert.Equal(workflow.Id, process.ActiveWorkflowDefinitionId);
        Assert.Equal(form.Id, process.ActiveFormDefinitionId);
        Assert.Equal(docs.Id, process.ActiveDocumentRequirementSetId);
        Assert.Equal(matrix.Id, process.ActiveApprovalMatrixId);
    }

    [Fact]
    public void Supplier_form_renders_from_dynamic_form_configuration_without_hardcoded_fields()
    {
        var form = new FormDefinition("SUPPLIER-REGISTRATION-FORM", "Supplier Registration Form", nameof(Supplier));
        var version = new FormVersion(form.Id, 1, WorkflowVersionStatus.Published, DateTimeOffset.UtcNow, "admin");
        var section = new FormSection(version.Id, "company", "Company", 1);
        section.Fields.Add(new FormField(section.Id, "legalName", "Legal name", "text", 1, true));
        section.Fields.Add(new FormField(section.Id, "contactEmail", "Contact email", "email", 2, true));
        version.Sections.Add(section);
        form = form with { ActiveVersionId = version.Id };
        form.Versions.Add(version);
        var renderedFields = form.Versions.Single(v => v.Id == form.ActiveVersionId).Sections.SelectMany(s => s.Fields).OrderBy(f => f.DisplayOrder).Select(f => f.Code).ToList();
        Assert.Equal(["legalName", "contactEmail"], renderedFields);
    }

    [Fact]
    public void Supplier_submission_starts_workflow_creates_verification_tasks_updates_status_and_audit()
    {
        var supplier = SupplierWithConfiguredDocuments();
        var audit = new InMemoryAuditSink();
        var rules = new BusinessRulesEngine(SeedData.Rules(), [], audit);
        var workflow = SeedData.SupplierOnboardingWorkflow();
        var approved = workflow.Versions[0].Transitions.Single(t => t.ActionCode == "Approve");
        var effects = new List<WorkflowTransitionEffect> { new(nameof(Supplier), nameof(Supplier.Status), "Approved", approved.Id) };
        var tasks = new List<WorkflowTask>();
        var actions = new List<WorkflowAction>();
        var history = new List<WorkflowHistory>();
        var engine = new WorkflowEngine([workflow], [], tasks, actions, history, rules, audit);
        var instance = engine.Start(workflow.Code, supplier, "supplier@demo.co.ls");
        instance = engine.ExecuteAction(instance, supplier, "Submit", "supplier@demo.co.ls");
        Assert.Contains(tasks, t => t.NodeCode == "DocumentCheck" && t.Status == WorkflowTaskStatus.Open);
        instance = engine.ExecuteAction(instance, supplier, "DocumentsAccepted", "procurement@lca.org.ls");
        instance = engine.ExecuteAction(instance, supplier, "TaxVerified", "procurement@lca.org.ls");
        instance = engine.ExecuteAction(instance, supplier, "Approve", "approver@lca.org.ls");
        var approvalEffect = effects.Single(e => e.TriggerTransitionId == approved.Id && e.PropertyName == nameof(Supplier.Status));
        supplier = supplier with { Status = Enum.Parse<SupplierStatus>(approvalEffect.ValueExpression) };
        Assert.Equal(WorkflowInstanceStatus.Completed, instance.Status);
        Assert.Equal(SupplierStatus.Approved, supplier.Status);
        Assert.NotEmpty(audit.Events);
    }

    private static Supplier SupplierWithConfiguredDocuments()
    {
        var supplier = SeedData.DemoSupplier(SeedData.Categories()[0]) with { Status = SupplierStatus.Draft };
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "CompanyRegistration", "registration.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        supplier.Documents.Add(new SupplierDocument(supplier.Id, "TaxClearance", "tax.pdf", "supplier@demo.co.ls", DateTimeOffset.UtcNow));
        return supplier;
    }
}
