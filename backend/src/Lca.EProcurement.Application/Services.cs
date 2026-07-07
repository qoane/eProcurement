using System.Text.Json;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Application;

public interface IConfigurationStudioApplicationService
{
    Task<ConfigurationStudioDto> GetStudioAsync(CancellationToken ct = default);
    Task<BusinessProcessDefinition> CreateBusinessProcessAsync(BusinessProcessDto dto, CancellationToken ct = default);
    Task<BusinessProcessDefinition?> PublishBusinessProcessAsync(string code, CancellationToken ct = default);
    Task<DocumentRequirementSet> CreateDocumentRequirementSetAsync(DocumentRequirementSetDto dto, CancellationToken ct = default);
    Task<ApprovalMatrix> CreateApprovalMatrixAsync(ApprovalMatrixDto dto, CancellationToken ct = default);
}
public sealed record BusinessProcessDto(string Code, string Name, string Description, string EntityType, Guid? ActiveWorkflowDefinitionId, Guid? ActiveFormDefinitionId, Guid? ActiveDocumentRequirementSetId, Guid? ActiveApprovalMatrixId, BusinessProcessStatus Status = BusinessProcessStatus.Draft);
public sealed record DocumentRequirementSetDto(string Name, string Description, string EntityType, List<DocumentRequirementDto> Requirements);
public sealed record DocumentRequirementDto(string DocumentType, bool Required, int MinimumFiles, int MaximumFiles, string AllowedExtensions, long MaximumFileSize, string? RuleCode = null);
public sealed record ApprovalMatrixDto(string Name, string Description, string EntityType, List<ApprovalStepDto> Steps);
public sealed record ApprovalStepDto(string Role, int Sequence, decimal? MinimumAmount = null, decimal? MaximumAmount = null, string? RuleCode = null);
public sealed record ConfigurationStudioDto(List<BusinessProcessDefinition> BusinessProcesses, List<DocumentRequirementSet> DocumentRequirementSets, List<ApprovalMatrix> ApprovalMatrices, List<WorkflowMapping> WorkflowMappings);

public interface IWorkflowApplicationService
{
    Task<List<WorkflowDefinition>> GetDefinitionsAsync(CancellationToken ct = default);
    Task<WorkflowDefinition> CreateWorkflowAsync(CreateWorkflowDto request, CancellationToken ct = default);
    Task<WorkflowNode?> AddNodeAsync(string workflowCode, WorkflowNodeDto dto, CancellationToken ct = default);
    Task<WorkflowTransition?> AddTransitionAsync(string workflowCode, WorkflowTransitionDto dto, CancellationToken ct = default);
    Task<WorkflowDefinition?> PublishAsync(string code, string actor, CancellationToken ct = default);
    Task<WorkflowDefinition?> ArchiveAsync(string code, int versionNumber, string actor, CancellationToken ct = default);
    Task<WorkflowDefinition> SaveDesignerAsync(WorkflowDesignerDto dto, CancellationToken ct = default);
    Task<PlatformDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<WorkflowInstance> StartAsync(string workflowCode, string entityType, Guid entityId, string actor, CancellationToken ct = default);
    Task<WorkflowInstance?> ExecuteActionAsync(Guid instanceId, string actionCode, string actor, CancellationToken ct = default);
    Task<List<WorkflowTask>> GetTasksAsync(CancellationToken ct = default);
    Task<WorkflowTask?> AssignTaskAsync(Guid id, string assignedTo, string actor, CancellationToken ct = default);
    Task<WorkflowTask?> CompleteTaskAsync(Guid id, string actor, CancellationToken ct = default);
}
public interface IBusinessRuleApplicationService
{
    Task<List<BusinessRuleDefinition>> GetRulesAsync(CancellationToken ct = default);
    Task<List<BusinessRuleExecutionLog>> GetHistoryAsync(string? ruleCode = null, CancellationToken ct = default);
    Task<BusinessRuleDefinition> CreateRuleAsync(CreateBusinessRuleDto dto, CancellationToken ct = default);
    Task<BusinessRuleDefinition?> PublishAsync(string code, string actor, CancellationToken ct = default);
    Task<RuleValidationResultDto> ValidateAsync(RuleExpressionDto dto, CancellationToken ct = default);
    Task<RuleSimulationResultDto> SimulateAsync(RuleSimulationDto dto, CancellationToken ct = default);
    Task<RuleResult> EvaluateAsync(string ruleCode, string entityType, Guid entityId, string actor, CancellationToken ct = default);
    Task<List<RuleResult>> EvaluatePublishedAsync(string appliesTo, string entityType, Guid entityId, string actor, Dictionary<string, string?>? values = null, CancellationToken ct = default);
    RuleDesignerMetadataDto GetDesignerMetadata(string appliesTo = nameof(Supplier));
}
public interface ISupplierApplicationService
{
    Task<List<object>> GetSuppliersAsync(CancellationToken ct = default);
    Task<SupplierRegistrationConfigurationDto?> GetRegistrationConfigurationAsync(CancellationToken ct = default);
    Task<SupplierDetailDto?> GetSupplierDetailAsync(string referenceNumber, CancellationToken ct = default);
    Task<SupplierRegistrationResultDto?> RegisterAsync(RegisterSupplierDto dto, CancellationToken ct = default);
    Task<WorkflowTaskDetailDto?> GetTaskDetailAsync(Guid taskId, CancellationToken ct = default);
    Task<WorkflowActionResultDto?> ExecuteTaskActionAsync(Guid taskId, ExecuteWorkflowTaskActionDto dto, CancellationToken ct = default);
    Task<WorkflowInstance?> SubmitAsync(string referenceNumber, string actor, CancellationToken ct = default);
}
public interface IDynamicFormApplicationService { Task<List<FormDefinition>> GetDefinitionsAsync(CancellationToken ct = default); Task<FormDefinition> CreateDefinitionAsync(CreateFormDefinitionDto dto, CancellationToken ct = default); Task<FormSection?> AddSectionAsync(string formCode, FormSectionDto dto, CancellationToken ct = default); Task<FormField?> AddFieldAsync(string formCode, string sectionCode, FormFieldDto dto, CancellationToken ct = default); Task<FormDefinition?> PublishVersionAsync(string code, string actor, CancellationToken ct = default); Task<FormDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default); Task<FormSubmission> SubmitAsync(SubmitFormDto dto, CancellationToken ct = default); Task<List<FormSubmission>> GetSubmissionsAsync(string entityType, Guid entityId, CancellationToken ct = default); }
public interface IAuditApplicationService { Task<List<AuditEvent>> GetEventsAsync(CancellationToken ct = default); }
public interface IPlatformConfigurationApplicationService
{
    Task<List<WorkflowMapping>> GetWorkflowMappingsAsync(CancellationToken ct = default);
    Task<WorkflowMapping> CreateWorkflowMappingAsync(WorkflowMappingDto dto, CancellationToken ct = default);
    Task<List<WorkflowTransitionEffect>> GetTransitionEffectsAsync(CancellationToken ct = default);
    Task<WorkflowTransitionEffect> CreateTransitionEffectAsync(CreateTransitionEffectDto dto, CancellationToken ct = default);
    Task<List<DocumentTypeRequirement>> GetDocumentTypeRequirementsAsync(CancellationToken ct = default);
    Task<DocumentTypeRequirement> CreateDocumentTypeRequirementAsync(DocumentTypeRequirementDto dto, CancellationToken ct = default);
    Task<List<LookupValue>> GetLookupValuesAsync(string? lookupType = null, CancellationToken ct = default);
    Task<LookupValue> CreateLookupValueAsync(LookupValueDto dto, CancellationToken ct = default);
    Task<SupplierCategory> CreateSupplierCategoryAsync(SupplierCategoryDto dto, CancellationToken ct = default);
}

public sealed record CreateWorkflowDto(string Code, string Name, string EntityType, List<WorkflowNodeDto> Nodes, List<WorkflowTransitionDto> Transitions, List<WorkflowTransitionEffectDto>? Effects = null);
public sealed record WorkflowNodeDto(string Code, string Name, WorkflowNodeKind Kind, bool CreatesTask = false, string? DefaultAssignedRole = null, bool IsStart = false, bool IsTerminal = false, int PositionX = 0, int PositionY = 0, string ActionConfigurationJson = "{}", string ConditionConfigurationJson = "{}", string BusinessRuleCodesJson = "[]", string AssignedRolesJson = "[]");
public sealed record WorkflowTransitionDto(string FromNodeCode, string ActionCode, string ActionName, string ToNodeCode, string? RequiredRuleCode = null, string ConditionExpression = "", string ActionConfigurationJson = "{}", string BusinessRuleCodesJson = "[]", string AssignedRolesJson = "[]");
public sealed record WorkflowDesignerDto(string Code, string Name, string EntityType, int? VersionNumber, List<WorkflowNodeDto> Nodes, List<WorkflowTransitionDto> Transitions, List<WorkflowTransitionEffectDto>? Effects = null);
public sealed record WorkflowTransitionEffectDto(string EntityType, string PropertyName, string ValueExpression, string TriggerActionCode, string TriggerFromNodeCode);
public sealed record CreateFormDefinitionDto(string Code, string Name, string EntityType, List<FormSectionDto> Sections);
public sealed record FormSectionDto(string Code, string Title, int DisplayOrder, List<FormFieldDto> Fields);
public sealed record FormFieldValidationDto(string ValidationType, string? ConfigurationJson, string Message);
public sealed record FormFieldVisibilityRuleDto(string Expression);
public sealed record FormFieldDto(string Code, string Label, string FieldType, int DisplayOrder, bool IsRequired, List<FormFieldValidationDto>? Validations = null, List<FormFieldVisibilityRuleDto>? VisibilityRules = null);
public sealed record SubmitFormDto(string FormCode, string EntityType, Guid EntityId, string SubmittedBy, Dictionary<string, string?> Values);
public sealed record CreateBusinessRuleDto(string Code, string Name, string AppliesTo, string Expression, bool IsActive = true, string Category = "General", string FailureMessage = "Rule failed");
public sealed record RuleExpressionDto(string AppliesTo, string Expression);
public sealed record RuleValidationResultDto(bool IsValid, List<string> Errors, List<string> Warnings);
public sealed record RuleSimulationDto(string AppliesTo, string Expression, Dictionary<string, string?> Values, List<SupplierDocumentUploadDto>? Documents = null, List<string>? Categories = null);
public sealed record RuleSimulationResultDto(bool Passed, RuleValidationResultDto Validation, Dictionary<string, object?> Trace);
public sealed record RuleDesignerMetadataDto(List<string> Categories, List<string> Fields, List<string> Functions, List<string> Operators);

public sealed record PlatformDashboardDto(int WorkflowsCount, int RulesCount, int FormsCount, int ActiveWorkflowInstances, int PendingTasks, int BusinessProcessesCount = 0, int ApprovalMatricesCount = 0);
public sealed record WorkflowMappingDto(string EntityType, string ActionCode, string WorkflowCode, bool IsActive = true);
public sealed record CreateTransitionEffectDto(Guid TriggerTransitionId, string EntityType, string PropertyName, string ValueExpression);
public sealed record DocumentTypeRequirementDto(string EntityType, string DocumentType, string Name, bool IsRequired = true);
public sealed record LookupValueDto(string LookupType, string Code, string Name, int DisplayOrder = 0, bool IsActive = true);
public sealed record SupplierCategoryDto(string Name);
public sealed record SupplierDocumentUploadDto(string DocumentType, string FileName);
public sealed record RegisterSupplierDto(string ReferenceNumber, string Actor, Dictionary<string, string?> Values, List<SupplierDocumentUploadDto> Documents);
public sealed record SupplierRegistrationConfigurationDto(BusinessProcessDefinition Process, FormDefinition Form, DocumentRequirementSet DocumentRequirements, ApprovalMatrix? ApprovalMatrix, WorkflowDefinition Workflow);
public sealed record SupplierRegistrationResultDto(string ReferenceNumber, Guid SupplierId, Guid FormSubmissionId, Guid WorkflowInstanceId, string CurrentNodeCode, string Status);
public sealed record SupplierDetailDto(Supplier Supplier, WorkflowInstance? WorkflowInstance, string? ActiveWorkflowStage, List<SupplierDocument> Documents, List<FormSubmission> FormSubmissions, List<AuditEvent> AuditTimeline, List<WorkflowTransition> AvailableActions);
public sealed record WorkflowTaskSummaryDto(Guid Id, Guid WorkflowInstanceId, string NodeCode, string? AssignedRole, string? AssignedTo, string Status, DateTimeOffset CreatedAt, string? EntityType, Guid? EntityId, string? EntityReference, string? EntityName);
public sealed record WorkflowTaskDetailDto(WorkflowTask Task, SupplierDetailDto? Supplier, List<WorkflowHistory> History, List<WorkflowAction> Actions, List<WorkflowTransition> AvailableActions);
public sealed record ExecuteWorkflowTaskActionDto(string ActionCode, string Actor);
public sealed record WorkflowActionResultDto(WorkflowInstance Instance, WorkflowTaskDetailDto? NextTask);


public interface INavigationApplicationService
{
    Task<NavigationDesignerDto> GetAsync(string code = "MAIN", CancellationToken ct = default);
    Task<NavigationDesignerDto> SaveAsync(NavigationDesignerDto dto, CancellationToken ct = default);
}
public sealed record NavigationDesignerDto(string Code, string Name, string Description, List<NavigationItemDto> Items);
public sealed record NavigationItemDto(Guid? Id, string Code, string Label, string ItemType, string? Url, string Icon, int DisplayOrder, Guid? ParentId, bool IsCollapsible, bool IsExpandedByDefault, string PermissionsJson, string VisibilityRule, bool IsVisible, List<NavigationItemDto> Children);

public sealed class NavigationApplicationService(EProcurementDbContext db) : INavigationApplicationService
{
    public async Task<NavigationDesignerDto> GetAsync(string code = "MAIN", CancellationToken ct = default)
    {
        var definition = await db.NavigationDefinitions.AsNoTracking().Include(x => x.Items).SingleOrDefaultAsync(x => x.Code == code, ct);
        if (definition is null) return new("MAIN", "Main navigation", "Configurable application sidebar navigation.", []);
        return new(definition.Code, definition.Name, definition.Description, BuildTree(definition.Items));
    }

    public async Task<NavigationDesignerDto> SaveAsync(NavigationDesignerDto dto, CancellationToken ct = default)
    {
        var definition = await db.NavigationDefinitions.Include(x => x.Items).SingleOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (definition is null)
        {
            definition = new NavigationDefinition(dto.Code, dto.Name, dto.Description, Status: MetadataStatus.Active, CreatedBy: "admin");
            db.NavigationDefinitions.Add(definition);
            await db.SaveChangesAsync(ct);
        }
        else
        {
            db.Entry(definition).CurrentValues[nameof(NavigationDefinition.Name)] = dto.Name;
            db.Entry(definition).CurrentValues[nameof(NavigationDefinition.Description)] = dto.Description;
            db.Entry(definition).CurrentValues[nameof(NavigationDefinition.Status)] = MetadataStatus.Active;
            db.Entry(definition).CurrentValues[nameof(NavigationDefinition.Modified)] = DateTimeOffset.UtcNow;
            db.Entry(definition).CurrentValues[nameof(NavigationDefinition.ModifiedBy)] = "admin";
            db.NavigationItems.RemoveRange(definition.Items);
            await db.SaveChangesAsync(ct);
        }
        AddItems(definition.Id, dto.Items, null);
        await db.SaveChangesAsync(ct);
        return await GetAsync(dto.Code, ct);
    }

    void AddItems(Guid definitionId, IEnumerable<NavigationItemDto> items, Guid? parentId)
    {
        foreach (var dto in items.OrderBy(x => x.DisplayOrder))
        {
            var item = new NavigationItem(definitionId, dto.Code, dto.Label, dto.ItemType, dto.Url, dto.Icon, dto.DisplayOrder, parentId, dto.IsCollapsible, dto.IsExpandedByDefault, dto.PermissionsJson, dto.VisibilityRule, dto.IsVisible);
            db.NavigationItems.Add(item);
            AddItems(definitionId, dto.Children, item.Id);
        }
    }

    static List<NavigationItemDto> BuildTree(IEnumerable<NavigationItem> items)
    {
        var all = items.Select(x => ToDto(x, [])).ToDictionary(x => x.Id!.Value);
        foreach (var item in items.Where(x => x.ParentId is not null).OrderBy(x => x.DisplayOrder))
            if (all.TryGetValue(item.ParentId!.Value, out var parent)) parent.Children.Add(all[item.Id]);
        return items.Where(x => x.ParentId is null).OrderBy(x => x.DisplayOrder).Select(x => all[x.Id]).ToList();
    }
    static NavigationItemDto ToDto(NavigationItem x, List<NavigationItemDto> children) => new(x.Id, x.Code, x.Label, x.ItemType, x.Url, x.Icon, x.DisplayOrder, x.ParentId, x.IsCollapsible, x.IsExpandedByDefault, x.PermissionsJson, x.VisibilityRule, x.IsVisible, children);
}

public sealed class BusinessRuleApplicationService(EProcurementDbContext db) : IBusinessRuleApplicationService
{
    static readonly string[] Functions = ["HasDocument(\"TaxClearance\")", "InCategory(\"ICT\")", "Field(\"legalName\")", "IsNotEmpty(Field(\"legalName\"))", "Equals(Field(\"country\"), \"Lesotho\")", "Contains(Field(\"email\"), \"@\")"];
    static readonly string[] Fields = ["Supplier.ReferenceNumber", "Supplier.LegalName", "Supplier.Status", "Supplier.Documents", "Supplier.Categories", "legalName", "tradingName", "email", "phone", "country"];
    public Task<List<BusinessRuleDefinition>> GetRulesAsync(CancellationToken ct = default) => db.BusinessRuleDefinitions.AsNoTracking().OrderBy(r => r.Category).ThenBy(r => r.Code).ToListAsync(ct);
    public Task<List<BusinessRuleExecutionLog>> GetHistoryAsync(string? ruleCode = null, CancellationToken ct = default) => db.BusinessRuleExecutionLogs.AsNoTracking().Where(x => ruleCode == null || x.RuleCode == ruleCode).OrderByDescending(x => x.ExecutedAt).Take(100).ToListAsync(ct);
    public async Task<BusinessRuleDefinition> CreateRuleAsync(CreateBusinessRuleDto dto, CancellationToken ct = default)
    {
        var validation = SimpleExpressionEvaluator.Validate(dto.Expression);
        if (!validation.IsValid) throw new InvalidOperationException(string.Join("; ", validation.Errors));
        var existing = await db.BusinessRuleDefinitions.SingleOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (existing is not null) db.BusinessRuleDefinitions.Remove(existing);
        var rule = new BusinessRuleDefinition(dto.Code, dto.Name, dto.AppliesTo, dto.Expression, dto.IsActive, dto.Category, BusinessRuleStatus.Draft, dto.FailureMessage);
        db.BusinessRuleDefinitions.Add(rule); await db.SaveChangesAsync(ct); return rule;
    }
    public async Task<BusinessRuleDefinition?> PublishAsync(string code, string actor, CancellationToken ct = default)
    {
        var rule = await db.BusinessRuleDefinitions.SingleOrDefaultAsync(r => r.Code == code && r.IsActive, ct); if (rule is null) return null;
        var validation = SimpleExpressionEvaluator.Validate(rule.Expression); if (!validation.IsValid) throw new InvalidOperationException(string.Join("; ", validation.Errors));
        db.Entry(rule).CurrentValues[nameof(BusinessRuleDefinition.Status)] = BusinessRuleStatus.Published;
        db.Entry(rule).CurrentValues[nameof(BusinessRuleDefinition.PublishedAt)] = DateTimeOffset.UtcNow;
        db.Entry(rule).CurrentValues[nameof(BusinessRuleDefinition.PublishedBy)] = actor;
        await db.SaveChangesAsync(ct); return rule with { Status = BusinessRuleStatus.Published, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = actor };
    }
    public Task<RuleValidationResultDto> ValidateAsync(RuleExpressionDto dto, CancellationToken ct = default) => Task.FromResult(SimpleExpressionEvaluator.Validate(dto.Expression));
    public Task<RuleSimulationResultDto> SimulateAsync(RuleSimulationDto dto, CancellationToken ct = default)
    {
        var validation = SimpleExpressionEvaluator.Validate(dto.Expression);
        var supplier = new Supplier("SIM-001", dto.Values.GetValueOrDefault("legalName") ?? "Simulation Supplier", SupplierStatus.Draft);
        supplier.Documents.AddRange((dto.Documents ?? []).Select(d => new SupplierDocument(supplier.Id, d.DocumentType, d.FileName, "simulation", DateTimeOffset.UtcNow)));
        supplier.Categories.AddRange((dto.Categories ?? []).Select(c => new SupplierCategory(c)));
        var context = new RuleEvaluationContext(supplier, dto.Values);
        var passed = validation.IsValid && SimpleExpressionEvaluator.Evaluate(dto.Expression, context);
        return Task.FromResult(new RuleSimulationResultDto(passed, validation, new() { ["documents"] = supplier.Documents.Count, ["categories"] = supplier.Categories.Count, ["values"] = dto.Values.Count }));
    }
    public async Task<RuleResult> EvaluateAsync(string ruleCode, string entityType, Guid entityId, string actor, CancellationToken ct = default)
    {
        var rule = await db.BusinessRuleDefinitions.SingleAsync(r => r.Code == ruleCode && r.IsActive && r.Status == BusinessRuleStatus.Published, ct);
        return (await EvaluateRules([rule], entityType, entityId, actor, null, ct)).Single();
    }
    public async Task<List<RuleResult>> EvaluatePublishedAsync(string appliesTo, string entityType, Guid entityId, string actor, Dictionary<string, string?>? values = null, CancellationToken ct = default)
    {
        var rules = await db.BusinessRuleDefinitions.Where(r => r.AppliesTo == appliesTo && r.IsActive && r.Status == BusinessRuleStatus.Published).OrderBy(r => r.Category).ThenBy(r => r.Code).ToListAsync(ct);
        return await EvaluateRules(rules, entityType, entityId, actor, values, ct);
    }
    public RuleDesignerMetadataDto GetDesignerMetadata(string appliesTo = nameof(Supplier)) => new(["Registration", "Compliance", "Risk", "Documents", "Eligibility"], Fields.ToList(), Functions.ToList(), ["&&", "||", "!", "==", "!="]);
    async Task<List<RuleResult>> EvaluateRules(List<BusinessRuleDefinition> rules, string entityType, Guid entityId, string actor, Dictionary<string, string?>? values, CancellationToken ct)
    {
        object entity = entityType == nameof(Supplier) ? await db.Suppliers.Include(s => s.Documents).Include(s => s.Categories).SingleAsync(s => s.Id == entityId, ct) : entityType == nameof(Requisition) ? await db.Requisitions.Include(r => r.Items).SingleAsync(r => r.Id == entityId, ct) : entityType == nameof(BidSubmission) ? await db.BidSubmissions.Include(b => b.Documents).Include(b => b.Declarations).Include(b => b.Items).SingleAsync(b => b.Id == entityId, ct) : entityType == nameof(BidOpeningSession) ? await db.BidOpeningSessions.Include(b => b.CommitteeMembers).Include(b => b.Submissions).SingleAsync(b => b.Id == entityId, ct) : entityType == nameof(EvaluationSession) ? await db.EvaluationSessions.Include(e => e.CommitteeMembers).Include(e => e.Submissions).Include(e => e.Recommendations).SingleAsync(e => e.Id == entityId, ct) : entityType == nameof(Award) ? await db.Awards.Include(a => a.Items).Include(a => a.Notifications).SingleAsync(a => a.Id == entityId, ct) : entityType == nameof(PurchaseOrder) ? await db.PurchaseOrders.Include(p => p.Lines).Include(p => p.Deliveries).Include(p => p.GoodsReceipts).SingleAsync(p => p.Id == entityId, ct) : entityType == nameof(Contract) ? await db.Contracts.Include(c => c.Lines).Include(c => c.Milestones).SingleAsync(c => c.Id == entityId, ct) : throw new NotSupportedException($"Rules for entity type '{entityType}' are not configured.");
        var results = new List<RuleResult>();
        foreach (var rule in rules)
        {
            var passed = SimpleExpressionEvaluator.Evaluate(rule.Expression, new RuleEvaluationContext(entity, values ?? []));
            var result = new RuleResult(rule.Code, passed, passed ? "Rule passed" : rule.FailureMessage);
            db.BusinessRuleExecutionLogs.Add(new BusinessRuleExecutionLog(rule.Code, entityType, entityId, JsonSerializer.Serialize(entity), passed ? RuleOutcome.Passed : RuleOutcome.Failed, JsonSerializer.Serialize(result), DateTimeOffset.UtcNow));
            db.AuditEvents.Add(new AuditEvent("Rule evaluated", entityType, entityId, EntityReference(entity), actor, result.Message, DateTimeOffset.UtcNow));
            results.Add(result);
        }
        await db.SaveChangesAsync(ct); return results;
    }
    static string EntityReference(object e) => e is Supplier s ? s.ReferenceNumber : e is Requisition r ? r.RequisitionNumber : e is BidSubmission b ? b.SubmissionNumber : e is BidOpeningSession bo ? bo.SessionNumber : e is EvaluationSession ev ? ev.SessionNumber : e is Award a ? a.AwardNumber : e is PurchaseOrder po ? po.PurchaseOrderNumber : e is Contract c ? c.ContractNumber : e.ToString() ?? string.Empty;
}

public sealed record RuleEvaluationContext(object Entity, Dictionary<string, string?> Values);

public static class SimpleExpressionEvaluator
{
    public static RuleValidationResultDto Validate(string expression)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(expression)) errors.Add("Expression is required.");
        if (expression.Contains(';')) errors.Add("Statements are not allowed; use a single boolean expression.");
        if (expression.Count(c => c == '(') != expression.Count(c => c == ')')) errors.Add("Parentheses are not balanced.");
        return new(errors.Count == 0, errors, expression.Length > 900 ? ["Expression is long; consider splitting rules by category."] : []);
    }
    public static bool Evaluate(string expression, object entityOrContext)
    {
        var context = entityOrContext is RuleEvaluationContext c ? c : new RuleEvaluationContext(entityOrContext, []);
        return EvalOr(expression.Trim(), context);
    }
    static bool EvalOr(string e, RuleEvaluationContext c) => SplitTop(e, "||").Select(x => EvalAnd(x, c)).Any(x => x);
    static bool EvalAnd(string e, RuleEvaluationContext c) => SplitTop(e, "&&").Select(x => EvalAtom(x, c)).All(x => x);
    static bool EvalAtom(string raw, RuleEvaluationContext c)
    {
        var e = Unwrap(raw.Trim());
        if (e.StartsWith('!')) return !EvalAtom(e[1..], c);
        if (e.StartsWith("HasDocument(", StringComparison.Ordinal)) return Supplier(c).Documents.Any(d => string.Equals(d.DocumentType, Arg(e), StringComparison.OrdinalIgnoreCase));
        if (e.StartsWith("InCategory(", StringComparison.Ordinal)) return Supplier(c).Categories.Any(x => string.Equals(x.Name, Arg(e), StringComparison.OrdinalIgnoreCase));
        if (e.StartsWith("IsNotEmpty(", StringComparison.Ordinal)) return !string.IsNullOrWhiteSpace(Value(e[11..^1], c));
        if (e.StartsWith("Contains(", StringComparison.Ordinal)) { var args = Args(e); return Value(args[0], c)?.Contains(Quoted(args[1]), StringComparison.OrdinalIgnoreCase) == true; }
        if (e.StartsWith("Equals(", StringComparison.Ordinal)) { var args = Args(e); return string.Equals(Value(args[0], c), Quoted(args[1]), StringComparison.OrdinalIgnoreCase); }
        if (e.StartsWith("Supplier.Documents.Any(", StringComparison.Ordinal)) return Supplier(c).Documents.Any(d => string.Equals(d.DocumentType, Quoted(e.Split("==",2)[1].TrimEnd(')')), StringComparison.OrdinalIgnoreCase));
        if (e == "Requisition.Items.Any()") return Requisition(c).Items.Any();
        if (e == "Requisition.ItemsHaveEstimates()") return Requisition(c).Items.All(i => i.Quantity > 0 && i.EstimatedUnitPrice > 0 && i.EstimatedTotal > 0);
        if (e == "BidSubmission.TenderIsPublished()") return string.Equals(c.Values.GetValueOrDefault("TenderStatus"), "Published", StringComparison.OrdinalIgnoreCase);
        if (e == "BidSubmission.TenderHasNotClosed()") return DateTimeOffset.TryParse(c.Values.GetValueOrDefault("TenderClosingDate"), out var closing) && closing > DateTimeOffset.UtcNow;
        if (e == "BidSubmission.SupplierIsApproved()") return string.Equals(c.Values.GetValueOrDefault("SupplierStatus"), "Approved", StringComparison.OrdinalIgnoreCase);
        if (e == "BidSubmission.RequiredDocumentsUploaded()") return string.Equals(c.Values.GetValueOrDefault("RequiredDocumentsUploaded"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "BidSubmission.MandatoryDeclarationAccepted()") return BidSubmission(c).Declarations.Any(d => d.Accepted);
        if (e == "BidSubmission.SubmissionBeforeClosingDate()") return DateTimeOffset.TryParse(c.Values.GetValueOrDefault("TenderClosingDate"), out var bidClosing) && DateTimeOffset.UtcNow < bidClosing;
        if (e == "BidOpening.TenderClosingDateHasPassed()") return DateTimeOffset.TryParse(c.Values.GetValueOrDefault("TenderClosingDate"), out var openingClosing) && openingClosing <= DateTimeOffset.UtcNow;
        if (e == "BidOpening.TenderIsNotCancelled()") return !string.Equals(c.Values.GetValueOrDefault("TenderStatus"), "Cancelled", StringComparison.OrdinalIgnoreCase);
        if (e == "BidOpening.SessionHasCommittee()") return string.Equals(c.Values.GetValueOrDefault("HasCommittee"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "BidOpening.BidSubmissionIsLocked()") return string.Equals(c.Values.GetValueOrDefault("BidStatus"), "Locked", StringComparison.OrdinalIgnoreCase);
        if (e == "BidOpening.SubmissionBeforeClosingDate()") return DateTimeOffset.TryParse(c.Values.GetValueOrDefault("TenderClosingDate"), out var cdate) && DateTimeOffset.TryParse(c.Values.GetValueOrDefault("SubmittedAt"), out var submitted) && submitted <= cdate;
        if (e == "BidOpening.RequiredDocumentsUploaded()") return string.Equals(c.Values.GetValueOrDefault("RequiredDocumentsUploaded"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "BidOpening.MandatoryDeclarationAccepted()") return string.Equals(c.Values.GetValueOrDefault("MandatoryDeclarationAccepted"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "BidOpening.UserIsCommitteeMember()") return true;
        if (e == "Evaluation.BidOpeningCompleted()") return string.Equals(c.Values.GetValueOrDefault("BidOpeningCompleted"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Evaluation.BidSubmissionOpened()") return Evaluation(c).Submissions.Any();
        if (e == "Evaluation.HasCommittee()") return string.Equals(c.Values.GetValueOrDefault("HasCommittee"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Evaluation.DeclarationsAccepted()") return string.Equals(c.Values.GetValueOrDefault("DeclarationsAccepted"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Evaluation.TechnicalThresholdMet()") return true;
        if (e == "Evaluation.FinancialCompleted()") return true;
        if (e == "Evaluation.ConsensusCompleted()") return true;
        if (e == "Evaluation.RecommendationExists()") return Evaluation(c).Recommendations.Any();
        if (e == "Award.EvaluationCompleted()") return string.Equals(c.Values.GetValueOrDefault("EvaluationCompleted"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.HasRecommendation()") return string.Equals(c.Values.GetValueOrDefault("HasRecommendation"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.RecommendedBidExists()") return string.Equals(c.Values.GetValueOrDefault("RecommendedBidExists"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.SupplierValid()") return string.Equals(c.Values.GetValueOrDefault("SupplierValid"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.AwardAmountPositive()") return Award(c).AwardAmount > 0 || string.Equals(c.Values.GetValueOrDefault("AwardAmountPositive"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.WithinBudget()") return string.Equals(c.Values.GetValueOrDefault("WithinBudget"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.RequiresApprovalBeforePublication()") return string.Equals(c.Values.GetValueOrDefault("RequiresApprovalBeforePublication"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Award.NotPublishedTwice()") return string.Equals(c.Values.GetValueOrDefault("NotPublishedTwice"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.AwardApproved()") return string.Equals(c.Values.GetValueOrDefault("AwardApproved"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.SupplierExists()") return string.Equals(c.Values.GetValueOrDefault("SupplierExists"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.BudgetCommitmentExists()") return string.Equals(c.Values.GetValueOrDefault("BudgetCommitmentExists"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.TotalEqualsAward()") return string.Equals(c.Values.GetValueOrDefault("PoTotalEqualsAward"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.DoesNotExceedAward()") return string.Equals(c.Values.GetValueOrDefault("PoDoesNotExceedAward"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.NotCancelled()") return string.Equals(c.Values.GetValueOrDefault("NotCancelled"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "PurchaseOrder.NotClosed()") return string.Equals(c.Values.GetValueOrDefault("NotClosed"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Contract.AwardApproved()") return string.Equals(c.Values.GetValueOrDefault("AwardApproved"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Contract.PurchaseOrderIssued()") return string.Equals(c.Values.GetValueOrDefault("PurchaseOrderIssued"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Contract.ContractDatesValid()") return Contract(c).EndDate > Contract(c).StartDate || string.Equals(c.Values.GetValueOrDefault("ContractDatesValid"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Contract.ContractValuePositive()") return Contract(c).CurrentValue > 0 || string.Equals(c.Values.GetValueOrDefault("ContractValuePositive"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Contract.RenewalBeforeExpiry()") return true;
        if (e == "Contract.VariationRequiresApproval()") return true;
        if (e == "Contract.MilestonesComplete()") return Contract(c).Milestones.All(x => x.Status == "Completed") || string.Equals(c.Values.GetValueOrDefault("MilestonesComplete"), "true", StringComparison.OrdinalIgnoreCase);
        if (e == "Supplier.Categories.Any()") return Supplier(c).Categories.Any();
        if (e.Contains(" != ")) { var parts = e.Split(" != ", 2, StringSplitOptions.TrimEntries); return !string.Equals(Value(parts[0], c), Quoted(parts[1]), StringComparison.OrdinalIgnoreCase); }
        if (e.Contains(" == ")) { var parts = e.Split(" == ", 2, StringSplitOptions.TrimEntries); return string.Equals(Value(parts[0], c), Quoted(parts[1]), StringComparison.OrdinalIgnoreCase); }
        throw new InvalidOperationException($"Expression '{e}' is not supported by the safe evaluator.");
    }
    static Contract Contract(RuleEvaluationContext c) => c.Entity as Contract ?? throw new InvalidOperationException("Contract rules require a Contract context.");
    static PurchaseOrder PurchaseOrder(RuleEvaluationContext c) => c.Entity as PurchaseOrder ?? throw new InvalidOperationException("Purchase order rules require a PurchaseOrder context.");
    static Award Award(RuleEvaluationContext c) => c.Entity as Award ?? throw new InvalidOperationException("Award rules require an Award context.");
    static EvaluationSession Evaluation(RuleEvaluationContext c) => c.Entity as EvaluationSession ?? throw new InvalidOperationException("Evaluation rules require an EvaluationSession context.");
    static Supplier Supplier(RuleEvaluationContext c) => c.Entity as Supplier ?? throw new InvalidOperationException("Supplier rules require a Supplier context.");
    static Requisition Requisition(RuleEvaluationContext c) => c.Entity as Requisition ?? throw new InvalidOperationException("Requisition rules require a Requisition context.");
    static BidSubmission BidSubmission(RuleEvaluationContext c) => c.Entity as BidSubmission ?? throw new InvalidOperationException("Bid submission rules require a BidSubmission context.");
    static BidOpeningSession BidOpeningSession(RuleEvaluationContext c) => c.Entity as BidOpeningSession ?? throw new InvalidOperationException("Bid opening rules require a BidOpeningSession context.");
    static string? Value(string token, RuleEvaluationContext c)
    {
        token = token.Trim();
        if (token.StartsWith("Field(", StringComparison.Ordinal)) return c.Values.GetValueOrDefault(Arg(token));
        if (c.Entity is Supplier s) return token switch { "Supplier.ReferenceNumber" => s.ReferenceNumber, "Supplier.LegalName" => s.LegalName, "Supplier.Status" => s.Status.ToString(), _ => c.Values.GetValueOrDefault(token) ?? Quoted(token) };
        if (c.Entity is PurchaseOrder po) return token switch { "PurchaseOrder.PurchaseOrderNumber" => po.PurchaseOrderNumber, "PurchaseOrder.SupplierName" => po.SupplierName, "PurchaseOrder.Status" => po.Status.ToString(), _ => c.Values.GetValueOrDefault(token) ?? Quoted(token) };
        return c.Values.GetValueOrDefault(token) ?? Quoted(token);
    }
    static string Unwrap(string e) { while (e.StartsWith('(') && e.EndsWith(')')) e = e[1..^1].Trim(); return e; }
    static List<string> SplitTop(string e, string op) { var parts = new List<string>(); var depth = 0; var start = 0; for (var i=0;i<e.Length-(op.Length-1);i++){ if(e[i]=='(')depth++; else if(e[i]==')')depth--; else if(depth==0 && e.Substring(i,op.Length)==op){ parts.Add(e[start..i]); start=i+op.Length; i+=op.Length-1; }} parts.Add(e[start..]); return parts; }
    static string Arg(string call) => Quoted(call[(call.IndexOf('(')+1)..call.LastIndexOf(')')]);
    static List<string> Args(string call) => SplitTop(call[(call.IndexOf('(')+1)..call.LastIndexOf(')')], ",");
    static string Quoted(string value) => value.Trim().Trim('"');
}

public sealed class WorkflowApplicationService(EProcurementDbContext db, IBusinessRuleApplicationService rules) : IWorkflowApplicationService
{
    public Task<List<WorkflowDefinition>> GetDefinitionsAsync(CancellationToken ct = default) => db.WorkflowDefinitions.AsNoTracking().Include(w => w.Versions).ThenInclude(v => v.Nodes).Include(w => w.Versions).ThenInclude(v => v.Transitions).OrderBy(w => w.Code).ToListAsync(ct);
    public async Task<PlatformDashboardDto> GetDashboardAsync(CancellationToken ct = default) => new(
        await db.WorkflowDefinitions.CountAsync(ct),
        await db.BusinessRuleDefinitions.CountAsync(ct),
        await db.FormDefinitions.CountAsync(ct),
        await db.WorkflowInstances.CountAsync(i => i.Status == WorkflowInstanceStatus.Running, ct),
        await db.WorkflowTasks.CountAsync(t => t.Status == WorkflowTaskStatus.Open || t.Status == WorkflowTaskStatus.Assigned, ct),
        await db.BusinessProcessDefinitions.CountAsync(x => x.Status == BusinessProcessStatus.Published, ct),
        await db.ApprovalMatrices.CountAsync(ct));
    public async Task<WorkflowDefinition> CreateWorkflowAsync(CreateWorkflowDto request, CancellationToken ct = default)
    {
        var definition = new WorkflowDefinition(request.Code, request.Name, request.EntityType); var version = new WorkflowVersion(definition.Id, 1);
        version.Nodes.AddRange(request.Nodes.Select(n => new WorkflowNode(version.Id, n.Code, n.Name, n.Kind, n.CreatesTask, n.DefaultAssignedRole, n.IsStart, n.IsTerminal, n.PositionX, n.PositionY, n.ActionConfigurationJson, n.ConditionConfigurationJson, n.BusinessRuleCodesJson, n.AssignedRolesJson)));
        version.Transitions.AddRange(request.Transitions.Select(t => new WorkflowTransition(version.Id, t.FromNodeCode, t.ActionCode, t.ActionName, t.ToNodeCode, t.RequiredRuleCode, t.ConditionExpression, t.ActionConfigurationJson, t.BusinessRuleCodesJson, t.AssignedRolesJson)));
        definition.Versions.Add(version); db.WorkflowDefinitions.Add(definition); await db.SaveChangesAsync(ct);
        foreach (var e in request.Effects ?? []) { var t = version.Transitions.Single(x => x.ActionCode == e.TriggerActionCode && x.FromNodeCode == e.TriggerFromNodeCode); db.WorkflowTransitionEffects.Add(new WorkflowTransitionEffect(e.EntityType, e.PropertyName, e.ValueExpression, t.Id)); }
        await db.SaveChangesAsync(ct); return definition;
    }
    public async Task<WorkflowNode?> AddNodeAsync(string workflowCode, WorkflowNodeDto dto, CancellationToken ct = default)
    {
        var version = await DraftWorkflowVersion(workflowCode, ct);
        if (version is null) return null;
        var node = new WorkflowNode(version.Id, dto.Code, dto.Name, dto.Kind, dto.CreatesTask, dto.DefaultAssignedRole, dto.IsStart, dto.IsTerminal);
        db.WorkflowNodes.Add(node); await db.SaveChangesAsync(ct); return node;
    }
    public async Task<WorkflowTransition?> AddTransitionAsync(string workflowCode, WorkflowTransitionDto dto, CancellationToken ct = default)
    {
        var version = await DraftWorkflowVersion(workflowCode, ct);
        if (version is null) return null;
        var transition = new WorkflowTransition(version.Id, dto.FromNodeCode, dto.ActionCode, dto.ActionName, dto.ToNodeCode, dto.RequiredRuleCode, dto.ConditionExpression, dto.ActionConfigurationJson, dto.BusinessRuleCodesJson, dto.AssignedRolesJson);
        db.WorkflowTransitions.Add(transition); await db.SaveChangesAsync(ct); return transition;
    }
    public async Task<WorkflowDefinition?> PublishAsync(string code, string actor, CancellationToken ct = default) { var d = await db.WorkflowDefinitions.Include(x => x.Versions).SingleOrDefaultAsync(x => x.Code == code, ct); var v = d?.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault(x => x.Status == WorkflowVersionStatus.Draft); if (d is null || v is null) return null; foreach (var published in d.Versions.Where(x => x.Status == WorkflowVersionStatus.Published)) db.Entry(published).CurrentValues[nameof(WorkflowVersion.Status)] = WorkflowVersionStatus.Archived; db.Entry(v).CurrentValues[nameof(WorkflowVersion.Status)] = WorkflowVersionStatus.Published; db.Entry(v).CurrentValues[nameof(WorkflowVersion.PublishedAt)] = DateTimeOffset.UtcNow; db.Entry(v).CurrentValues[nameof(WorkflowVersion.PublishedBy)] = actor; db.Entry(d).CurrentValues[nameof(WorkflowDefinition.PublishedVersionId)] = v.Id; await db.SaveChangesAsync(ct); return d; }
    public async Task<WorkflowDefinition?> ArchiveAsync(string code, int versionNumber, string actor, CancellationToken ct = default) { var d = await db.WorkflowDefinitions.Include(x => x.Versions).SingleOrDefaultAsync(x => x.Code == code, ct); var v = d?.Versions.SingleOrDefault(x => x.VersionNumber == versionNumber); if (d is null || v is null) return null; db.Entry(v).CurrentValues[nameof(WorkflowVersion.Status)] = WorkflowVersionStatus.Archived; if (d.PublishedVersionId == v.Id) db.Entry(d).CurrentValues[nameof(WorkflowDefinition.PublishedVersionId)] = null; await db.SaveChangesAsync(ct); return d; }
    public async Task<WorkflowDefinition> SaveDesignerAsync(WorkflowDesignerDto dto, CancellationToken ct = default)
    {
        var definition = await db.WorkflowDefinitions.Include(x => x.Versions).ThenInclude(x => x.Nodes).Include(x => x.Versions).ThenInclude(x => x.Transitions).SingleOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (definition is null) { definition = new WorkflowDefinition(dto.Code, dto.Name, dto.EntityType); db.WorkflowDefinitions.Add(definition); await db.SaveChangesAsync(ct); }
        else { db.Entry(definition).CurrentValues[nameof(WorkflowDefinition.Name)] = dto.Name; db.Entry(definition).CurrentValues[nameof(WorkflowDefinition.EntityType)] = dto.EntityType; }
        var version = definition.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault(x => x.Status == WorkflowVersionStatus.Draft);
        if (version is null) { version = new WorkflowVersion(definition.Id, (definition.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault()?.VersionNumber ?? 0) + 1); db.WorkflowVersions.Add(version); await db.SaveChangesAsync(ct); }
        db.WorkflowNodes.RemoveRange(version.Nodes); db.WorkflowTransitions.RemoveRange(version.Transitions); await db.SaveChangesAsync(ct);
        db.WorkflowNodes.AddRange(dto.Nodes.Select(n => new WorkflowNode(version.Id, n.Code, n.Name, n.Kind, n.CreatesTask, n.DefaultAssignedRole, n.IsStart, n.IsTerminal, n.PositionX, n.PositionY, n.ActionConfigurationJson, n.ConditionConfigurationJson, n.BusinessRuleCodesJson, n.AssignedRolesJson)));
        db.WorkflowTransitions.AddRange(dto.Transitions.Select(t => new WorkflowTransition(version.Id, t.FromNodeCode, t.ActionCode, t.ActionName, t.ToNodeCode, t.RequiredRuleCode, t.ConditionExpression, t.ActionConfigurationJson, t.BusinessRuleCodesJson, t.AssignedRolesJson)));
        await db.SaveChangesAsync(ct); return await Load(dto.Code, ct);
    }
    public async Task<WorkflowInstance> StartAsync(string workflowCode, string entityType, Guid entityId, string actor, CancellationToken ct = default) { var d = await Load(workflowCode, ct); var v = Published(d); var start = v.Nodes.Single(n => n.IsStart); var i = new WorkflowInstance(d.Id, v.Id, entityType, entityId, start.Code, StartedAt: DateTimeOffset.UtcNow); db.WorkflowInstances.Add(i); db.WorkflowHistories.Add(new WorkflowHistory(i.Id,"WorkflowStarted",start.Code,actor,workflowCode,DateTimeOffset.UtcNow)); db.AuditEvents.Add(new AuditEvent("Workflow started", entityType, entityId, await Reference(entityType, entityId, ct), actor, workflowCode, DateTimeOffset.UtcNow)); await CreateTask(v, i, actor, ct); await db.SaveChangesAsync(ct); return i; }
    public async Task<WorkflowInstance?> ExecuteActionAsync(Guid instanceId, string actionCode, string actor, CancellationToken ct = default) { var i = await db.WorkflowInstances.SingleOrDefaultAsync(x => x.Id == instanceId, ct); if (i is null) return null; var v = await db.WorkflowVersions.Include(x => x.Nodes).Include(x => x.Transitions).SingleAsync(x => x.Id == i.WorkflowVersionId, ct); var t = v.Transitions.Single(x => x.FromNodeCode == i.CurrentNodeCode && x.ActionCode == actionCode); if (t.RequiredRuleCode is not null && !(await rules.EvaluateAsync(t.RequiredRuleCode, i.EntityType, i.EntityId, actor, ct)).Passed) throw new InvalidOperationException($"Workflow action '{actionCode}' blocked by rule '{t.RequiredRuleCode}'."); foreach (var task in await db.WorkflowTasks.Where(x => x.WorkflowInstanceId == i.Id && x.NodeCode == i.CurrentNodeCode && (x.Status == WorkflowTaskStatus.Open || x.Status == WorkflowTaskStatus.Assigned)).ToListAsync(ct)) { db.Entry(task).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Completed; db.Entry(task).CurrentValues[nameof(WorkflowTask.CompletedAt)] = DateTimeOffset.UtcNow; } var target = v.Nodes.Single(n => n.Code == t.ToNodeCode); db.Entry(i).CurrentValues[nameof(WorkflowInstance.CurrentNodeCode)] = target.Code; db.Entry(i).CurrentValues[nameof(WorkflowInstance.Status)] = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running; db.WorkflowActions.Add(new WorkflowAction(i.Id, actionCode, t.ActionName, WorkflowActionKind.Transition, t.FromNodeCode, t.ToNodeCode, actor, DateTimeOffset.UtcNow)); db.WorkflowHistories.Add(new WorkflowHistory(i.Id, "WorkflowActionExecuted", t.ToNodeCode, actor, t.ActionName, DateTimeOffset.UtcNow)); await ApplyEffects(t.Id, i.EntityType, i.EntityId, ct); db.AuditEvents.Add(new AuditEvent("Workflow action executed", i.EntityType, i.EntityId, await Reference(i.EntityType, i.EntityId, ct), actor, $"{t.FromNodeCode} -> {t.ToNodeCode}", DateTimeOffset.UtcNow)); await CreateTask(v, i with { CurrentNodeCode = target.Code }, actor, ct); await db.SaveChangesAsync(ct); return i with { CurrentNodeCode = target.Code, Status = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running }; }
    public Task<List<WorkflowTask>> GetTasksAsync(CancellationToken ct = default) => db.WorkflowTasks.AsNoTracking().OrderBy(x => x.CreatedAt).ToListAsync(ct);
    public async Task<WorkflowTask?> AssignTaskAsync(Guid id, string assignedTo, string actor, CancellationToken ct = default) { var t = await db.WorkflowTasks.SingleOrDefaultAsync(x => x.Id == id, ct); if (t is null) return null; db.Entry(t).CurrentValues[nameof(WorkflowTask.AssignedTo)] = assignedTo; db.Entry(t).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Assigned; await db.SaveChangesAsync(ct); return t with { AssignedTo = assignedTo, Status = WorkflowTaskStatus.Assigned }; }
    public async Task<WorkflowTask?> CompleteTaskAsync(Guid id, string actor, CancellationToken ct = default) { var t = await db.WorkflowTasks.SingleOrDefaultAsync(x => x.Id == id, ct); if (t is null) return null; db.Entry(t).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Completed; await db.SaveChangesAsync(ct); return t with { Status = WorkflowTaskStatus.Completed }; }
    async Task<WorkflowDefinition> Load(string code, CancellationToken ct) => await db.WorkflowDefinitions.Include(w => w.Versions).ThenInclude(v => v.Nodes).Include(w => w.Versions).ThenInclude(v => v.Transitions).SingleAsync(w => w.Code == code && w.IsActive, ct);
    async Task<WorkflowVersion?> DraftWorkflowVersion(string code, CancellationToken ct) => (await db.WorkflowDefinitions.Include(w => w.Versions).SingleOrDefaultAsync(w => w.Code == code && w.IsActive, ct))?.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault(v => v.Status == WorkflowVersionStatus.Draft);
    static WorkflowVersion Published(WorkflowDefinition d) => d.Versions.Single(v => v.Id == d.PublishedVersionId || v.Status == WorkflowVersionStatus.Published);
    async Task CreateTask(WorkflowVersion v, WorkflowInstance i, string actor, CancellationToken ct) { var n = v.Nodes.Single(x => x.Code == i.CurrentNodeCode); if (!n.CreatesTask) return; db.WorkflowTasks.Add(new WorkflowTask(i.Id, n.Code, n.DefaultAssignedRole, CreatedAt: DateTimeOffset.UtcNow)); await Task.CompletedTask; }
    async Task ApplyEffects(Guid transitionId, string entityType, Guid entityId, CancellationToken ct) { foreach (var e in await db.WorkflowTransitionEffects.Where(x => x.TriggerTransitionId == transitionId && x.EntityType == entityType).ToListAsync(ct)) if (entityType == nameof(Supplier) && e.PropertyName == nameof(Supplier.Status)) { var s = await db.Suppliers.SingleAsync(x => x.Id == entityId, ct); db.Entry(s).CurrentValues[e.PropertyName] = Enum.Parse<SupplierStatus>(e.ValueExpression.Trim('"'), true); } }
    async Task<string> Reference(string entityType, Guid entityId, CancellationToken ct) => entityType == nameof(Supplier) ? (await db.Suppliers.SingleAsync(s => s.Id == entityId, ct)).ReferenceNumber : entityType == nameof(Requisition) ? (await db.Requisitions.SingleAsync(r => r.Id == entityId, ct)).RequisitionNumber : entityType == nameof(BidOpeningSession) ? (await db.BidOpeningSessions.SingleAsync(r => r.Id == entityId, ct)).SessionNumber : entityId.ToString();
}

public sealed class SupplierApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IDynamicFormApplicationService forms, IBusinessRuleApplicationService rules) : ISupplierApplicationService
{
    public async Task<List<object>> GetSuppliersAsync(CancellationToken ct = default) => await db.Suppliers.AsNoTracking().Include(s => s.Documents).Include(s => s.Categories).OrderBy(s => s.ReferenceNumber).Select(s => new { s.Id, s.ReferenceNumber, s.LegalName, Status = s.Status.ToString(), Documents = s.Documents, Categories = s.Categories.Select(c => c.Name) }).Cast<object>().ToListAsync(ct);

    public async Task<SupplierRegistrationConfigurationDto?> GetRegistrationConfigurationAsync(CancellationToken ct = default)
    {
        var process = await ActiveSupplierProcess(ct);
        if (process?.ActiveFormDefinitionId is null || process.ActiveWorkflowDefinitionId is null || process.ActiveDocumentRequirementSetId is null) return null;
        var activeFormVersionId = await db.FormDefinitions
            .AsNoTracking()
            .Where(x => x.Id == process.ActiveFormDefinitionId)
            .Select(x => x.ActiveVersionId)
            .SingleAsync(ct);
        var publishedWorkflowVersionId = await db.WorkflowDefinitions
            .AsNoTracking()
            .Where(x => x.Id == process.ActiveWorkflowDefinitionId)
            .Select(x => x.PublishedVersionId)
            .SingleAsync(ct);

        var form = await db.FormDefinitions
            .AsNoTracking()
            .Include(x => x.Versions.Where(v => (activeFormVersionId != null && v.Id == activeFormVersionId) || (activeFormVersionId == null && v.Status == WorkflowVersionStatus.Published)))
            .ThenInclude(v => v.Sections)
            .ThenInclude(s => s.Fields)
            .ThenInclude(f => f.Validations)
            .Include(x => x.Versions.Where(v => (activeFormVersionId != null && v.Id == activeFormVersionId) || (activeFormVersionId == null && v.Status == WorkflowVersionStatus.Published)))
            .ThenInclude(v => v.Sections)
            .ThenInclude(s => s.Fields)
            .ThenInclude(f => f.VisibilityRules)
            .SingleAsync(x => x.Id == process.ActiveFormDefinitionId, ct);
        var docs = await db.DocumentRequirementSets.AsNoTracking().Include(x => x.Requirements).SingleAsync(x => x.Id == process.ActiveDocumentRequirementSetId, ct);
        var matrix = process.ActiveApprovalMatrixId is null ? null : await db.ApprovalMatrices.AsNoTracking().Include(x => x.Steps).SingleAsync(x => x.Id == process.ActiveApprovalMatrixId, ct);
        var workflow = await db.WorkflowDefinitions
            .AsNoTracking()
            .Include(x => x.Versions.Where(v => (publishedWorkflowVersionId != null && v.Id == publishedWorkflowVersionId) || (publishedWorkflowVersionId == null && v.Status == WorkflowVersionStatus.Published)))
            .ThenInclude(v => v.Nodes)
            .Include(x => x.Versions.Where(v => (publishedWorkflowVersionId != null && v.Id == publishedWorkflowVersionId) || (publishedWorkflowVersionId == null && v.Status == WorkflowVersionStatus.Published)))
            .ThenInclude(v => v.Transitions)
            .SingleAsync(x => x.Id == process.ActiveWorkflowDefinitionId, ct);
        return new(process, form, docs, matrix, workflow);
    }

    public async Task<SupplierRegistrationResultDto?> RegisterAsync(RegisterSupplierDto dto, CancellationToken ct = default)
    {
        var config = await GetRegistrationConfigurationAsync(ct); if (config is null) return null;
        var legalName = dto.Values.TryGetValue("legalName", out var name) && !string.IsNullOrWhiteSpace(name) ? name! : dto.ReferenceNumber;
        var supplier = await db.Suppliers.Include(x => x.Documents).SingleOrDefaultAsync(x => x.ReferenceNumber == dto.ReferenceNumber, ct);
        if (supplier is null) { supplier = new Supplier(dto.ReferenceNumber, legalName, SupplierStatus.Draft); db.Suppliers.Add(supplier); await db.SaveChangesAsync(ct); }
        else db.Entry(supplier).CurrentValues[nameof(Supplier.LegalName)] = legalName;
        foreach (var doc in dto.Documents) if (!supplier.Documents.Any(x => x.DocumentType == doc.DocumentType && x.FileName == doc.FileName)) db.SupplierDocuments.Add(new SupplierDocument(supplier.Id, doc.DocumentType, doc.FileName, dto.Actor, DateTimeOffset.UtcNow));
        db.AuditEvents.Add(new AuditEvent("Supplier registration submitted", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, dto.Actor, config.Process.Code, DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct);
        var ruleResults = await rules.EvaluatePublishedAsync("SupplierRegistration", nameof(Supplier), supplier.Id, dto.Actor, dto.Values, ct);
        if (ruleResults.Any(x => !x.Passed)) throw new InvalidOperationException($"Supplier registration blocked by published rules: {string.Join(", ", ruleResults.Where(x => !x.Passed).Select(x => x.RuleCode))}");
        var submission = await forms.SubmitAsync(new SubmitFormDto(config.Form.Code, nameof(Supplier), supplier.Id, dto.Actor, dto.Values), ct);
        var instance = await workflows.StartAsync(config.Workflow.Code, nameof(Supplier), supplier.Id, dto.Actor, ct);
        var version = config.Workflow.Versions.Single(v => v.Id == config.Workflow.PublishedVersionId || v.Status == WorkflowVersionStatus.Published);
        var transition = version.Transitions.FirstOrDefault(t => t.FromNodeCode == instance.CurrentNodeCode);
        if (transition is not null) instance = await workflows.ExecuteActionAsync(instance.Id, transition.ActionCode, dto.Actor, ct) ?? instance;
        return new SupplierRegistrationResultDto(supplier.ReferenceNumber, supplier.Id, submission.Id, instance.Id, instance.CurrentNodeCode, instance.Status.ToString());
    }

    public async Task<SupplierDetailDto?> GetSupplierDetailAsync(string referenceNumber, CancellationToken ct = default)
    {
        var supplier = await db.Suppliers.AsNoTracking().Include(x => x.Documents).Include(x => x.Categories).SingleOrDefaultAsync(x => x.ReferenceNumber == referenceNumber, ct); if (supplier is null) return null;
        return await BuildSupplierDetail(supplier, ct);
    }

    public async Task<WorkflowTaskDetailDto?> GetTaskDetailAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await db.WorkflowTasks.AsNoTracking().SingleOrDefaultAsync(x => x.Id == taskId, ct); if (task is null) return null;
        var instance = await db.WorkflowInstances.AsNoTracking().SingleAsync(x => x.Id == task.WorkflowInstanceId, ct);
        SupplierDetailDto? supplier = null;
        if (instance.EntityType == nameof(Supplier)) { var s = await db.Suppliers.AsNoTracking().Include(x => x.Documents).Include(x => x.Categories).SingleAsync(x => x.Id == instance.EntityId, ct); supplier = await BuildSupplierDetail(s, ct); }
        return new(task, supplier, await db.WorkflowHistories.AsNoTracking().Where(x => x.WorkflowInstanceId == task.WorkflowInstanceId).OrderBy(x => x.OccurredAt).ToListAsync(ct), await db.WorkflowActions.AsNoTracking().Where(x => x.WorkflowInstanceId == task.WorkflowInstanceId).OrderBy(x => x.ActionedAt).ToListAsync(ct), await AvailableActions(instance, ct));
    }

    public async Task<WorkflowActionResultDto?> ExecuteTaskActionAsync(Guid taskId, ExecuteWorkflowTaskActionDto dto, CancellationToken ct = default)
    {
        var task = await db.WorkflowTasks.AsNoTracking().SingleOrDefaultAsync(x => x.Id == taskId, ct); if (task is null) return null;
        var instance = await workflows.ExecuteActionAsync(task.WorkflowInstanceId, dto.ActionCode, dto.Actor, ct); if (instance is null) return null;
        var nextTask = await db.WorkflowTasks.AsNoTracking().Where(x => x.WorkflowInstanceId == instance.Id && (x.Status == WorkflowTaskStatus.Open || x.Status == WorkflowTaskStatus.Assigned)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(ct);
        return new(instance, nextTask is null ? null : await GetTaskDetailAsync(nextTask.Id, ct));
    }

    public async Task<WorkflowInstance?> SubmitAsync(string referenceNumber, string actor, CancellationToken ct = default)
    {
        var supplier = await db.Suppliers.SingleOrDefaultAsync(s => s.ReferenceNumber == referenceNumber, ct); if (supplier is null) return null;
        var config = await GetRegistrationConfigurationAsync(ct); if (config is null) return null;
        return await workflows.StartAsync(config.Workflow.Code, nameof(Supplier), supplier.Id, actor, ct);
    }

    async Task<BusinessProcessDefinition?> ActiveSupplierProcess(CancellationToken ct) => await db.BusinessProcessDefinitions.AsNoTracking().OrderBy(x => x.Code).FirstOrDefaultAsync(p => p.EntityType == nameof(Supplier) && p.Status == BusinessProcessStatus.Published, ct);
    async Task<SupplierDetailDto> BuildSupplierDetail(Supplier supplier, CancellationToken ct) { var instance = await db.WorkflowInstances.AsNoTracking().Where(x => x.EntityType == nameof(Supplier) && x.EntityId == supplier.Id).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct); return new(supplier, instance, instance?.CurrentNodeCode, supplier.Documents, await db.FormSubmissions.AsNoTracking().Include(x => x.Values).Where(x => x.EntityType == nameof(Supplier) && x.EntityId == supplier.Id).OrderByDescending(x => x.SubmittedAt).ToListAsync(ct), await db.AuditEvents.AsNoTracking().Where(x => x.EntityType == nameof(Supplier) && x.EntityId == supplier.Id).OrderBy(x => x.OccurredAt).ToListAsync(ct), instance is null ? [] : await AvailableActions(instance, ct)); }
    async Task<List<WorkflowTransition>> AvailableActions(WorkflowInstance instance, CancellationToken ct) => instance.Status == WorkflowInstanceStatus.Running ? await db.WorkflowTransitions.AsNoTracking().Where(x => x.WorkflowVersionId == instance.WorkflowVersionId && x.FromNodeCode == instance.CurrentNodeCode).OrderBy(x => x.ActionName).ToListAsync(ct) : [];
}


public sealed class DynamicFormApplicationService(EProcurementDbContext db) : IDynamicFormApplicationService
{
    public Task<List<FormDefinition>> GetDefinitionsAsync(CancellationToken ct = default) => db.FormDefinitions.AsNoTracking().Include(f => f.Versions).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).ThenInclude(f => f.Validations).Include(f => f.Versions).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).ThenInclude(f => f.VisibilityRules).OrderBy(f => f.Code).ToListAsync(ct);
    public async Task<FormDefinition> CreateDefinitionAsync(CreateFormDefinitionDto dto, CancellationToken ct = default)
    {
        var existing = await db.FormDefinitions.Include(x => x.Versions).ThenInclude(v => v.Sections).ThenInclude(sec => sec.Fields).SingleOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (existing is not null)
        {
            db.FormDefinitions.Remove(existing);
            await db.SaveChangesAsync(ct);
        }
        var def = new FormDefinition(dto.Code, dto.Name, dto.EntityType); var version = new FormVersion(def.Id, 1);
        foreach (var s in dto.Sections) { var section = new FormSection(version.Id, s.Code, s.Title, s.DisplayOrder); foreach (var f in s.Fields)
            {
                var field = new FormField(section.Id, f.Code, f.Label, f.FieldType, f.DisplayOrder, f.IsRequired);
                field.Validations.AddRange((f.Validations ?? []).Select(v => new FormFieldValidation(field.Id, v.ValidationType, v.ConfigurationJson, v.Message)));
                field.VisibilityRules.AddRange((f.VisibilityRules ?? []).Select(v => new FormFieldVisibilityRule(field.Id, v.Expression)));
                section.Fields.Add(field);
            } version.Sections.Add(section); }
        def.Versions.Add(version); db.FormDefinitions.Add(def); await db.SaveChangesAsync(ct); return def;
    }
    public async Task<FormSection?> AddSectionAsync(string formCode, FormSectionDto dto, CancellationToken ct = default)
    {
        var version = await DraftFormVersion(formCode, ct);
        if (version is null) return null;
        var section = new FormSection(version.Id, dto.Code, dto.Title, dto.DisplayOrder);
        foreach (var f in dto.Fields)
        {
            var field = new FormField(section.Id, f.Code, f.Label, f.FieldType, f.DisplayOrder, f.IsRequired);
            field.Validations.AddRange((f.Validations ?? []).Select(v => new FormFieldValidation(field.Id, v.ValidationType, v.ConfigurationJson, v.Message)));
            field.VisibilityRules.AddRange((f.VisibilityRules ?? []).Select(v => new FormFieldVisibilityRule(field.Id, v.Expression)));
            section.Fields.Add(field);
        }
        db.FormSections.Add(section); await db.SaveChangesAsync(ct); return section;
    }
    public async Task<FormField?> AddFieldAsync(string formCode, string sectionCode, FormFieldDto dto, CancellationToken ct = default)
    {
        var version = await DraftFormVersion(formCode, ct);
        if (version is null) return null;
        var section = await db.FormSections.SingleOrDefaultAsync(s => s.FormVersionId == version.Id && s.Code == sectionCode, ct);
        if (section is null) return null;
        var field = new FormField(section.Id, dto.Code, dto.Label, dto.FieldType, dto.DisplayOrder, dto.IsRequired);
        field.Validations.AddRange((dto.Validations ?? []).Select(v => new FormFieldValidation(field.Id, v.ValidationType, v.ConfigurationJson, v.Message)));
        field.VisibilityRules.AddRange((dto.VisibilityRules ?? []).Select(v => new FormFieldVisibilityRule(field.Id, v.Expression)));
        db.FormFields.Add(field); await db.SaveChangesAsync(ct); return field;
    }
    public async Task<FormDefinition?> PublishVersionAsync(string code, string actor, CancellationToken ct = default) { var d = await db.FormDefinitions.Include(x => x.Versions).SingleOrDefaultAsync(x => x.Code == code, ct); var v = d?.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault(x => x.Status == WorkflowVersionStatus.Draft); if (d is null || v is null) return null; db.Entry(v).CurrentValues[nameof(FormVersion.Status)] = WorkflowVersionStatus.Published; db.Entry(v).CurrentValues[nameof(FormVersion.PublishedAt)] = DateTimeOffset.UtcNow; db.Entry(v).CurrentValues[nameof(FormVersion.PublishedBy)] = actor; db.Entry(d).CurrentValues[nameof(FormDefinition.ActiveVersionId)] = v.Id; await db.SaveChangesAsync(ct); return d; }
    public Task<FormDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default) => db.FormDefinitions.AsNoTracking().Include(d => d.Versions.Where(v => v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).ThenInclude(f => f.Validations).Include(d => d.Versions.Where(v => v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).ThenInclude(f => f.VisibilityRules).SingleOrDefaultAsync(d => d.Code == code && d.IsActive, ct);
    public async Task<FormSubmission> SubmitAsync(SubmitFormDto dto, CancellationToken ct = default) { var d = await db.FormDefinitions.SingleAsync(x => x.Code == dto.FormCode && x.ActiveVersionId != null, ct); var sub = new FormSubmission(d.Id, d.ActiveVersionId!.Value, dto.EntityType, dto.EntityId, dto.SubmittedBy, DateTimeOffset.UtcNow); sub.Values.AddRange(dto.Values.Select(v => new FormSubmissionValue(sub.Id, v.Key, v.Value))); db.FormSubmissions.Add(sub); await db.SaveChangesAsync(ct); return sub; }
    public Task<List<FormSubmission>> GetSubmissionsAsync(string entityType, Guid entityId, CancellationToken ct = default) => db.FormSubmissions.AsNoTracking().Include(s => s.Values).Where(s => s.EntityType == entityType && s.EntityId == entityId).ToListAsync(ct);
    async Task<FormVersion?> DraftFormVersion(string code, CancellationToken ct) => (await db.FormDefinitions.Include(f => f.Versions).SingleOrDefaultAsync(f => f.Code == code && f.IsActive, ct))?.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault(v => v.Status == WorkflowVersionStatus.Draft);
}
public sealed class AuditApplicationService(EProcurementDbContext db) : IAuditApplicationService { public Task<List<AuditEvent>> GetEventsAsync(CancellationToken ct = default) => db.AuditEvents.AsNoTracking().OrderByDescending(e => e.OccurredAt).Take(100).ToListAsync(ct); }


public sealed class ConfigurationStudioApplicationService(EProcurementDbContext db) : IConfigurationStudioApplicationService
{
    public async Task<ConfigurationStudioDto> GetStudioAsync(CancellationToken ct = default) => new(
        await db.BusinessProcessDefinitions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct),
        await db.DocumentRequirementSets.AsNoTracking().Include(x => x.Requirements).OrderBy(x => x.Name).ToListAsync(ct),
        await db.ApprovalMatrices.AsNoTracking().Include(x => x.Steps).OrderBy(x => x.Name).ToListAsync(ct),
        await db.WorkflowMappings.AsNoTracking().OrderBy(x => x.EntityType).ThenBy(x => x.ActionCode).ToListAsync(ct));
    public async Task<BusinessProcessDefinition> CreateBusinessProcessAsync(BusinessProcessDto dto, CancellationToken ct = default) { var item = new BusinessProcessDefinition(dto.Code, dto.Name, dto.Description, dto.EntityType, dto.ActiveWorkflowDefinitionId, dto.ActiveFormDefinitionId, dto.ActiveDocumentRequirementSetId, dto.ActiveApprovalMatrixId, dto.Status); db.BusinessProcessDefinitions.Add(item); await db.SaveChangesAsync(ct); return item; }
    public async Task<BusinessProcessDefinition?> PublishBusinessProcessAsync(string code, CancellationToken ct = default) { var item = await db.BusinessProcessDefinitions.SingleOrDefaultAsync(x => x.Code == code, ct); if (item is null) return null; db.Entry(item).CurrentValues[nameof(BusinessProcessDefinition.Status)] = BusinessProcessStatus.Published; await db.SaveChangesAsync(ct); return item with { Status = BusinessProcessStatus.Published }; }
    public async Task<DocumentRequirementSet> CreateDocumentRequirementSetAsync(DocumentRequirementSetDto dto, CancellationToken ct = default) { var set = new DocumentRequirementSet(dto.Name, dto.Description, dto.EntityType); set.Requirements.AddRange(dto.Requirements.Select(x => new DocumentRequirement(set.Id, x.DocumentType, x.Required, x.MinimumFiles, x.MaximumFiles, x.AllowedExtensions, x.MaximumFileSize, x.RuleCode))); db.DocumentRequirementSets.Add(set); await db.SaveChangesAsync(ct); return set; }
    public async Task<ApprovalMatrix> CreateApprovalMatrixAsync(ApprovalMatrixDto dto, CancellationToken ct = default) { var matrix = new ApprovalMatrix(dto.Name, dto.Description, dto.EntityType); matrix.Steps.AddRange(dto.Steps.Select(x => new ApprovalStep(matrix.Id, x.Role, x.Sequence, x.MinimumAmount, x.MaximumAmount, x.RuleCode))); db.ApprovalMatrices.Add(matrix); await db.SaveChangesAsync(ct); return matrix; }
}

public sealed class PlatformConfigurationApplicationService(EProcurementDbContext db) : IPlatformConfigurationApplicationService
{
    public Task<List<WorkflowMapping>> GetWorkflowMappingsAsync(CancellationToken ct = default) => db.WorkflowMappings.AsNoTracking().OrderBy(x => x.EntityType).ThenBy(x => x.ActionCode).ToListAsync(ct);
    public async Task<WorkflowMapping> CreateWorkflowMappingAsync(WorkflowMappingDto dto, CancellationToken ct = default) { var item = new WorkflowMapping(dto.EntityType, dto.ActionCode, dto.WorkflowCode, dto.IsActive); db.WorkflowMappings.Add(item); await db.SaveChangesAsync(ct); return item; }
    public Task<List<WorkflowTransitionEffect>> GetTransitionEffectsAsync(CancellationToken ct = default) => db.WorkflowTransitionEffects.AsNoTracking().OrderBy(x => x.EntityType).ToListAsync(ct);
    public async Task<WorkflowTransitionEffect> CreateTransitionEffectAsync(CreateTransitionEffectDto dto, CancellationToken ct = default) { var item = new WorkflowTransitionEffect(dto.EntityType, dto.PropertyName, dto.ValueExpression, dto.TriggerTransitionId); db.WorkflowTransitionEffects.Add(item); await db.SaveChangesAsync(ct); return item; }
    public Task<List<DocumentTypeRequirement>> GetDocumentTypeRequirementsAsync(CancellationToken ct = default) => db.DocumentTypeRequirements.AsNoTracking().OrderBy(x => x.EntityType).ThenBy(x => x.DocumentType).ToListAsync(ct);
    public async Task<DocumentTypeRequirement> CreateDocumentTypeRequirementAsync(DocumentTypeRequirementDto dto, CancellationToken ct = default) { var item = new DocumentTypeRequirement(dto.EntityType, dto.DocumentType, dto.Name, dto.IsRequired); db.DocumentTypeRequirements.Add(item); await db.SaveChangesAsync(ct); return item; }
    public Task<List<LookupValue>> GetLookupValuesAsync(string? lookupType = null, CancellationToken ct = default) => db.LookupValues.AsNoTracking().Where(x => lookupType == null || x.LookupType == lookupType).OrderBy(x => x.LookupType).ThenBy(x => x.DisplayOrder).ToListAsync(ct);
    public async Task<LookupValue> CreateLookupValueAsync(LookupValueDto dto, CancellationToken ct = default) { var item = new LookupValue(dto.LookupType, dto.Code, dto.Name, dto.DisplayOrder, dto.IsActive); db.LookupValues.Add(item); await db.SaveChangesAsync(ct); return item; }
    public async Task<SupplierCategory> CreateSupplierCategoryAsync(SupplierCategoryDto dto, CancellationToken ct = default) { var item = new SupplierCategory(dto.Name); db.SupplierCategories.Add(item); await db.SaveChangesAsync(ct); return item; }
}


public sealed record MetadataDefinitionDto(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, string CreatedBy = "system", string? ModifiedBy = null);
public sealed record MetadataDefinitionUpdateDto(string Code, string Name, string Description, int Version, MetadataStatus Status, string? ModifiedBy = null);

public interface IMetadataRepository<TEntity> where TEntity : MetadataEntity
{
    Task<List<TEntity>> ListAsync(CancellationToken ct = default);
    Task<TEntity?> GetAsync(Guid id, CancellationToken ct = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    Task<TEntity?> UpdateAsync(Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class MetadataRepository<TEntity>(EProcurementDbContext db) : IMetadataRepository<TEntity> where TEntity : MetadataEntity
{
    public Task<List<TEntity>> ListAsync(CancellationToken ct = default) => db.Set<TEntity>().AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    public Task<TEntity?> GetAsync(Guid id, CancellationToken ct = default) => db.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default) { db.Set<TEntity>().Add(entity); await db.SaveChangesAsync(ct); return entity; }
    public async Task<TEntity?> UpdateAsync(Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await db.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Code)] = dto.Code;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Name)] = dto.Name;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Description)] = dto.Description;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Version)] = dto.Version;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Status)] = dto.Status;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.Modified)] = DateTimeOffset.UtcNow;
        db.Entry(entity).CurrentValues[nameof(MetadataEntity.ModifiedBy)] = dto.ModifiedBy;
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) { var entity = await db.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return false; db.Remove(entity); await db.SaveChangesAsync(ct); return true; }
}

public interface IMetadataApplicationService
{
    Task<List<MetadataEntity>> ListAsync(string type, CancellationToken ct = default);
    Task<MetadataEntity?> GetAsync(string type, Guid id, CancellationToken ct = default);
    Task<MetadataEntity> CreateAsync(string type, MetadataDefinitionDto dto, CancellationToken ct = default);
    Task<MetadataEntity?> UpdateAsync(string type, Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string type, Guid id, CancellationToken ct = default);
}

public sealed class MetadataApplicationService(EProcurementDbContext db) : IMetadataApplicationService
{
    public async Task<List<MetadataEntity>> ListAsync(string type, CancellationToken ct = default) => Canonical(type) switch
    {
        "applications" => (await Repo<Lca.EProcurement.Domain.Application>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "business-processes" => (await Repo<BusinessProcess>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "entity-definitions" => (await Repo<EntityDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "page-definitions" => (await Repo<PageDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "layout-definitions" => (await Repo<LayoutDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "component-definitions" => (await Repo<ComponentDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "navigation-definitions" => (await Repo<NavigationDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "menu-definitions" => (await Repo<MenuDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "dashboard-definitions" => (await Repo<DashboardDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "report-definitions" => (await Repo<ReportDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "theme-definitions" => (await Repo<ThemeDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "lookup-definitions" => (await Repo<LookupDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "document-type-definitions" => (await Repo<DocumentTypeDefinition>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        "system-settings" => (await Repo<SystemSetting>().ListAsync(ct)).Cast<MetadataEntity>().ToList(),
        _ => throw Unsupported(type)
    };
    public Task<MetadataEntity?> GetAsync(string type, Guid id, CancellationToken ct = default) => Canonical(type) switch
    {
        "applications" => Get<Lca.EProcurement.Domain.Application>(id, ct), "business-processes" => Get<BusinessProcess>(id, ct), "entity-definitions" => Get<EntityDefinition>(id, ct), "page-definitions" => Get<PageDefinition>(id, ct), "layout-definitions" => Get<LayoutDefinition>(id, ct), "component-definitions" => Get<ComponentDefinition>(id, ct), "navigation-definitions" => Get<NavigationDefinition>(id, ct), "menu-definitions" => Get<MenuDefinition>(id, ct), "dashboard-definitions" => Get<DashboardDefinition>(id, ct), "report-definitions" => Get<ReportDefinition>(id, ct), "theme-definitions" => Get<ThemeDefinition>(id, ct), "lookup-definitions" => Get<LookupDefinition>(id, ct), "document-type-definitions" => Get<DocumentTypeDefinition>(id, ct), "system-settings" => Get<SystemSetting>(id, ct), _ => throw Unsupported(type)
    };
    public async Task<MetadataEntity> CreateAsync(string type, MetadataDefinitionDto dto, CancellationToken ct = default) { var entity = Create(type, dto); db.Add(entity); await db.SaveChangesAsync(ct); return entity; }
    public Task<MetadataEntity?> UpdateAsync(string type, Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct = default) => Canonical(type) switch
    {
        "applications" => Update<Lca.EProcurement.Domain.Application>(id, dto, ct), "business-processes" => Update<BusinessProcess>(id, dto, ct), "entity-definitions" => Update<EntityDefinition>(id, dto, ct), "page-definitions" => Update<PageDefinition>(id, dto, ct), "layout-definitions" => Update<LayoutDefinition>(id, dto, ct), "component-definitions" => Update<ComponentDefinition>(id, dto, ct), "navigation-definitions" => Update<NavigationDefinition>(id, dto, ct), "menu-definitions" => Update<MenuDefinition>(id, dto, ct), "dashboard-definitions" => Update<DashboardDefinition>(id, dto, ct), "report-definitions" => Update<ReportDefinition>(id, dto, ct), "theme-definitions" => Update<ThemeDefinition>(id, dto, ct), "lookup-definitions" => Update<LookupDefinition>(id, dto, ct), "document-type-definitions" => Update<DocumentTypeDefinition>(id, dto, ct), "system-settings" => Update<SystemSetting>(id, dto, ct), _ => throw Unsupported(type)
    };
    public Task<bool> DeleteAsync(string type, Guid id, CancellationToken ct = default) => Canonical(type) switch
    {
        "applications" => Repo<Lca.EProcurement.Domain.Application>().DeleteAsync(id, ct), "business-processes" => Repo<BusinessProcess>().DeleteAsync(id, ct), "entity-definitions" => Repo<EntityDefinition>().DeleteAsync(id, ct), "page-definitions" => Repo<PageDefinition>().DeleteAsync(id, ct), "layout-definitions" => Repo<LayoutDefinition>().DeleteAsync(id, ct), "component-definitions" => Repo<ComponentDefinition>().DeleteAsync(id, ct), "navigation-definitions" => Repo<NavigationDefinition>().DeleteAsync(id, ct), "menu-definitions" => Repo<MenuDefinition>().DeleteAsync(id, ct), "dashboard-definitions" => Repo<DashboardDefinition>().DeleteAsync(id, ct), "report-definitions" => Repo<ReportDefinition>().DeleteAsync(id, ct), "theme-definitions" => Repo<ThemeDefinition>().DeleteAsync(id, ct), "lookup-definitions" => Repo<LookupDefinition>().DeleteAsync(id, ct), "document-type-definitions" => Repo<DocumentTypeDefinition>().DeleteAsync(id, ct), "system-settings" => Repo<SystemSetting>().DeleteAsync(id, ct), _ => throw Unsupported(type)
    };
    MetadataRepository<TEntity> Repo<TEntity>() where TEntity : MetadataEntity => new(db);
    async Task<MetadataEntity?> Get<TEntity>(Guid id, CancellationToken ct) where TEntity : MetadataEntity => await Repo<TEntity>().GetAsync(id, ct);
    async Task<MetadataEntity?> Update<TEntity>(Guid id, MetadataDefinitionUpdateDto dto, CancellationToken ct) where TEntity : MetadataEntity => await Repo<TEntity>().UpdateAsync(id, dto, ct);
    static string Canonical(string type) => type.Trim().ToLowerInvariant();
    static NotSupportedException Unsupported(string type) => new($"Metadata type '{type}' is not supported.");
    static MetadataEntity Create(string type, MetadataDefinitionDto d) => Canonical(type) switch
    {
        "applications" => new Lca.EProcurement.Domain.Application(d.Code, d.Name, d.Description, Version: d.Version, Status: d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "business-processes" => new BusinessProcess(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "entity-definitions" => new EntityDefinition(d.Code, d.Name, d.Description, d.Name, d.Name, string.Empty, Version: d.Version, Status: d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "page-definitions" => new PageDefinition(d.Code, d.Name, d.Description, null, PageType.Dashboard, $"/app/{d.Code.ToLowerInvariant()}", "FileText", Version: d.Version, Status: d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "layout-definitions" => new LayoutDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "component-definitions" => new ComponentDefinition(d.Code, d.Name, d.Description, Version: d.Version, Status: d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "navigation-definitions" => new NavigationDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "menu-definitions" => new MenuDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "dashboard-definitions" => new DashboardDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "report-definitions" => new ReportDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "theme-definitions" => new ThemeDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "lookup-definitions" => new LookupDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "document-type-definitions" => new DocumentTypeDefinition(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), "system-settings" => new SystemSetting(d.Code, d.Name, d.Description, d.Version, d.Status, CreatedBy: d.CreatedBy, ModifiedBy: d.ModifiedBy), _ => throw Unsupported(type)
    };
}

public sealed record EntityRecordDto(Guid Id, string EntityCode, Dictionary<string, JsonElement> Values, DateTimeOffset Created, DateTimeOffset? Modified);
public sealed record EntityRecordMutationDto(Dictionary<string, JsonElement> Values, string? Actor = null, List<string>? Roles = null, string? PageCode = null);
public sealed record EntityRecordValidationError(string Field, string Message);
public sealed class EntityRecordValidationException(List<EntityRecordValidationError> errors) : InvalidOperationException("Entity record validation failed.") { public List<EntityRecordValidationError> Errors { get; } = errors; }
public sealed class EntityRecordPermissionException(string operation, string entityCode) : UnauthorizedAccessException($"The current user cannot {operation} records for entity '{entityCode}'.");

public interface IEntityRecordApplicationService
{
    Task<EntityDefinition?> ResolveDefinitionAsync(string entityCode, CancellationToken ct = default);
    Task<List<EntityRecordDto>?> ListAsync(string entityCode, CancellationToken ct = default);
    Task<EntityRecordDto?> GetAsync(string entityCode, Guid id, CancellationToken ct = default);
    Task<EntityRecordDto?> CreateAsync(string entityCode, EntityRecordMutationDto dto, CancellationToken ct = default);
    Task<EntityRecordDto?> UpdateAsync(string entityCode, Guid id, EntityRecordMutationDto dto, CancellationToken ct = default);
    Task<bool?> DeleteAsync(string entityCode, Guid id, EntityRecordMutationDto dto, CancellationToken ct = default);
}

public sealed class EntityRecordApplicationService(EProcurementDbContext db) : IEntityRecordApplicationService
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public Task<EntityDefinition?> ResolveDefinitionAsync(string entityCode, CancellationToken ct = default) { var code = Normalize(entityCode); return db.EntityDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Code == code || x.Code.ToUpper() == code, ct); }
    public async Task<List<EntityRecordDto>?> ListAsync(string entityCode, CancellationToken ct = default)
    {
        var definition = await ResolveDefinitionAsync(entityCode, ct); if (definition is null) return null;
        return (await db.DynamicEntityRecords.AsNoTracking().Where(x => x.EntityDefinitionId == definition.Id).OrderByDescending(x => x.Created).ToListAsync(ct)).Select(ToDto).ToList();
    }
    public async Task<EntityRecordDto?> GetAsync(string entityCode, Guid id, CancellationToken ct = default)
    {
        var definition = await ResolveDefinitionAsync(entityCode, ct); if (definition is null) return null;
        return await db.DynamicEntityRecords.AsNoTracking().SingleOrDefaultAsync(x => x.EntityDefinitionId == definition.Id && x.Id == id, ct) is { } record ? ToDto(record) : null;
    }
    public async Task<EntityRecordDto?> CreateAsync(string entityCode, EntityRecordMutationDto dto, CancellationToken ct = default)
    {
        var definition = await ResolveDefinitionAsync(entityCode, ct); if (definition is null) return null; EnsureCanMutate(definition, "create", dto);
        var values = NormalizePayload(definition, dto.Values, requireAllRequired: true);
        var record = new DynamicEntityRecord(definition.Id, definition.Code, JsonSerializer.Serialize(values, JsonOptions), CreatedBy: dto.Actor ?? "api");
        db.DynamicEntityRecords.Add(record); await db.SaveChangesAsync(ct); return ToDto(record);
    }
    public async Task<EntityRecordDto?> UpdateAsync(string entityCode, Guid id, EntityRecordMutationDto dto, CancellationToken ct = default)
    {
        var definition = await ResolveDefinitionAsync(entityCode, ct); if (definition is null) return null; EnsureCanMutate(definition, "update", dto);
        var record = await db.DynamicEntityRecords.SingleOrDefaultAsync(x => x.EntityDefinitionId == definition.Id && x.Id == id, ct); if (record is null) return null;
        var values = NormalizePayload(definition, dto.Values, requireAllRequired: true);
        db.Entry(record).CurrentValues[nameof(DynamicEntityRecord.DataJson)] = JsonSerializer.Serialize(values, JsonOptions);
        db.Entry(record).CurrentValues[nameof(DynamicEntityRecord.Modified)] = DateTimeOffset.UtcNow;
        db.Entry(record).CurrentValues[nameof(DynamicEntityRecord.ModifiedBy)] = dto.Actor ?? "api";
        await db.SaveChangesAsync(ct); return ToDto(await db.DynamicEntityRecords.AsNoTracking().SingleAsync(x => x.Id == id, ct));
    }
    public async Task<bool?> DeleteAsync(string entityCode, Guid id, EntityRecordMutationDto dto, CancellationToken ct = default)
    {
        var definition = await ResolveDefinitionAsync(entityCode, ct); if (definition is null) return null; EnsureCanMutate(definition, "delete", dto);
        var record = await db.DynamicEntityRecords.SingleOrDefaultAsync(x => x.EntityDefinitionId == definition.Id && x.Id == id, ct); if (record is null) return false;
        db.DynamicEntityRecords.Remove(record); await db.SaveChangesAsync(ct); return true;
    }
    void EnsureCanMutate(EntityDefinition definition, string operation, EntityRecordMutationDto dto)
    {
        var roles = dto.Roles ?? [];
        var pages = db.PageDefinitions.AsNoTracking().ToList().Where(p => PageTargetsEntity(p, definition.Code) && (string.IsNullOrWhiteSpace(dto.PageCode) || p.Code.Equals(dto.PageCode, StringComparison.OrdinalIgnoreCase))).ToList();
        var permissions = pages.SelectMany(p => DeserializeList<PagePermissionMetadata>(p.PermissionsJson)).ToList();
        if (permissions.Count == 0) return;
        if (!permissions.Any(p => roles.Contains(p.Role, StringComparer.OrdinalIgnoreCase) && (p.Access.Equals("Admin", StringComparison.OrdinalIgnoreCase) || p.Access.Equals("Edit", StringComparison.OrdinalIgnoreCase) || p.Access.Equals(operation, StringComparison.OrdinalIgnoreCase)))) throw new EntityRecordPermissionException(operation, definition.Code);
    }
    static bool PageTargetsEntity(PageDefinition page, string entityCode)
    {
        var datasource = Deserialize(page.DatasourceJson, new PageDatasourceMetadata(""));
        return datasource.Entity.Equals(entityCode, StringComparison.OrdinalIgnoreCase) || datasource.Entity.Equals(Normalize(entityCode), StringComparison.OrdinalIgnoreCase);
    }
    static Dictionary<string, JsonElement> NormalizePayload(EntityDefinition definition, Dictionary<string, JsonElement>? values, bool requireAllRequired)
    {
        var supplied = values ?? []; var errors = new List<EntityRecordValidationError>(); var result = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        var properties = DeserializeList<EntityPropertyMetadata>(definition.PropertiesJson);
        foreach (var property in properties)
        {
            if (!supplied.TryGetValue(property.Code, out var value))
            {
                if (property.Required && requireAllRequired) errors.Add(new(property.Code, $"{property.Name} is required."));
                continue;
            }
            if (IsBlank(value) && property.Required) errors.Add(new(property.Code, $"{property.Name} is required."));
            if (!IsBlank(value) && !MatchesType(value, property.DataType)) errors.Add(new(property.Code, $"{property.Name} must be a {property.DataType} value."));
            result[property.Code] = value;
        }
        foreach (var validation in DeserializeList<EntityValidationMetadata>(definition.ValidationsJson))
            if (validation.Expression.StartsWith("required:", StringComparison.OrdinalIgnoreCase) && !result.ContainsKey(validation.Expression[9..])) errors.Add(new(validation.Expression[9..], validation.Message));
        if (errors.Count > 0) throw new EntityRecordValidationException(errors);
        return result;
    }
    static bool IsBlank(JsonElement value) => value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined || (value.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(value.GetString()));
    static bool MatchesType(JsonElement value, string dataType) => dataType.ToLowerInvariant() switch { "string" or "text" or "email" => value.ValueKind == JsonValueKind.String, "number" or "decimal" => value.ValueKind == JsonValueKind.Number, "integer" or "int" => value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out _), "boolean" or "bool" => value.ValueKind is JsonValueKind.True or JsonValueKind.False, "date" or "datetime" => value.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(value.GetString(), out _), _ => true };
    static EntityRecordDto ToDto(DynamicEntityRecord r) => new(r.Id, r.EntityCode, Deserialize(r.DataJson, new Dictionary<string, JsonElement>()), r.Created, r.Modified);
    static string Normalize(string code) => code.Trim().ToUpperInvariant();
    static T Deserialize<T>(string json, T fallback) => string.IsNullOrWhiteSpace(json) ? fallback : JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback;
    static List<T> DeserializeList<T>(string json) => string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
    sealed record EntityPropertyMetadata(string Code, string Name, string DataType, bool Required = false, bool Searchable = false, string? DefaultValue = null);
    sealed record EntityValidationMetadata(string Code, string Name, string Expression, string Message);
    sealed record PageDatasourceMetadata(string Entity, string Mode = "Metadata", string? Endpoint = null, string? KeyField = null);
    sealed record PagePermissionMetadata(string Role, string Access = "View");
}

public interface IAnnualProcurementPlanApplicationService
{
    Task<List<AnnualProcurementPlan>> GetAsync(CancellationToken ct = default);
    Task<AnnualProcurementPlan?> GetAsync(Guid id, CancellationToken ct = default);
    Task<AnnualProcurementPlan> CreateAsync(CreateAnnualProcurementPlanDto dto, CancellationToken ct = default);
    Task<AnnualProcurementPlan?> SubmitAsync(Guid id, string actor, CancellationToken ct = default);
    Task<AnnualProcurementPlan?> ApproveAsync(Guid id, string actor, CancellationToken ct = default);
    Task<PlanningDashboardDto> DashboardAsync(CancellationToken ct = default);
}
public sealed record CreateAnnualProcurementPlanDto(string PlanNumber, string Title, Guid FinancialYearId, string Department, string CreatedBy, List<CreateProcurementPlanItemDto>? Items = null);
public sealed record CreateProcurementPlanItemDto(string ItemCode, string Description, Guid ProcurementCategoryId, decimal EstimatedAmount, string PlannedQuarter, string ProcurementMethod);
public sealed record PlanningDashboardDto(int Plans, int DraftPlans, int SubmittedPlans, int ApprovedPlans, decimal TotalPlannedValue, decimal TotalBudgetValue, decimal AvailableBudget);
public interface IBudgetApplicationService
{
    Task<List<Budget>> GetAsync(CancellationToken ct = default);
    Task<Budget?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Budget> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default);
}
public interface ICostCentreApplicationService
{
    Task<List<CostCentre>> GetAsync(CancellationToken ct = default);
    Task<CostCentre> CreateAsync(CreateCostCentreDto dto, CancellationToken ct = default);
}
public interface IProcurementCategoryApplicationService
{
    Task<List<ProcurementCategory>> GetAsync(CancellationToken ct = default);
    Task<ProcurementCategory> CreateAsync(CreateProcurementCategoryDto dto, CancellationToken ct = default);
}
public interface IFinancialYearApplicationService
{
    Task<List<FinancialYear>> GetAsync(CancellationToken ct = default);
}
public sealed record CreateBudgetDto(Guid FinancialYearId, string Department, decimal TotalAmount, decimal CommittedAmount, List<CreateBudgetLineDto>? Lines = null);
public sealed record CreateBudgetLineDto(Guid CostCentreId, Guid ProcurementCategoryId, decimal AllocatedAmount, decimal CommittedAmount);
public sealed record CreateCostCentreDto(string Code, string Name, string Department);
public sealed record CreateProcurementCategoryDto(string Code, string Name);

public sealed class AnnualProcurementPlanApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows) : IAnnualProcurementPlanApplicationService
{
    const string WorkflowCode = "ANNUAL-PROCUREMENT-PLAN";
    public Task<List<AnnualProcurementPlan>> GetAsync(CancellationToken ct = default) => db.AnnualProcurementPlans.AsNoTracking().Include(x => x.Items).OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    public Task<AnnualProcurementPlan?> GetAsync(Guid id, CancellationToken ct = default) => db.AnnualProcurementPlans.AsNoTracking().Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<AnnualProcurementPlan> CreateAsync(CreateAnnualProcurementPlanDto dto, CancellationToken ct = default)
    {
        var plan = new AnnualProcurementPlan(dto.PlanNumber, dto.Title, dto.FinancialYearId, dto.Department, "Draft", dto.CreatedBy, DateTimeOffset.UtcNow);
        foreach (var item in dto.Items ?? []) plan.Items.Add(new ProcurementPlanItem(plan.Id, item.ItemCode, item.Description, item.ProcurementCategoryId, item.EstimatedAmount, item.PlannedQuarter, item.ProcurementMethod, "Draft"));
        db.AnnualProcurementPlans.Add(plan);
        db.AuditEvents.Add(new AuditEvent("Annual procurement plan created", nameof(AnnualProcurementPlan), plan.Id, plan.PlanNumber, dto.CreatedBy, "Created through planning module", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return plan;
    }
    public async Task<AnnualProcurementPlan?> SubmitAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var plan = await db.AnnualProcurementPlans.SingleOrDefaultAsync(x => x.Id == id, ct); if (plan is null) return null;
        var instance = await db.WorkflowInstances.Where(x => x.EntityType == nameof(AnnualProcurementPlan) && x.EntityId == id && x.Status == WorkflowInstanceStatus.Running).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct);
        if (instance is null) instance = await workflows.StartAsync(WorkflowCode, nameof(AnnualProcurementPlan), id, actor, ct);
        if (instance.CurrentNodeCode == "Draft") await workflows.ExecuteActionAsync(instance.Id, "Submit", actor, ct);
        db.Entry(plan).CurrentValues[nameof(AnnualProcurementPlan.Status)] = "Submitted"; db.Entry(plan).CurrentValues[nameof(AnnualProcurementPlan.SubmittedAt)] = DateTimeOffset.UtcNow;
        db.AuditEvents.Add(new AuditEvent("Annual procurement plan submitted", nameof(AnnualProcurementPlan), plan.Id, plan.PlanNumber, actor, WorkflowCode, DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return plan with { Status = "Submitted", SubmittedAt = DateTimeOffset.UtcNow };
    }
    public async Task<AnnualProcurementPlan?> ApproveAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var plan = await db.AnnualProcurementPlans.SingleOrDefaultAsync(x => x.Id == id, ct); if (plan is null) return null;
        var instance = await db.WorkflowInstances.Where(x => x.EntityType == nameof(AnnualProcurementPlan) && x.EntityId == id && x.Status == WorkflowInstanceStatus.Running).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct);
        if (instance is null) { instance = await workflows.StartAsync(WorkflowCode, nameof(AnnualProcurementPlan), id, actor, ct); await workflows.ExecuteActionAsync(instance.Id, "Submit", actor, ct); instance = await db.WorkflowInstances.SingleAsync(x => x.Id == instance.Id, ct); }
        foreach (var action in new[] { "ProcurementReview", "FinanceReview", "Approve" }) { instance = await db.WorkflowInstances.SingleAsync(x => x.Id == instance.Id, ct); if (instance.Status == WorkflowInstanceStatus.Running) await workflows.ExecuteActionAsync(instance.Id, action, actor, ct); }
        db.Entry(plan).CurrentValues[nameof(AnnualProcurementPlan.Status)] = "Approved"; db.Entry(plan).CurrentValues[nameof(AnnualProcurementPlan.ApprovedAt)] = DateTimeOffset.UtcNow;
        db.AuditEvents.Add(new AuditEvent("Annual procurement plan approved", nameof(AnnualProcurementPlan), plan.Id, plan.PlanNumber, actor, WorkflowCode, DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return plan with { Status = "Approved", ApprovedAt = DateTimeOffset.UtcNow };
    }
    public async Task<PlanningDashboardDto> DashboardAsync(CancellationToken ct = default) => new(await db.AnnualProcurementPlans.CountAsync(ct), await db.AnnualProcurementPlans.CountAsync(x => x.Status == "Draft", ct), await db.AnnualProcurementPlans.CountAsync(x => x.Status == "Submitted", ct), await db.AnnualProcurementPlans.CountAsync(x => x.Status == "Approved", ct), await db.ProcurementPlanItems.SumAsync(x => (decimal?)x.EstimatedAmount, ct) ?? 0, await db.Budgets.SumAsync(x => (decimal?)x.TotalAmount, ct) ?? 0, await db.Budgets.SumAsync(x => (decimal?)x.AvailableAmount, ct) ?? 0);
}
public sealed class BudgetApplicationService(EProcurementDbContext db) : IBudgetApplicationService
{
    public Task<List<Budget>> GetAsync(CancellationToken ct = default) => db.Budgets.AsNoTracking().Include(x => x.Lines).OrderBy(x => x.Department).ToListAsync(ct);
    public Task<Budget?> GetAsync(Guid id, CancellationToken ct = default) => db.Budgets.AsNoTracking().Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<Budget> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default) { var budget = new Budget(dto.FinancialYearId, dto.Department, dto.TotalAmount, dto.CommittedAmount, dto.TotalAmount - dto.CommittedAmount); foreach (var l in dto.Lines ?? []) budget.Lines.Add(new BudgetLine(budget.Id, l.CostCentreId, l.ProcurementCategoryId, l.AllocatedAmount, l.CommittedAmount, l.AllocatedAmount - l.CommittedAmount)); db.Budgets.Add(budget); db.AuditEvents.Add(new AuditEvent("Budget created", nameof(Budget), budget.Id, dto.Department, "system", "Budget foundation", DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return budget; }
}
public sealed class CostCentreApplicationService(EProcurementDbContext db) : ICostCentreApplicationService
{
    public Task<List<CostCentre>> GetAsync(CancellationToken ct = default) => db.CostCentres.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    public async Task<CostCentre> CreateAsync(CreateCostCentreDto dto, CancellationToken ct = default) { var c = new CostCentre(dto.Code, dto.Name, dto.Department); db.CostCentres.Add(c); db.AuditEvents.Add(new AuditEvent("Cost centre created", nameof(CostCentre), c.Id, c.Code, "system", c.Department, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return c; }
}
public sealed class ProcurementCategoryApplicationService(EProcurementDbContext db) : IProcurementCategoryApplicationService
{
    public Task<List<ProcurementCategory>> GetAsync(CancellationToken ct = default) => db.ProcurementCategories.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    public async Task<ProcurementCategory> CreateAsync(CreateProcurementCategoryDto dto, CancellationToken ct = default) { var c = new ProcurementCategory(dto.Code, dto.Name); db.ProcurementCategories.Add(c); db.AuditEvents.Add(new AuditEvent("Procurement category created", nameof(ProcurementCategory), c.Id, c.Code, "system", c.Name, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return c; }
}
public sealed class FinancialYearApplicationService(EProcurementDbContext db) : IFinancialYearApplicationService
{
    public Task<List<FinancialYear>> GetAsync(CancellationToken ct = default) => db.FinancialYears.AsNoTracking().OrderByDescending(x => x.StartDate).ToListAsync(ct);
}

public interface IRequisitionApplicationService
{
    Task<List<Requisition>> GetAsync(CancellationToken ct = default);
    Task<Requisition?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Requisition> CreateAsync(CreateRequisitionDto dto, CancellationToken ct = default);
    Task<Requisition?> SubmitAsync(Guid id, string actor, CancellationToken ct = default);
    Task<Requisition?> ApproveAsync(Guid id, string actor, CancellationToken ct = default);
    Task<Requisition?> RejectAsync(Guid id, string actor, string reason, CancellationToken ct = default);
    Task<BudgetValidationDto> ValidateBudgetAsync(Guid id, CancellationToken ct = default);
}
public sealed record CreateRequisitionDto(string RequisitionNumber, string Title, string Description, string Department, Guid CostCentreId, Guid FinancialYearId, string RequestedBy, DateTimeOffset RequiredDate, string Priority, List<CreateRequisitionItemDto> Items);
public sealed record CreateRequisitionItemDto(string Description, decimal Quantity, string UnitOfMeasure, decimal EstimatedUnitPrice, Guid ProcurementCategoryId, Guid? ProcurementPlanItemId = null);
public sealed record BudgetValidationDto(bool BudgetExists, bool HasItems, bool ItemsHaveEstimates, bool BudgetAvailable, decimal RequiredAmount, decimal AvailableAmount, List<string> Messages);

public sealed class RequisitionApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IRequisitionApplicationService
{
    const string WorkflowCode = "REQUISITION-APPROVAL-WORKFLOW";
    public Task<List<Requisition>> GetAsync(CancellationToken ct = default) => db.Requisitions.AsNoTracking().Include(x => x.Items).Include(x => x.Attachments).Include(x => x.StatusHistory).OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    public Task<Requisition?> GetAsync(Guid id, CancellationToken ct = default) => db.Requisitions.AsNoTracking().Include(x => x.Items).Include(x => x.Attachments).Include(x => x.StatusHistory).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<Requisition> CreateAsync(CreateRequisitionDto dto, CancellationToken ct = default)
    {
        var total = dto.Items.Sum(i => i.Quantity * i.EstimatedUnitPrice);
        var req = new Requisition(dto.RequisitionNumber, dto.Title, dto.Description, dto.Department, dto.CostCentreId, dto.FinancialYearId, dto.RequestedBy, dto.RequiredDate, dto.Priority, total, RequisitionStatus.Draft, DateTimeOffset.UtcNow);
        foreach (var i in dto.Items) req.Items.Add(new RequisitionItem(req.Id, i.Description, i.Quantity, i.UnitOfMeasure, i.EstimatedUnitPrice, i.Quantity * i.EstimatedUnitPrice, i.ProcurementCategoryId, i.ProcurementPlanItemId));
        req.StatusHistory.Add(new RequisitionStatusHistory(req.Id, RequisitionStatus.Draft, RequisitionStatus.Draft, dto.RequestedBy, "Requisition created", DateTimeOffset.UtcNow));
        db.Requisitions.Add(req); db.AuditEvents.Add(new AuditEvent("Requisition created", nameof(Requisition), req.Id, req.RequisitionNumber, dto.RequestedBy, "Internal requisition created", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return req;
    }
    public async Task<BudgetValidationDto> ValidateBudgetAsync(Guid id, CancellationToken ct = default)
    {
        var req = await db.Requisitions.AsNoTracking().Include(x => x.Items).SingleAsync(x => x.Id == id, ct);
        var lineChecks = new List<decimal>();
        foreach (var item in req.Items)
            lineChecks.Add(await db.BudgetLines.Where(l => l.CostCentreId == req.CostCentreId && l.ProcurementCategoryId == item.ProcurementCategoryId && db.Budgets.Any(b => b.Id == l.BudgetId && b.FinancialYearId == req.FinancialYearId)).SumAsync(l => (decimal?)l.AvailableAmount, ct) ?? 0);
        var available = lineChecks.Sum(); var messages = new List<string>();
        var exists = req.Items.Any() && lineChecks.All(x => x > 0); if (!exists) messages.Add("No matching budget lines exist.");
        var hasItems = req.Items.Any(); if (!hasItems) messages.Add("Requisition must have items.");
        var estimates = req.Items.All(i => i.Quantity > 0 && i.EstimatedUnitPrice > 0 && i.EstimatedTotal > 0); if (!estimates) messages.Add("All items must have positive estimates.");
        var budgetAvailable = available >= req.EstimatedTotal; if (!budgetAvailable) messages.Add("Available budget is insufficient.");
        return new(exists, hasItems, estimates, budgetAvailable, req.EstimatedTotal, available, messages);
    }
    public async Task<Requisition?> SubmitAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var req = await db.Requisitions.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id, ct); if (req is null) return null;
        foreach (var result in await rules.EvaluatePublishedAsync(nameof(Requisition), nameof(Requisition), id, actor, null, ct)) if (!result.Passed) throw new InvalidOperationException(result.Message);
        var validation = await ValidateBudgetAsync(id, ct); if (!validation.HasItems || !validation.ItemsHaveEstimates || !validation.BudgetExists) throw new InvalidOperationException(string.Join(" ", validation.Messages));
        var instance = await db.WorkflowInstances.Where(x => x.EntityType == nameof(Requisition) && x.EntityId == id && x.Status == WorkflowInstanceStatus.Running).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct) ?? await workflows.StartAsync(WorkflowCode, nameof(Requisition), id, actor, ct);
        if (instance.CurrentNodeCode == "Draft") await workflows.ExecuteActionAsync(instance.Id, "Submit", actor, ct);
        Change(req, RequisitionStatus.Submitted, actor, "Submitted for approval"); db.Entry(req).CurrentValues[nameof(Requisition.SubmittedAt)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct); return req with { Status = RequisitionStatus.Submitted, SubmittedAt = DateTimeOffset.UtcNow };
    }
    public async Task<Requisition?> ApproveAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var req = await db.Requisitions.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id, ct); if (req is null) return null;
        var validation = await ValidateBudgetAsync(id, ct); if (!validation.BudgetAvailable) throw new InvalidOperationException(string.Join(" ", validation.Messages));
        var instance = await db.WorkflowInstances.Where(x => x.EntityType == nameof(Requisition) && x.EntityId == id && x.Status == WorkflowInstanceStatus.Running).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct) ?? await workflows.StartAsync(WorkflowCode, nameof(Requisition), id, actor, ct);
        foreach (var action in new[] { "Submit", "ValidateBudget", "ManagerApprove", "ProcurementReview", "Approve" }) { instance = await db.WorkflowInstances.SingleAsync(x => x.Id == instance.Id, ct); if (instance.Status == WorkflowInstanceStatus.Running && db.WorkflowTransitions.Any(t => t.WorkflowVersionId == instance.WorkflowVersionId && t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == action)) await workflows.ExecuteActionAsync(instance.Id, action, actor, ct); }
        foreach (var item in req.Items) { var line = await db.BudgetLines.Where(l => l.CostCentreId == req.CostCentreId && l.ProcurementCategoryId == item.ProcurementCategoryId).OrderByDescending(l => l.AvailableAmount).FirstAsync(ct); var budget = await db.Budgets.SingleAsync(b => b.Id == line.BudgetId, ct); db.Entry(line).CurrentValues[nameof(BudgetLine.CommittedAmount)] = line.CommittedAmount + item.EstimatedTotal; db.Entry(line).CurrentValues[nameof(BudgetLine.AvailableAmount)] = line.AvailableAmount - item.EstimatedTotal; db.Entry(budget).CurrentValues[nameof(Budget.CommittedAmount)] = budget.CommittedAmount + item.EstimatedTotal; db.Entry(budget).CurrentValues[nameof(Budget.AvailableAmount)] = budget.AvailableAmount - item.EstimatedTotal; db.BudgetCommitments.Add(new BudgetCommitment(req.Id, budget.Id, line.Id, req.FinancialYearId, req.CostCentreId, item.ProcurementCategoryId, item.EstimatedTotal, actor, DateTimeOffset.UtcNow, req.RequisitionNumber)); }
        Change(req, RequisitionStatus.Approved, actor, "Approved and budget committed"); db.Entry(req).CurrentValues[nameof(Requisition.ApprovedAt)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct); return req with { Status = RequisitionStatus.Approved, ApprovedAt = DateTimeOffset.UtcNow };
    }
    public async Task<Requisition?> RejectAsync(Guid id, string actor, string reason, CancellationToken ct = default) { var req = await db.Requisitions.SingleOrDefaultAsync(x => x.Id == id, ct); if (req is null) return null; Change(req, RequisitionStatus.Rejected, actor, reason); db.Entry(req).CurrentValues[nameof(Requisition.RejectedAt)] = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return req with { Status = RequisitionStatus.Rejected, RejectedAt = DateTimeOffset.UtcNow }; }
    void Change(Requisition req, RequisitionStatus status, string actor, string notes) { var from = req.Status; db.Entry(req).CurrentValues[nameof(Requisition.Status)] = status; db.RequisitionStatusHistories.Add(new RequisitionStatusHistory(req.Id, from, status, actor, notes, DateTimeOffset.UtcNow)); db.AuditEvents.Add(new AuditEvent($"Requisition {status}", nameof(Requisition), req.Id, req.RequisitionNumber, actor, notes, DateTimeOffset.UtcNow)); }
}

public interface IBidSubmissionApplicationService
{
    Task<List<BidSubmission>> GetAsync(CancellationToken ct = default);
    Task<BidSubmission?> GetAsync(Guid id, CancellationToken ct = default);
    Task<BidSubmission> CreateAsync(CreateBidSubmissionDto dto, CancellationToken ct = default);
    Task<BidSubmission?> UpdateAsync(Guid id, UpdateBidSubmissionDto dto, CancellationToken ct = default);
    Task<BidSubmission?> SubmitAsync(Guid id, string actor, CancellationToken ct = default);
    Task<BidSubmission?> WithdrawAsync(Guid id, string actor, CancellationToken ct = default);
    Task<List<BidSubmissionDocument>> GetDocumentsAsync(Guid id, CancellationToken ct = default);
    Task<BidSubmissionDocument?> AddDocumentAsync(Guid id, UploadBidDocumentDto dto, CancellationToken ct = default);
    Task<List<BidSubmissionHistory>> GetHistoryAsync(Guid id, CancellationToken ct = default);
}
public sealed record CreateBidSubmissionDto(string SubmissionNumber, Guid TenderId, Guid SupplierId, string SubmittedBy, List<CreateBidSubmissionItemDto>? Items = null, List<CreateBidSubmissionDeclarationDto>? Declarations = null);
public sealed record UpdateBidSubmissionDto(List<CreateBidSubmissionItemDto> Items, List<CreateBidSubmissionDeclarationDto> Declarations);
public sealed record CreateBidSubmissionItemDto(Guid? TenderLotId, string Description, decimal Quantity, decimal UnitPrice, string? Notes = null);
public sealed record CreateBidSubmissionDeclarationDto(string DeclarationType, bool Accepted, string AcceptedBy);
public sealed record UploadBidDocumentDto(string DocumentType, string Filename, string StorageReference, string UploadedBy);

public sealed class BidSubmissionApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IBidSubmissionApplicationService
{
    const string WorkflowCode = "BID-SUBMISSION-WORKFLOW";
    public Task<List<BidSubmission>> GetAsync(CancellationToken ct = default) => Include(db.BidSubmissions.AsNoTracking()).OrderByDescending(x => x.SubmissionDate ?? DateTimeOffset.MinValue).ToListAsync(ct);
    public Task<BidSubmission?> GetAsync(Guid id, CancellationToken ct = default) => Include(db.BidSubmissions.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<BidSubmission> CreateAsync(CreateBidSubmissionDto dto, CancellationToken ct = default)
    {
        var bid = new BidSubmission(dto.SubmissionNumber, dto.TenderId, dto.SupplierId, BidSubmissionStatus.Draft, DateTimeOffset.UtcNow, dto.SubmittedBy);
        ApplyItems(bid, dto.Items ?? []);
        foreach (var d in dto.Declarations ?? []) bid.Declarations.Add(new BidSubmissionDeclaration(bid.Id, d.DeclarationType, d.Accepted, d.AcceptedBy, DateTimeOffset.UtcNow));
        bid.Versions.Add(new BidSubmissionVersion(bid.Id, 1, DateTimeOffset.UtcNow, dto.SubmittedBy));
        AddHistory(bid, "Submission created", dto.SubmittedBy, "Draft bid submission created.");
        db.BidSubmissions.Add(bid); db.AuditEvents.Add(new AuditEvent("Submission created", nameof(BidSubmission), bid.Id, bid.SubmissionNumber, dto.SubmittedBy, "Draft bid submission created", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return bid;
    }
    public async Task<BidSubmission?> UpdateAsync(Guid id, UpdateBidSubmissionDto dto, CancellationToken ct = default)
    {
        var bid = await Include(db.BidSubmissions).SingleOrDefaultAsync(x => x.Id == id, ct); if (bid is null) return null;
        EnsureEditable(bid);
        db.BidSubmissionItems.RemoveRange(bid.Items); db.BidSubmissionDeclarations.RemoveRange(bid.Declarations); await db.SaveChangesAsync(ct);
        ApplyItems(bid, dto.Items); foreach (var d in dto.Declarations) bid.Declarations.Add(new BidSubmissionDeclaration(bid.Id, d.DeclarationType, d.Accepted, d.AcceptedBy, DateTimeOffset.UtcNow));
        AddHistory(bid, "Submission updated", bid.SubmittedBy, "Draft bid submission updated."); db.AuditEvents.Add(new AuditEvent("Submission updated", nameof(BidSubmission), bid.Id, bid.SubmissionNumber, bid.SubmittedBy, "Bid submission updated", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return bid;
    }
    public async Task<BidSubmission?> SubmitAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var bid = await Include(db.BidSubmissions).SingleOrDefaultAsync(x => x.Id == id, ct); if (bid is null) return null; EnsureEditable(bid);
        await ValidateRequiredDocuments(bid, ct);
        foreach (var result in await rules.EvaluatePublishedAsync(nameof(BidSubmission), nameof(BidSubmission), id, actor, await RuleValues(bid, ct), ct)) if (!result.Passed) throw new InvalidOperationException(result.Message);
        var instance = await db.WorkflowInstances.Where(x => x.EntityType == nameof(BidSubmission) && x.EntityId == id && x.Status == WorkflowInstanceStatus.Running).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct) ?? await workflows.StartAsync(WorkflowCode, nameof(BidSubmission), id, actor, ct);
        foreach (var action in new[] { "MarkReady", "Submit", "Lock" }) { instance = await db.WorkflowInstances.SingleAsync(x => x.Id == instance.Id, ct); if (await db.WorkflowTransitions.AnyAsync(t => t.WorkflowVersionId == instance.WorkflowVersionId && t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == action, ct)) await workflows.ExecuteActionAsync(instance.Id, action, actor, ct); }
        Change(bid, BidSubmissionStatus.Locked, actor, "Submission validated, submitted and locked."); db.Entry(bid).CurrentValues[nameof(BidSubmission.SubmittedAt)] = DateTimeOffset.UtcNow; db.Entry(bid).CurrentValues[nameof(BidSubmission.LockedAt)] = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct); return bid with { Status = BidSubmissionStatus.Locked, SubmittedAt = DateTimeOffset.UtcNow, LockedAt = DateTimeOffset.UtcNow };
    }
    public async Task<BidSubmission?> WithdrawAsync(Guid id, string actor, CancellationToken ct = default) { var bid = await db.BidSubmissions.SingleOrDefaultAsync(x => x.Id == id, ct); if (bid is null) return null; Change(bid, BidSubmissionStatus.Withdrawn, actor, "Submission withdrawn by supplier."); db.Entry(bid).CurrentValues[nameof(BidSubmission.WithdrawnAt)] = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return bid with { Status = BidSubmissionStatus.Withdrawn, WithdrawnAt = DateTimeOffset.UtcNow }; }
    public Task<List<BidSubmissionDocument>> GetDocumentsAsync(Guid id, CancellationToken ct = default) => db.BidSubmissionDocuments.AsNoTracking().Where(x => x.BidSubmissionId == id).OrderBy(x => x.DocumentType).ToListAsync(ct);
    public async Task<BidSubmissionDocument?> AddDocumentAsync(Guid id, UploadBidDocumentDto dto, CancellationToken ct = default) { var bid = await db.BidSubmissions.SingleOrDefaultAsync(x => x.Id == id, ct); if (bid is null) return null; EnsureEditable(bid); var version = await db.BidSubmissionDocuments.CountAsync(x => x.BidSubmissionId == id && x.DocumentType == dto.DocumentType, ct) + 1; var doc = new BidSubmissionDocument(id, dto.DocumentType, dto.Filename, dto.StorageReference, dto.UploadedBy, DateTimeOffset.UtcNow, version); db.BidSubmissionDocuments.Add(doc); AddHistory(bid, "Document uploaded", dto.UploadedBy, dto.DocumentType); db.AuditEvents.Add(new AuditEvent("Document uploaded", nameof(BidSubmission), id, bid.SubmissionNumber, dto.UploadedBy, dto.DocumentType, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return doc; }
    public Task<List<BidSubmissionHistory>> GetHistoryAsync(Guid id, CancellationToken ct = default) => db.BidSubmissionHistories.AsNoTracking().Where(x => x.BidSubmissionId == id).OrderByDescending(x => x.OccurredAt).ToListAsync(ct);
    static IQueryable<BidSubmission> Include(IQueryable<BidSubmission> q) => q.Include(x => x.Items).Include(x => x.Documents).Include(x => x.History).Include(x => x.Declarations).Include(x => x.Versions).Include(x => x.StatusHistory);
    static void EnsureEditable(BidSubmission bid) { if (bid.Status is BidSubmissionStatus.Locked or BidSubmissionStatus.Submitted or BidSubmissionStatus.Opened or BidSubmissionStatus.Evaluated or BidSubmissionStatus.Awarded or BidSubmissionStatus.Rejected) throw new InvalidOperationException("Locked bid submissions cannot be edited."); }
    static void ApplyItems(BidSubmission bid, IEnumerable<CreateBidSubmissionItemDto> items) { foreach (var i in items) bid.Items.Add(new BidSubmissionItem(bid.Id, i.TenderLotId, i.Description, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice, i.Notes)); }
    void AddHistory(BidSubmission bid, string type, string actor, string details) => db.BidSubmissionHistories.Add(new BidSubmissionHistory(bid.Id, type, actor, details, DateTimeOffset.UtcNow));
    void Change(BidSubmission bid, BidSubmissionStatus status, string actor, string notes) { var from = bid.Status; db.Entry(bid).CurrentValues[nameof(BidSubmission.Status)] = status; db.BidSubmissionStatusHistories.Add(new BidSubmissionStatusHistory(bid.Id, from, status, actor, DateTimeOffset.UtcNow, notes)); AddHistory(bid, $"Submission {status}", actor, notes); db.AuditEvents.Add(new AuditEvent($"Submission {status}", nameof(BidSubmission), bid.Id, bid.SubmissionNumber, actor, notes, DateTimeOffset.UtcNow)); }
    async Task ValidateRequiredDocuments(BidSubmission bid, CancellationToken ct) { var set = await db.BusinessProcessDefinitions.AsNoTracking().Where(p => p.Code == "BID-SUBMISSION").Join(db.DocumentRequirementSets.Include(x => x.Requirements), p => p.ActiveDocumentRequirementSetId, s => s.Id, (p, s) => s).SingleAsync(ct); var missing = set.Requirements.Where(r => r.Required && bid.Documents.Count(d => d.DocumentType == r.DocumentType) < r.MinimumFiles).Select(r => r.DocumentType).ToList(); if (missing.Any()) throw new InvalidOperationException("Missing required bid documents: " + string.Join(", ", missing)); }
    async Task<Dictionary<string, string?>> RuleValues(BidSubmission bid, CancellationToken ct) { var tender = await db.Tenders.AsNoTracking().SingleAsync(x => x.Id == bid.TenderId, ct); var supplier = await db.Suppliers.AsNoTracking().SingleAsync(x => x.Id == bid.SupplierId, ct); var set = await db.BusinessProcessDefinitions.AsNoTracking().Where(p => p.Code == "BID-SUBMISSION").Join(db.DocumentRequirementSets.Include(x => x.Requirements), p => p.ActiveDocumentRequirementSetId, s => s.Id, (p, s) => s).SingleAsync(ct); return new() { ["TenderStatus"] = tender.Status.ToString(), ["TenderClosingDate"] = tender.ClosingDate.ToString("O"), ["SupplierStatus"] = supplier.Status.ToString(), ["RequiredDocumentsUploaded"] = set.Requirements.Where(r => r.Required).All(r => bid.Documents.Count(d => d.DocumentType == r.DocumentType) >= r.MinimumFiles).ToString().ToLowerInvariant() }; }
}


public interface IBidOpeningApplicationService
{
    Task<List<BidOpeningSummaryDto>> GetAsync(CancellationToken ct = default);
    Task<BidOpeningDetailDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<BidOpeningDetailDto> CreateAsync(CreateBidOpeningSessionDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> ScheduleAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> StartAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> OpenSubmissionAsync(Guid id, Guid bidSubmissionId, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> CompleteAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> ReferToEvaluationAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<BidOpeningDetailDto?> CancelAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default);
    Task<List<BidOpeningSubmission>> GetSubmissionsAsync(Guid id, CancellationToken ct = default);
    Task<List<BidOpeningMinute>> GetMinutesAsync(Guid id, CancellationToken ct = default);
    Task<BidOpeningMinute?> AddMinuteAsync(Guid id, AddBidOpeningMinuteDto dto, CancellationToken ct = default);
    Task<BidOpeningReport?> GetReportAsync(Guid id, CancellationToken ct = default);
}
public sealed record BidOpeningActorDto(string Actor);
public sealed record CreateBidOpeningCommitteeMemberDto(string Name, string Email, string Role, bool AttendanceConfirmed = false);
public sealed record CreateBidOpeningSessionDto(Guid TenderId, string Title, DateTimeOffset ScheduledAt, string CreatedBy, string Chairperson, string? Notes = null, List<CreateBidOpeningCommitteeMemberDto>? CommitteeMembers = null);
public sealed record AddBidOpeningMinuteDto(string MinuteText, string RecordedBy);
public sealed record BidOpeningSummaryDto(Guid Id, string SessionNumber, Guid TenderId, string TenderNumber, string TenderTitle, DateTimeOffset ScheduledAt, string Status, string Chairperson, int SubmissionCount);
public sealed record BidOpeningDetailDto(BidOpeningSession Session, Tender Tender, List<BidOpeningCommitteeMember> Committee, List<BidOpeningChecklistItem> Checklist, List<BidOpeningSubmission> Submissions, List<BidOpeningMinute> Minutes, List<BidOpeningReport> Reports, List<AuditEvent> AuditTimeline, WorkflowInstance? WorkflowInstance);

public sealed class BidOpeningApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IBidOpeningApplicationService
{
    const string WorkflowCode = "BID-OPENING-WORKFLOW";
    public Task<List<BidOpeningSummaryDto>> GetAsync(CancellationToken ct = default) => db.BidOpeningSessions.AsNoTracking().GroupJoin(db.Tenders.AsNoTracking(), s => s.TenderId, t => t.Id, (s, ts) => new { s, t = ts.First() }).Select(x => new BidOpeningSummaryDto(x.s.Id, x.s.SessionNumber, x.s.TenderId, x.t.TenderNumber, x.t.Title, x.s.ScheduledAt, x.s.Status.ToString(), x.s.Chairperson, x.s.Submissions.Count)).OrderByDescending(x => x.ScheduledAt).ToListAsync(ct);
    public async Task<BidOpeningDetailDto?> GetAsync(Guid id, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct); return s is null ? null : await Detail(s, ct); }
    public async Task<BidOpeningDetailDto> CreateAsync(CreateBidOpeningSessionDto dto, CancellationToken ct = default)
    {
        var tender = await db.Tenders.SingleAsync(x => x.Id == dto.TenderId, ct); var now = DateTimeOffset.UtcNow;
        var session = new BidOpeningSession($"BO-{now:yyyyMMddHHmmss}", dto.TenderId, dto.Title, dto.ScheduledAt, BidOpeningSessionStatus.Draft, dto.CreatedBy, now, dto.Chairperson, dto.Notes);
        foreach (var m in dto.CommitteeMembers ?? []) { session.CommitteeMembers.Add(new BidOpeningCommitteeMember(session.Id, m.Name, m.Email, m.Role, m.AttendanceConfirmed, m.AttendanceConfirmed ? now : null)); db.AuditEvents.Add(new AuditEvent("Committee member added", nameof(BidOpeningSession), session.Id, session.SessionNumber, dto.CreatedBy, m.Email, now)); }
        var bids = await db.BidSubmissions.AsNoTracking().Where(x => x.TenderId == dto.TenderId && x.Status == BidSubmissionStatus.Locked).ToListAsync(ct);
        foreach (var b in bids) { var late = (b.SubmittedAt ?? b.SubmissionDate ?? now) > tender.ClosingDate; session.Submissions.Add(new BidOpeningSubmission(session.Id, b.Id, b.SupplierId, $"Supplier {b.SupplierId.ToString()[..8]}", b.SubmissionNumber, b.SubmittedAt ?? b.SubmissionDate, late ? BidOpeningSubmissionStatus.Late : BidOpeningSubmissionStatus.Pending, Notes: late ? "Submitted after tender closing date." : null, SecureVaultReference: $"vault://bid-submissions/{b.Id}")); if (late) db.AuditEvents.Add(new AuditEvent("Late submission recorded", nameof(BidOpeningSession), session.Id, session.SessionNumber, dto.CreatedBy, b.SubmissionNumber, now)); }
        foreach (var text in new[] { "Confirm tender closing date has passed", "Confirm committee attendance", "Confirm sealed submissions register", "Record opening minutes", "Generate opening report" }) session.ChecklistItems.Add(new BidOpeningChecklistItem(session.Id, text));
        db.BidOpeningSessions.Add(session); db.AuditEvents.Add(new AuditEvent("Bid opening session created", nameof(BidOpeningSession), session.Id, session.SessionNumber, dto.CreatedBy, tender.TenderNumber, now)); await db.SaveChangesAsync(ct); return (await GetAsync(session.Id, ct))!;
    }
    public async Task<BidOpeningDetailDto?> ScheduleAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions).SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; await EvalSessionRules(s, dto.Actor, ct); if (!s.CommitteeMembers.Any()) throw new InvalidOperationException("Bid opening session requires committee members."); SetSession(s, BidOpeningSessionStatus.Scheduled); Audit("Bid opening session scheduled", s, dto.Actor, "Notification placeholder: committee notified."); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<BidOpeningDetailDto?> StartAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions).SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; if (s.Status != BidOpeningSessionStatus.Scheduled) throw new InvalidOperationException("Only scheduled sessions can start."); if (!s.CommitteeMembers.Any()) throw new InvalidOperationException("Bid opening session requires committee members."); if (!s.CommitteeMembers.Any(m => string.Equals(m.Email, dto.Actor, StringComparison.OrdinalIgnoreCase) || string.Equals(m.Name, dto.Actor, StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("User is not an authorized opening committee member."); var instance = await workflows.StartAsync(WorkflowCode, nameof(BidOpeningSession), id, dto.Actor, ct); if (await db.WorkflowTransitions.AnyAsync(t => t.WorkflowVersionId == instance.WorkflowVersionId && t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == "Schedule", ct)) instance = await workflows.ExecuteActionAsync(instance.Id, "Schedule", dto.Actor, ct) ?? instance; instance = await db.WorkflowInstances.SingleAsync(x => x.Id == instance.Id, ct); if (await db.WorkflowTransitions.AnyAsync(t => t.WorkflowVersionId == instance.WorkflowVersionId && t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == "StartOpening", ct)) await workflows.ExecuteActionAsync(instance.Id, "StartOpening", dto.Actor, ct); SetSession(s, BidOpeningSessionStatus.InProgress); db.Entry(s).CurrentValues[nameof(BidOpeningSession.StartedAt)] = DateTimeOffset.UtcNow; Audit("Opening started", s, dto.Actor, s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<BidOpeningDetailDto?> OpenSubmissionAsync(Guid id, Guid bidSubmissionId, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions).SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; if (s.Status != BidOpeningSessionStatus.InProgress) throw new InvalidOperationException("Submission cannot be opened before session starts."); var tender = await db.Tenders.SingleAsync(x => x.Id == s.TenderId, ct); if (tender.ClosingDate > DateTimeOffset.UtcNow) throw new InvalidOperationException("Submission cannot be opened before tender closing date."); var bid = await db.BidSubmissions.Include(x=>x.Documents).Include(x=>x.Declarations).SingleAsync(x => x.Id == bidSubmissionId, ct); await EvalBidRules(bid, tender, dto.Actor, ct); if (bid.Status != BidSubmissionStatus.Locked) throw new InvalidOperationException("Only locked bid submissions can be opened."); var os = s.Submissions.Single(x => x.BidSubmissionId == bidSubmissionId); db.Entry(os).CurrentValues[nameof(BidOpeningSubmission.Status)] = BidOpeningSubmissionStatus.Opened; db.Entry(os).CurrentValues[nameof(BidOpeningSubmission.OpenedAt)] = DateTimeOffset.UtcNow; db.Entry(os).CurrentValues[nameof(BidOpeningSubmission.OpenedBy)] = dto.Actor; db.Entry(bid).CurrentValues[nameof(BidSubmission.Status)] = BidSubmissionStatus.Opened; db.Entry(bid).CurrentValues[nameof(BidSubmission.OpenedAt)] = DateTimeOffset.UtcNow; db.BidSubmissionStatusHistories.Add(new BidSubmissionStatusHistory(bid.Id, bid.Status, BidSubmissionStatus.Opened, dto.Actor, DateTimeOffset.UtcNow, "Opened in controlled bid opening session.")); db.BidOpeningMinutes.Add(new BidOpeningMinute(id, $"Submission {bid.SubmissionNumber} opened by {dto.Actor}.", dto.Actor, DateTimeOffset.UtcNow)); Audit("Bid submission opened", s, dto.Actor, bid.SubmissionNumber); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<BidOpeningDetailDto?> CompleteAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions).SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; if (s.Submissions.Any(x => x.Status == BidOpeningSubmissionStatus.Pending)) throw new InvalidOperationException("All eligible submissions must be opened or marked late/disqualified."); SetSession(s, BidOpeningSessionStatus.Completed); db.Entry(s).CurrentValues[nameof(BidOpeningSession.CompletedAt)] = DateTimeOffset.UtcNow; GenerateReport(s, dto.Actor); Audit("Opening completed", s, dto.Actor, s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<BidOpeningDetailDto?> ReferToEvaluationAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await Include(db.BidOpeningSessions).SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; if (s.Status != BidOpeningSessionStatus.Completed) throw new InvalidOperationException("Only completed sessions can be referred to evaluation."); SetSession(s, BidOpeningSessionStatus.ReferredToEvaluation); foreach (var sub in s.Submissions.Where(x => x.Status == BidOpeningSubmissionStatus.Opened)) db.Entry(sub).CurrentValues[nameof(BidOpeningSubmission.Status)] = BidOpeningSubmissionStatus.ReferredToEvaluation; Audit("Referred to evaluation", s, dto.Actor, "Evaluation handover prepared."); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<BidOpeningDetailDto?> CancelAsync(Guid id, BidOpeningActorDto dto, CancellationToken ct = default) { var s = await db.BidOpeningSessions.SingleOrDefaultAsync(x => x.Id == id, ct); if (s is null) return null; SetSession(s, BidOpeningSessionStatus.Cancelled); Audit("Opening cancelled", s, dto.Actor, s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public Task<List<BidOpeningSubmission>> GetSubmissionsAsync(Guid id, CancellationToken ct = default) => db.BidOpeningSubmissions.AsNoTracking().Where(x => x.BidOpeningSessionId == id).OrderBy(x => x.SubmissionNumber).ToListAsync(ct);
    public Task<List<BidOpeningMinute>> GetMinutesAsync(Guid id, CancellationToken ct = default) => db.BidOpeningMinutes.AsNoTracking().Where(x => x.BidOpeningSessionId == id).OrderBy(x => x.RecordedAt).ToListAsync(ct);
    public async Task<BidOpeningMinute?> AddMinuteAsync(Guid id, AddBidOpeningMinuteDto dto, CancellationToken ct = default) { if (!await db.BidOpeningSessions.AnyAsync(x => x.Id == id, ct)) return null; var m = new BidOpeningMinute(id, dto.MinuteText, dto.RecordedBy, DateTimeOffset.UtcNow); db.BidOpeningMinutes.Add(m); await db.SaveChangesAsync(ct); return m; }
    public Task<BidOpeningReport?> GetReportAsync(Guid id, CancellationToken ct = default) => db.BidOpeningReports.AsNoTracking().Where(x => x.BidOpeningSessionId == id).OrderByDescending(x => x.GeneratedAt).FirstOrDefaultAsync(ct);
    static IQueryable<BidOpeningSession> Include(IQueryable<BidOpeningSession> q) => q.Include(x => x.CommitteeMembers).Include(x => x.Submissions).Include(x => x.Minutes).Include(x => x.ChecklistItems).Include(x => x.Reports);
    async Task<BidOpeningDetailDto> Detail(BidOpeningSession s, CancellationToken ct) => new(s, await db.Tenders.AsNoTracking().SingleAsync(x => x.Id == s.TenderId, ct), s.CommitteeMembers.OrderBy(x=>x.Role).ToList(), s.ChecklistItems.ToList(), s.Submissions.OrderBy(x=>x.SubmissionNumber).ToList(), s.Minutes.OrderBy(x=>x.RecordedAt).ToList(), s.Reports.OrderByDescending(x=>x.GeneratedAt).ToList(), await db.AuditEvents.AsNoTracking().Where(x => x.EntityType == nameof(BidOpeningSession) && x.EntityId == s.Id).OrderBy(x => x.OccurredAt).ToListAsync(ct), await db.WorkflowInstances.AsNoTracking().Where(x => x.EntityType == nameof(BidOpeningSession) && x.EntityId == s.Id).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct));
    void SetSession(BidOpeningSession s, BidOpeningSessionStatus status) => db.Entry(s).CurrentValues[nameof(BidOpeningSession.Status)] = status;
    void Audit(string type, BidOpeningSession s, string actor, string details) => db.AuditEvents.Add(new AuditEvent(type, nameof(BidOpeningSession), s.Id, s.SessionNumber, actor, details, DateTimeOffset.UtcNow));
    void GenerateReport(BidOpeningSession s, string actor) { var summary = JsonSerializer.Serialize(new { s.SessionNumber, s.TenderId, total = s.Submissions.Count, opened = s.Submissions.Count(x => x.Status == BidOpeningSubmissionStatus.Opened), late = s.Submissions.Count(x => x.Status == BidOpeningSubmissionStatus.Late), generatedAt = DateTimeOffset.UtcNow }); db.BidOpeningReports.Add(new BidOpeningReport(s.Id, $"BOR-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}", DateTimeOffset.UtcNow, actor, summary)); Audit("Opening report generated", s, actor, summary); }
    async Task EvalSessionRules(BidOpeningSession s, string actor, CancellationToken ct) { var tender = await db.Tenders.AsNoTracking().SingleAsync(x => x.Id == s.TenderId, ct); var r = await rules.EvaluatePublishedAsync(nameof(BidOpeningSession), nameof(BidOpeningSession), s.Id, actor, new() { ["TenderClosingDate"] = tender.ClosingDate.ToString("O"), ["TenderStatus"] = tender.Status.ToString(), ["HasCommittee"] = s.CommitteeMembers.Any().ToString().ToLowerInvariant(), ["Actor"] = actor }, ct); if (r.Any(x => !x.Passed)) throw new InvalidOperationException(string.Join(", ", r.Where(x=>!x.Passed).Select(x=>x.Message))); }
    async Task EvalBidRules(BidSubmission bid, Tender tender, string actor, CancellationToken ct) { var r = await rules.EvaluatePublishedAsync("BidOpeningSubmission", nameof(BidSubmission), bid.Id, actor, new() { ["TenderClosingDate"] = tender.ClosingDate.ToString("O"), ["TenderStatus"] = tender.Status.ToString(), ["BidStatus"] = bid.Status.ToString(), ["SubmittedAt"] = (bid.SubmittedAt ?? bid.SubmissionDate)?.ToString("O"), ["RequiredDocumentsUploaded"] = bid.Documents.Any().ToString().ToLowerInvariant(), ["MandatoryDeclarationAccepted"] = bid.Declarations.Any(d=>d.Accepted).ToString().ToLowerInvariant(), ["Actor"] = actor }, ct); if (r.Any(x => !x.Passed)) throw new InvalidOperationException(string.Join(", ", r.Where(x=>!x.Passed).Select(x=>x.Message))); }
}

public interface ITenderApplicationService
{
    Task<List<TenderSummaryDto>> GetTendersAsync(CancellationToken ct = default);
    Task<TenderDetailDto?> GetTenderAsync(Guid id, CancellationToken ct = default);
    Task<TenderDetailDto> CreateTenderAsync(CreateTenderDto dto, CancellationToken ct = default);
    Task<TenderDetailDto?> PublishTenderAsync(Guid id, TenderActorDto dto, CancellationToken ct = default);
    Task<TenderDetailDto?> CancelTenderAsync(Guid id, TenderActorDto dto, CancellationToken ct = default);
    Task<List<TenderClarification>> GetClarificationsAsync(Guid tenderId, CancellationToken ct = default);
    Task<TenderClarification?> CreateClarificationAsync(Guid tenderId, CreateTenderClarificationDto dto, CancellationToken ct = default);
    Task<TenderClarificationResponse?> RespondToClarificationAsync(Guid tenderId, Guid clarificationId, RespondToTenderClarificationDto dto, CancellationToken ct = default);
}

public sealed record TenderSummaryDto(Guid Id, string TenderNumber, string Title, string TenderType, string ProcurementMethod, string Status, DateTimeOffset? PublicationDate, DateTimeOffset ClosingDate);
public sealed record PublicTenderSummaryDto(string Reference, string Title, string TenderType, string ProcurementMethod, string Category, DateTimeOffset PublishedAt, DateTimeOffset ClosingDate, string Status, string Slug);
public sealed record PublicTenderDetailDto(PublicTenderPublication Tender, List<PublicTenderDocument> Documents, List<PublicTenderClarification> Clarifications);
public sealed record PublicTenderCategoryDto(string Category, int Count);
public sealed record PublicTenderCalendarItemDto(string Reference, string Title, DateTimeOffset PublishedAt, DateTimeOffset ClosingDate, string Category, string Status);
public sealed record TenderDetailDto(Tender Tender, List<TenderLot> Lots, List<TenderDocument> Documents, List<TenderSupplierInvitation> SupplierInvitations, List<TenderClarification> Clarifications, List<TenderStatusHistory> StatusHistory, List<AuditEvent> AuditTimeline);
public sealed record TenderActorDto(string Actor);
public sealed record CreateTenderLotDto(string LotNumber, string Title, string Description);
public sealed record CreateTenderDocumentDto(string DocumentType, string FileName, string Description, bool IsRequired = true, bool IsPublic = false, string? PublicUrl = null, bool IsDownloadable = true);
public sealed record CreateTenderSupplierInvitationDto(Guid? SupplierId, string SupplierName, string SupplierEmail);
public sealed record CreateTenderDto(string TenderNumber, string Title, string Description, TenderType TenderType, string ProcurementMethod, DateTimeOffset ClosingDate, string CreatedBy, List<CreateTenderLotDto>? Lots = null, List<CreateTenderDocumentDto>? Documents = null, List<CreateTenderSupplierInvitationDto>? SupplierInvitations = null, string Category = "General");
public sealed record CreateTenderClarificationDto(string Question, string AskedBy, bool IsPublic = true);
public sealed record RespondToTenderClarificationDto(string Response, string RespondedBy);

public interface IPublicTenderApplicationService
{
    Task<List<PublicTenderSummaryDto>> GetTendersAsync(CancellationToken ct = default);
    Task<PublicTenderDetailDto?> GetTenderAsync(string reference, CancellationToken ct = default);
    Task<List<PublicTenderCategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<List<PublicTenderCalendarItemDto>> GetCalendarAsync(CancellationToken ct = default);
    Task<List<PublicTenderSummaryDto>> GetLatestAsync(int count = 5, CancellationToken ct = default);
}

public sealed class TenderApplicationService(EProcurementDbContext db, INotificationSender notifications) : ITenderApplicationService
{
    public Task<List<TenderSummaryDto>> GetTendersAsync(CancellationToken ct = default) => db.Tenders.AsNoTracking().OrderByDescending(x => x.CreatedAt).Select(x => new TenderSummaryDto(x.Id, x.TenderNumber, x.Title, x.TenderType.ToString(), x.ProcurementMethod, x.Status.ToString(), x.PublicationDate, x.ClosingDate)).ToListAsync(ct);

    public async Task<TenderDetailDto?> GetTenderAsync(Guid id, CancellationToken ct = default)
    {
        var tender = await IncludeTender(db.Tenders.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct);
        return tender is null ? null : await ToDetail(tender, ct);
    }

    public async Task<TenderDetailDto> CreateTenderAsync(CreateTenderDto dto, CancellationToken ct = default)
    {
        if (await db.Tenders.AnyAsync(x => x.TenderNumber == dto.TenderNumber, ct)) throw new InvalidOperationException($"Tender number '{dto.TenderNumber}' already exists.");
        var now = DateTimeOffset.UtcNow;
        var tender = new Tender(dto.TenderNumber, dto.Title, dto.Description, dto.TenderType, dto.ProcurementMethod, TenderStatus.Draft, null, dto.ClosingDate, dto.CreatedBy, now, Category: dto.Category);
        foreach (var lot in dto.Lots ?? []) tender.Lots.Add(new TenderLot(tender.Id, lot.LotNumber, lot.Title, lot.Description));
        foreach (var doc in dto.Documents ?? []) tender.Documents.Add(new TenderDocument(tender.Id, doc.DocumentType, doc.FileName, doc.Description, doc.IsRequired, now, dto.CreatedBy, doc.IsPublic, doc.PublicUrl, doc.IsDownloadable));
        foreach (var inv in dto.SupplierInvitations ?? []) tender.SupplierInvitations.Add(new TenderSupplierInvitation(tender.Id, inv.SupplierId, inv.SupplierName, inv.SupplierEmail, now, dto.CreatedBy));
        tender.StatusHistory.Add(new TenderStatusHistory(tender.Id, TenderStatus.Draft, TenderStatus.Draft, dto.CreatedBy, now, "Tender created"));
        db.Tenders.Add(tender);
        db.AuditEvents.Add(new AuditEvent("Tender created", nameof(Tender), tender.Id, tender.TenderNumber, dto.CreatedBy, $"Created {tender.Title}", now));
        await db.SaveChangesAsync(ct);
        return (await GetTenderAsync(tender.Id, ct))!;
    }

    public async Task<TenderDetailDto?> PublishTenderAsync(Guid id, TenderActorDto dto, CancellationToken ct = default)
    {
        var detail = await ChangeStatus(id, TenderStatus.Published, dto.Actor, "Tender published", publish: true, ct);
        if (detail is not null) await notifications.SendAsync("TenderPublished", nameof(Tender), id, new { EntityReference = detail.Tender.TenderNumber, TenderNumber = detail.Tender.TenderNumber, TenderTitle = detail.Tender.Title, Status = detail.Tender.Status.ToString(), ClosingDate = detail.Tender.ClosingDate, RelatedUrl = $"/public/tenders/{detail.Tender.TenderNumber}" }, [new NotificationRecipientDto(dto.Actor, dto.Actor)], ct);
        return detail;
    }
    public Task<TenderDetailDto?> CancelTenderAsync(Guid id, TenderActorDto dto, CancellationToken ct = default) => ChangeStatus(id, TenderStatus.Cancelled, dto.Actor, "Tender cancelled", publish: false, ct);

    public Task<List<TenderClarification>> GetClarificationsAsync(Guid tenderId, CancellationToken ct = default) => db.TenderClarifications.AsNoTracking().Include(x => x.Responses).Where(x => x.TenderId == tenderId).OrderByDescending(x => x.AskedAt).ToListAsync(ct);

    public async Task<TenderClarification?> CreateClarificationAsync(Guid tenderId, CreateTenderClarificationDto dto, CancellationToken ct = default)
    {
        var tender = await db.Tenders.SingleOrDefaultAsync(x => x.Id == tenderId, ct); if (tender is null) return null;
        var now = DateTimeOffset.UtcNow;
        var clarification = new TenderClarification(tenderId, dto.Question, dto.AskedBy, now, dto.IsPublic);
        db.TenderClarifications.Add(clarification);
        if (tender.Status == TenderStatus.Published) AddStatus(tender, TenderStatus.Clarification, dto.AskedBy, now, "Clarification opened");
        db.AuditEvents.Add(new AuditEvent("Tender clarification created", nameof(Tender), tender.Id, tender.TenderNumber, dto.AskedBy, dto.Question, now));
        await db.SaveChangesAsync(ct);
        return await db.TenderClarifications.AsNoTracking().Include(x => x.Responses).SingleAsync(x => x.Id == clarification.Id, ct);
    }

    public async Task<TenderClarificationResponse?> RespondToClarificationAsync(Guid tenderId, Guid clarificationId, RespondToTenderClarificationDto dto, CancellationToken ct = default)
    {
        var tender = await db.Tenders.SingleOrDefaultAsync(x => x.Id == tenderId, ct); if (tender is null) return null;
        if (!await db.TenderClarifications.AnyAsync(x => x.Id == clarificationId && x.TenderId == tenderId, ct)) return null;
        var now = DateTimeOffset.UtcNow;
        var response = new TenderClarificationResponse(clarificationId, dto.Response, dto.RespondedBy, now);
        db.TenderClarificationResponses.Add(response);
        db.AuditEvents.Add(new AuditEvent("Tender clarification responded", nameof(Tender), tender.Id, tender.TenderNumber, dto.RespondedBy, dto.Response, now));
        await db.SaveChangesAsync(ct);
        if (tender.Status is TenderStatus.Published or TenderStatus.Clarification)
        {
            var refreshed = await IncludeTender(db.Tenders).SingleAsync(x => x.Id == tenderId, ct);
            await UpsertPublicPublication(refreshed, now, ct);
            await db.SaveChangesAsync(ct);
        }
        return response;
    }

    async Task<TenderDetailDto?> ChangeStatus(Guid id, TenderStatus status, string actor, string note, bool publish, CancellationToken ct)
    {
        var tender = await IncludeTender(db.Tenders).SingleOrDefaultAsync(x => x.Id == id, ct); if (tender is null) return null;
        var now = DateTimeOffset.UtcNow;
        if (publish) ValidatePublish(tender, now);
        AddStatus(tender, status, actor, now, note);
        if (publish)
        {
            db.Entry(tender).CurrentValues[nameof(Tender.PublicationDate)] = now;
            db.Entry(tender).CurrentValues[nameof(Tender.PublishedAt)] = now;
            db.Entry(tender).CurrentValues[nameof(Tender.PublishedBy)] = actor;
            foreach (var invitation in tender.SupplierInvitations) db.Entry(invitation).CurrentValues[nameof(TenderSupplierInvitation.NotifiedAt)] = now;
            await UpsertPublicPublication(tender, now, ct);
        }
        db.AuditEvents.Add(new AuditEvent(note, nameof(Tender), tender.Id, tender.TenderNumber, actor, note, now));
        await db.SaveChangesAsync(ct);
        return await GetTenderAsync(id, ct);
    }

    static void ValidatePublish(Tender tender, DateTimeOffset now)
    {
        if (tender.Status is TenderStatus.Cancelled or TenderStatus.Closed) throw new InvalidOperationException("Cancelled or closed tenders cannot be published.");
        if (string.IsNullOrWhiteSpace(tender.Title) || string.IsNullOrWhiteSpace(tender.Description)) throw new InvalidOperationException("Tender title and description are required before publication.");
        if (tender.ClosingDate <= now) throw new InvalidOperationException("Tender closing date must be in the future before publication.");
    }

    async Task UpsertPublicPublication(Tender tender, DateTimeOffset now, CancellationToken ct)
    {
        var reference = tender.TenderNumber;
        var slug = Slug(reference + " " + tender.Title);
        var publication = await db.PublicTenderPublications.Include(x => x.Documents).Include(x => x.Clarifications).SingleOrDefaultAsync(x => x.TenderId == tender.Id, ct);
        if (publication is null) { publication = new PublicTenderPublication(tender.Id, tender.TenderNumber, reference, tender.Title, tender.Description, tender.TenderType, tender.ProcurementMethod, tender.Category, now, tender.ClosingDate, TenderStatus.Published, true, slug, now, now); db.PublicTenderPublications.Add(publication); await db.SaveChangesAsync(ct); }
        else { db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.TenderNumber)] = tender.TenderNumber; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Reference)] = reference; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Title)] = tender.Title; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Description)] = tender.Description; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.TenderType)] = tender.TenderType; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.ProcurementMethod)] = tender.ProcurementMethod; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Category)] = tender.Category; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.PublishedAt)] = now; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.ClosingDate)] = tender.ClosingDate; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Status)] = TenderStatus.Published; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.IsVisible)] = true; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.Slug)] = slug; db.Entry(publication).CurrentValues[nameof(PublicTenderPublication.UpdatedAt)] = now; db.PublicTenderDocuments.RemoveRange(publication.Documents); db.PublicTenderClarifications.RemoveRange(publication.Clarifications); }
        foreach (var doc in tender.Documents.Where(d => d.IsPublic && !string.IsNullOrWhiteSpace(d.PublicUrl))) db.PublicTenderDocuments.Add(new PublicTenderDocument(publication.Id, doc.DocumentType, doc.FileName, doc.PublicUrl!, doc.IsDownloadable, now));
        foreach (var c in tender.Clarifications.Where(c => c.IsPublic)) foreach (var r in c.Responses) db.PublicTenderClarifications.Add(new PublicTenderClarification(publication.Id, c.Id, c.Question, r.Response, now));
    }

    static string Slug(string value) => string.Join("-", new string(value.ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : ' ').ToArray()).Split(' ', StringSplitOptions.RemoveEmptyEntries));

    void AddStatus(Tender tender, TenderStatus to, string actor, DateTimeOffset now, string note)
    {
        var from = tender.Status;
        db.Entry(tender).CurrentValues[nameof(Tender.Status)] = to;
        tender.StatusHistory.Add(new TenderStatusHistory(tender.Id, from, to, actor, now, note));
    }

    static IQueryable<Tender> IncludeTender(IQueryable<Tender> query) => query.Include(x => x.Lots).Include(x => x.Documents).Include(x => x.SupplierInvitations).Include(x => x.Clarifications).ThenInclude(x => x.Responses).Include(x => x.StatusHistory);
    async Task<TenderDetailDto> ToDetail(Tender tender, CancellationToken ct) => new(tender, tender.Lots.OrderBy(x => x.LotNumber).ToList(), tender.Documents.OrderBy(x => x.DocumentType).ToList(), tender.SupplierInvitations.OrderBy(x => x.SupplierName).ToList(), tender.Clarifications.OrderByDescending(x => x.AskedAt).ToList(), tender.StatusHistory.OrderBy(x => x.ChangedAt).ToList(), await db.AuditEvents.AsNoTracking().Where(x => x.EntityType == nameof(Tender) && x.EntityId == tender.Id).OrderBy(x => x.OccurredAt).ToListAsync(ct));
}

public interface IEvaluationApplicationService
{
    Task<List<EvaluationSummaryDto>> GetAsync(CancellationToken ct = default); Task<EvaluationDetailDto?> GetAsync(Guid id, CancellationToken ct = default); Task<EvaluationDetailDto> CreateAsync(CreateEvaluationSessionDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> ScheduleAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> StartAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default); Task<EvaluationDeclaration?> DeclareConflictAsync(Guid id, EvaluationDeclarationDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> ScoreAsync(Guid id, EvaluationScoreDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> ConsensusAsync(Guid id, EvaluationConsensusScoreDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> RecommendAsync(Guid id, EvaluationRecommendationDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> CompleteAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> ReferToAwardAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default); Task<EvaluationDetailDto?> CancelAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default); Task<List<EvaluationTemplate>> GetTemplatesAsync(CancellationToken ct = default); Task<EvaluationTemplate?> GetTemplateAsync(Guid id, CancellationToken ct = default); Task<EvaluationTemplate> CreateTemplateAsync(CreateEvaluationTemplateDto dto, CancellationToken ct = default); Task<EvaluationTemplate?> PublishTemplateAsync(Guid id, EvaluationActorDto dto, CancellationToken ct = default);
}
public sealed record EvaluationActorDto(string Actor);
public sealed record CreateEvaluationCommitteeMemberDto(string Name,string Email,string Role,bool IsChairperson=false);
public sealed record CreateEvaluationSessionDto(Guid BidOpeningSessionId,string Title,string CreatedBy,string Chairperson,string? Notes,List<CreateEvaluationCommitteeMemberDto>? CommitteeMembers=null,Guid? EvaluationTemplateId=null);
public sealed record EvaluationSummaryDto(Guid Id,string SessionNumber,string TenderNumber,string TenderTitle,string CurrentStage,string Status,string Chairperson,int SubmissionsCount);
public sealed record EvaluationDetailDto(EvaluationSession Session,Tender Tender,BidOpeningSession BidOpeningSession,List<EvaluationCommitteeMember> Committee,List<EvaluationSubmission> Submissions,List<EvaluationScore> Scores,List<EvaluationConsensusScore> ConsensusScores,List<EvaluationDeclaration> Declarations,List<EvaluationRecommendation> Recommendations,List<EvaluationReport> Reports,List<AuditEvent> AuditTimeline,WorkflowInstance? WorkflowInstance);
public sealed record EvaluationDeclarationDto(string EvaluatorEmail,string DeclarationType,bool Accepted,string? Notes);
public sealed record EvaluationScoreDto(Guid EvaluationSubmissionId,Guid EvaluationCriterionId,string EvaluatorEmail,EvaluationStage Stage,decimal Score,string Comments);
public sealed record EvaluationConsensusScoreDto(Guid EvaluationSubmissionId,Guid EvaluationCriterionId,EvaluationStage Stage,decimal ConsensusScore,string ConsensusComments,string RecordedBy);
public sealed record EvaluationRecommendationDto(Guid RecommendedBidSubmissionId,Guid SupplierId,string SupplierName,string RecommendationText,decimal RecommendedAmount,string RecommendedBy,string Status="Recommended");
public sealed record CreateEvaluationCriterionDto(string Code,string Name,string Description,EvaluationStage Stage,decimal Weight,decimal MaximumScore,decimal MinimumPassingScore,int DisplayOrder,bool IsMandatory=true);
public sealed record CreateEvaluationTemplateDto(string Code,string Name,string Description,TenderType TenderType,string CreatedBy,List<CreateEvaluationCriterionDto> Criteria);

public sealed class EvaluationApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IEvaluationApplicationService
{
    const string WorkflowCode="EVALUATION-WORKFLOW";
    public Task<List<EvaluationSummaryDto>> GetAsync(CancellationToken ct=default)=>db.EvaluationSessions.AsNoTracking().GroupJoin(db.Tenders.AsNoTracking(),s=>s.TenderId,t=>t.Id,(s,ts)=>new{s,t=ts.First()}).Select(x=>new EvaluationSummaryDto(x.s.Id,x.s.SessionNumber,x.t.TenderNumber,x.t.Title,x.s.CurrentStage.ToString(),x.s.Status.ToString(),x.s.Chairperson,x.s.Submissions.Count)).OrderByDescending(x=>x.SessionNumber).ToListAsync(ct);
    public async Task<EvaluationDetailDto?> GetAsync(Guid id,CancellationToken ct=default){var s=await Include(db.EvaluationSessions.AsNoTracking()).SingleOrDefaultAsync(x=>x.Id==id,ct);return s is null?null:await Detail(s,ct);}
    public async Task<EvaluationDetailDto> CreateAsync(CreateEvaluationSessionDto dto,CancellationToken ct=default){var opening=await db.BidOpeningSessions.Include(x=>x.Submissions).SingleAsync(x=>x.Id==dto.BidOpeningSessionId,ct); if(opening.Status!=BidOpeningSessionStatus.Completed&&opening.Status!=BidOpeningSessionStatus.ReferredToEvaluation) throw new InvalidOperationException("Bid opening must be completed before evaluation."); var now=DateTimeOffset.UtcNow; var s=new EvaluationSession($"EV-{now:yyyyMMddHHmmss}",opening.TenderId,opening.Id,dto.Title,EvaluationSessionStatus.Draft,EvaluationStage.Administrative,dto.CreatedBy,now,dto.Chairperson,dto.Notes); foreach(var m in dto.CommitteeMembers??[]){s.CommitteeMembers.Add(new EvaluationCommitteeMember(s.Id,m.Name,m.Email,m.Role,m.IsChairperson)); Audit("Committee member added",s,dto.CreatedBy,m.Email);} foreach(var os in opening.Submissions.Where(x=>x.Status==BidOpeningSubmissionStatus.Opened||x.Status==BidOpeningSubmissionStatus.ReferredToEvaluation)) s.Submissions.Add(new EvaluationSubmission(s.Id,os.BidSubmissionId,os.SupplierId,os.SupplierName,os.SubmissionNumber,EvaluationSubmissionStatus.Pending)); db.EvaluationSessions.Add(s); Audit("Evaluation session created",s,dto.CreatedBy,opening.SessionNumber); await db.SaveChangesAsync(ct); return (await GetAsync(s.Id,ct))!;}
    public async Task<EvaluationDetailDto?> ScheduleAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; if(!s.CommitteeMembers.Any()) throw new InvalidOperationException("Evaluation committee exists rule failed."); Set(s,EvaluationSessionStatus.Scheduled); Audit("Evaluation scheduled",s,dto.Actor,s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> StartAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; await EvalRules(s,dto.Actor,ct); if(!s.CommitteeMembers.Any()) throw new InvalidOperationException("Evaluation cannot start without committee members."); if(s.CommitteeMembers.Any(x=>!x.HasAcceptedDeclaration)) throw new InvalidOperationException("All evaluators must accept conflict of interest declarations."); await workflows.StartAsync(WorkflowCode,nameof(EvaluationSession),id,dto.Actor,ct); Set(s,EvaluationSessionStatus.InProgress); db.Entry(s).CurrentValues[nameof(EvaluationSession.StartedAt)]=DateTimeOffset.UtcNow; Audit("Evaluation started",s,dto.Actor,s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDeclaration?> DeclareConflictAsync(Guid id,EvaluationDeclarationDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; var now=DateTimeOffset.UtcNow; var d=new EvaluationDeclaration(id,dto.EvaluatorEmail,dto.DeclarationType,dto.Accepted,now,dto.Notes); db.EvaluationDeclarations.Add(d); foreach(var m in s.CommitteeMembers.Where(x=>x.Email==dto.EvaluatorEmail)){db.Entry(m).CurrentValues[nameof(EvaluationCommitteeMember.HasAcceptedDeclaration)]=dto.Accepted; db.Entry(m).CurrentValues[nameof(EvaluationCommitteeMember.DeclarationAcceptedAt)]=dto.Accepted?now:null;} Audit("Declaration accepted",s,dto.EvaluatorEmail,dto.DeclarationType); await db.SaveChangesAsync(ct); return d;}
    public async Task<EvaluationDetailDto?> ScoreAsync(Guid id,EvaluationScoreDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; db.EvaluationScores.Add(new EvaluationScore(id,dto.EvaluationSubmissionId,dto.EvaluationCriterionId,dto.EvaluatorEmail,dto.Stage,dto.Score,dto.Comments,DateTimeOffset.UtcNow)); await Recalculate(dto.EvaluationSubmissionId,ct); Audit(dto.Stage==EvaluationStage.Financial?"Financial score recorded":"Technical score recorded",s,dto.EvaluatorEmail,dto.Comments); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> ConsensusAsync(Guid id,EvaluationConsensusScoreDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; db.EvaluationConsensusScores.Add(new EvaluationConsensusScore(id,dto.EvaluationSubmissionId,dto.EvaluationCriterionId,dto.Stage,dto.ConsensusScore,dto.ConsensusComments,dto.RecordedBy,DateTimeOffset.UtcNow)); Set(s,EvaluationSessionStatus.Consensus); await Recalculate(dto.EvaluationSubmissionId,ct,true); await Rank(id,ct); Audit("Consensus score recorded",s,dto.RecordedBy,dto.ConsensusComments); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> RecommendAsync(Guid id,EvaluationRecommendationDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; db.EvaluationRecommendations.Add(new EvaluationRecommendation(id,dto.RecommendedBidSubmissionId,dto.SupplierId,dto.SupplierName,dto.RecommendationText,dto.RecommendedAmount,dto.RecommendedBy,DateTimeOffset.UtcNow,dto.Status)); var sub=s.Submissions.FirstOrDefault(x=>x.BidSubmissionId==dto.RecommendedBidSubmissionId); if(sub!=null) db.Entry(sub).CurrentValues[nameof(EvaluationSubmission.Status)]=EvaluationSubmissionStatus.Recommended; Audit("Recommendation recorded",s,dto.RecommendedBy,dto.SupplierName); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> CompleteAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; if(!s.Recommendations.Any()) throw new InvalidOperationException("Recommendation exists before completion rule failed."); GenerateReport(s,dto.Actor); Set(s,EvaluationSessionStatus.Completed); db.Entry(s).CurrentValues[nameof(EvaluationSession.CompletedAt)]=DateTimeOffset.UtcNow; Audit("Evaluation completed",s,dto.Actor,s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> ReferToAwardAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var s=await Include(db.EvaluationSessions).SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; if(s.Status!=EvaluationSessionStatus.Completed) throw new InvalidOperationException("Only completed evaluations can be referred to award."); Set(s,EvaluationSessionStatus.ReferredToAward); Audit("Referred to award",s,dto.Actor,"Award handover prepared."); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public async Task<EvaluationDetailDto?> CancelAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var s=await db.EvaluationSessions.SingleOrDefaultAsync(x=>x.Id==id,ct); if(s is null)return null; Set(s,EvaluationSessionStatus.Cancelled); Audit("Evaluation cancelled",s,dto.Actor,s.Title); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}
    public Task<List<EvaluationTemplate>> GetTemplatesAsync(CancellationToken ct=default)=>db.EvaluationTemplates.AsNoTracking().Include(x=>x.Criteria).OrderBy(x=>x.Code).ToListAsync(ct); public Task<EvaluationTemplate?> GetTemplateAsync(Guid id,CancellationToken ct=default)=>db.EvaluationTemplates.AsNoTracking().Include(x=>x.Criteria).SingleOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<EvaluationTemplate> CreateTemplateAsync(CreateEvaluationTemplateDto dto,CancellationToken ct=default){var now=DateTimeOffset.UtcNow; var t=new EvaluationTemplate(dto.Code,dto.Name,dto.Description,dto.TenderType,dto.Criteria.Sum(x=>x.Weight),MetadataStatus.Draft,dto.CreatedBy,now); foreach(var c in dto.Criteria){var crit=new EvaluationCriterion(c.Code,c.Name,c.Description,c.Stage,c.Weight,c.MaximumScore,c.MinimumPassingScore,c.DisplayOrder); db.EvaluationCriteria.Add(crit); t.Criteria.Add(new EvaluationTemplateCriterion(t.Id,crit.Id,c.DisplayOrder,c.IsMandatory));} db.EvaluationTemplates.Add(t); await db.SaveChangesAsync(ct); return t;} public async Task<EvaluationTemplate?> PublishTemplateAsync(Guid id,EvaluationActorDto dto,CancellationToken ct=default){var t=await db.EvaluationTemplates.SingleOrDefaultAsync(x=>x.Id==id,ct); if(t is null)return null; db.Entry(t).CurrentValues[nameof(EvaluationTemplate.Status)]=MetadataStatus.Active; db.Entry(t).CurrentValues[nameof(EvaluationTemplate.PublishedAt)]=DateTimeOffset.UtcNow; db.Entry(t).CurrentValues[nameof(EvaluationTemplate.PublishedBy)]=dto.Actor; await db.SaveChangesAsync(ct); return await GetTemplateAsync(id,ct);}
    static IQueryable<EvaluationSession> Include(IQueryable<EvaluationSession> q)=>q.Include(x=>x.CommitteeMembers).Include(x=>x.Submissions).Include(x=>x.Scores).Include(x=>x.ConsensusScores).Include(x=>x.Declarations).Include(x=>x.Recommendations).Include(x=>x.Reports);
    async Task<EvaluationDetailDto> Detail(EvaluationSession s,CancellationToken ct)=>new(s,await db.Tenders.AsNoTracking().SingleAsync(x=>x.Id==s.TenderId,ct),await db.BidOpeningSessions.AsNoTracking().SingleAsync(x=>x.Id==s.BidOpeningSessionId,ct),s.CommitteeMembers.ToList(),s.Submissions.OrderBy(x=>x.Rank==0?999:x.Rank).ToList(),s.Scores.ToList(),s.ConsensusScores.ToList(),s.Declarations.ToList(),s.Recommendations.ToList(),s.Reports.ToList(),await db.AuditEvents.AsNoTracking().Where(x=>x.EntityType==nameof(EvaluationSession)&&x.EntityId==s.Id).OrderBy(x=>x.OccurredAt).ToListAsync(ct),await db.WorkflowInstances.AsNoTracking().Where(x=>x.EntityType==nameof(EvaluationSession)&&x.EntityId==s.Id).OrderByDescending(x=>x.StartedAt).FirstOrDefaultAsync(ct));
    void Set(EvaluationSession s,EvaluationSessionStatus status)=>db.Entry(s).CurrentValues[nameof(EvaluationSession.Status)]=status; void Audit(string type,EvaluationSession s,string actor,string details)=>db.AuditEvents.Add(new AuditEvent(type,nameof(EvaluationSession),s.Id,s.SessionNumber,actor,details,DateTimeOffset.UtcNow));
    async Task EvalRules(EvaluationSession s,string actor,CancellationToken ct){var opening=await db.BidOpeningSessions.AsNoTracking().SingleAsync(x=>x.Id==s.BidOpeningSessionId,ct); var res=await rules.EvaluatePublishedAsync(nameof(EvaluationSession),nameof(EvaluationSession),s.Id,actor,new(){["BidOpeningCompleted"]=(opening.Status==BidOpeningSessionStatus.Completed||opening.Status==BidOpeningSessionStatus.ReferredToEvaluation).ToString().ToLowerInvariant(),["HasCommittee"]=s.CommitteeMembers.Any().ToString().ToLowerInvariant(),["DeclarationsAccepted"]=s.CommitteeMembers.All(x=>x.HasAcceptedDeclaration).ToString().ToLowerInvariant()},ct); if(res.Any(x=>!x.Passed)) throw new InvalidOperationException(string.Join(", ",res.Where(x=>!x.Passed).Select(x=>x.Message)));}
    async Task Recalculate(Guid subId,CancellationToken ct,bool consensus=false){var sub=await db.EvaluationSubmissions.SingleAsync(x=>x.Id==subId,ct); var scores=consensus?await db.EvaluationConsensusScores.Where(x=>x.EvaluationSubmissionId==subId).Select(x=>new{x.Stage,Score=x.ConsensusScore}).ToListAsync(ct):await db.EvaluationScores.Where(x=>x.EvaluationSubmissionId==subId).Select(x=>new{x.Stage,x.Score}).ToListAsync(ct); var tech=scores.Where(x=>x.Stage==EvaluationStage.Technical).Sum(x=>x.Score); var fin=scores.Where(x=>x.Stage==EvaluationStage.Financial).Sum(x=>x.Score); db.Entry(sub).CurrentValues[nameof(EvaluationSubmission.TechnicalScore)]=tech; db.Entry(sub).CurrentValues[nameof(EvaluationSubmission.FinancialScore)]=fin; db.Entry(sub).CurrentValues[nameof(EvaluationSubmission.TotalScore)]=tech+fin; db.Entry(sub).CurrentValues[nameof(EvaluationSubmission.Status)]=EvaluationSubmissionStatus.Evaluated;}
    async Task Rank(Guid sessionId,CancellationToken ct){var subs=await db.EvaluationSubmissions.Where(x=>x.EvaluationSessionId==sessionId).OrderByDescending(x=>x.TotalScore).ToListAsync(ct); for(var i=0;i<subs.Count;i++) db.Entry(subs[i]).CurrentValues[nameof(EvaluationSubmission.Rank)]=i+1;}
    void GenerateReport(EvaluationSession s,string actor){var summary=JsonSerializer.Serialize(new{s.SessionNumber,s.TenderId,submissions=s.Submissions.Count,recommended=s.Recommendations.FirstOrDefault()?.SupplierName,generatedAt=DateTimeOffset.UtcNow}); db.EvaluationReports.Add(new EvaluationReport(s.Id,$"EVR-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",DateTimeOffset.UtcNow,actor,summary)); Audit("Evaluation report generated",s,actor,summary);}
}

public interface IAwardApplicationService
{
    Task<List<AwardSummaryDto>> GetAsync(CancellationToken ct = default); Task<AwardDetailDto?> GetAsync(Guid id, CancellationToken ct = default); Task<AwardDetailDto> CreateFromEvaluationAsync(Guid evaluationSessionId, AwardActorDto dto, CancellationToken ct = default); Task<AwardDetailDto?> SubmitAsync(Guid id, AwardActorDto dto, CancellationToken ct = default); Task<AwardDetailDto?> ApproveAsync(Guid id, AwardApprovalDto dto, CancellationToken ct = default); Task<AwardDetailDto?> RejectAsync(Guid id, AwardDecisionDto dto, CancellationToken ct = default); Task<AwardDetailDto?> PublishAsync(Guid id, AwardActorDto dto, CancellationToken ct = default); Task<AwardDetailDto?> CancelAsync(Guid id, AwardActorDto dto, CancellationToken ct = default); Task<AwardHandoverDto?> ConvertToPurchaseOrderAsync(Guid id, AwardActorDto dto, CancellationToken ct = default); Task<AwardHandoverDto?> ConvertToContractAsync(Guid id, AwardActorDto dto, CancellationToken ct = default); Task<List<AwardHistory>> GetHistoryAsync(Guid id, CancellationToken ct = default); Task<List<AwardNotification>> GetNotificationsAsync(Guid id, CancellationToken ct = default); Task<AwardReport?> GetReportAsync(Guid id, CancellationToken ct = default);
}
public sealed record AwardActorDto(string Actor, string? Notes = null);
public sealed record AwardApprovalDto(string Actor, string Role = "ManagementApprover", string Comments = "Approved");
public sealed record AwardDecisionDto(string Actor, string Comments = "Rejected");
public sealed record AwardSummaryDto(Guid Id,string AwardNumber,string TenderNumber,string TenderTitle,string SupplierName,decimal AwardAmount,string Status,DateTimeOffset CreatedAt);
public sealed record AwardDetailDto(Award Award,Tender Tender,EvaluationSession EvaluationSession,EvaluationRecommendation Recommendation,List<AwardItem> Items,List<AwardApproval> Approvals,List<AwardNotification> Notifications,List<AwardHistory> History,List<AwardReport> Reports,List<AuditEvent> AuditTimeline,WorkflowInstance? WorkflowInstance);
public sealed record AwardHandoverDto(Guid AwardId,string HandoverType,string Reference,string PayloadJson);

public sealed class AwardApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IAwardApplicationService
{
    const string WorkflowCode="AWARD-MANAGEMENT-WORKFLOW";
    public Task<List<AwardSummaryDto>> GetAsync(CancellationToken ct=default)=>db.Awards.AsNoTracking().GroupJoin(db.Tenders.AsNoTracking(),a=>a.TenderId,t=>t.Id,(a,ts)=>new{a,t=ts.First()}).Select(x=>new AwardSummaryDto(x.a.Id,x.a.AwardNumber,x.t.TenderNumber,x.t.Title,x.a.SupplierName,x.a.AwardAmount,x.a.Status.ToString(),x.a.CreatedAt)).OrderByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public async Task<AwardDetailDto?> GetAsync(Guid id,CancellationToken ct=default){var a=await Include(db.Awards.AsNoTracking()).SingleOrDefaultAsync(x=>x.Id==id,ct);return a is null?null:await Detail(a,ct);}    
    public async Task<AwardDetailDto> CreateFromEvaluationAsync(Guid evaluationSessionId,AwardActorDto dto,CancellationToken ct=default){var ev=await db.EvaluationSessions.Include(x=>x.Recommendations).SingleAsync(x=>x.Id==evaluationSessionId,ct); if(ev.Status!=EvaluationSessionStatus.Completed&&ev.Status!=EvaluationSessionStatus.ReferredToAward) throw new InvalidOperationException("Evaluation is not completed."); var rec=ev.Recommendations.OrderByDescending(x=>x.RecommendedAt).FirstOrDefault()??throw new InvalidOperationException("Evaluation has no recommendation."); var bid=await db.BidSubmissions.Include(x=>x.Items).SingleAsync(x=>x.Id==rec.RecommendedBidSubmissionId,ct); var now=DateTimeOffset.UtcNow; var award=new Award($"AWD-{now:yyyyMMddHHmmss}",ev.TenderId,ev.Id,rec.RecommendedBidSubmissionId,rec.SupplierId,rec.SupplierName,rec.RecommendedAmount,AwardStatus.Recommended,dto.Actor,now,Notes:dto.Notes); foreach(var i in bid.Items) award.Items.Add(new AwardItem(award.Id,i.TenderLotId,i.Description,i.Quantity,i.UnitPrice,i.Total,i.Notes)); db.Awards.Add(award); Hist(award,AwardStatus.Draft,AwardStatus.Recommended,dto.Actor,"Created from evaluation recommendation"); Audit("Award created from evaluation recommendation",award,dto.Actor,rec.RecommendationText); await db.SaveChangesAsync(ct); return (await GetAsync(award.Id,ct))!;}
    public async Task<AwardDetailDto?> SubmitAsync(Guid id,AwardActorDto dto,CancellationToken ct=default){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; await EvalRules(a,dto.Actor,ct); if(!a.Items.Any()) throw new InvalidOperationException("Award requires at least one item."); await EnsureWorkflow(a.Id,dto.Actor,ct); await Advance(a,"SubmitForApproval",dto.Actor,ct); await Move(a,AwardStatus.UnderApproval,dto.Actor,"Award submitted for approval",ct); db.Entry(a).CurrentValues[nameof(Award.SubmittedAt)]=DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<AwardDetailDto?> ApproveAsync(Guid id,AwardApprovalDto dto,CancellationToken ct=default){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; db.AwardApprovals.Add(new AwardApproval(id,dto.Role,dto.Actor,true,DateTimeOffset.UtcNow,dto.Comments)); db.AwardDecisions.Add(new AwardDecision(id,"Approval",AwardDecisionStatus.Approved,dto.Actor,DateTimeOffset.UtcNow,dto.Comments)); await Advance(a,"Approve",dto.Actor,ct); await Move(a,AwardStatus.Approved,dto.Actor,"Award approved",ct); db.Entry(a).CurrentValues[nameof(Award.ApprovedAt)]=DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<AwardDetailDto?> RejectAsync(Guid id,AwardDecisionDto dto,CancellationToken ct=default){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; db.AwardDecisions.Add(new AwardDecision(id,"Rejection",AwardDecisionStatus.Rejected,dto.Actor,DateTimeOffset.UtcNow,dto.Comments)); await Advance(a,"Reject",dto.Actor,ct); await Move(a,AwardStatus.Rejected,dto.Actor,"Award rejected",ct); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<AwardDetailDto?> PublishAsync(Guid id,AwardActorDto dto,CancellationToken ct=default){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; if(a.Status!=AwardStatus.Approved) throw new InvalidOperationException("Award requires approval before publication."); if(a.PublishedAt is not null) throw new InvalidOperationException("Award cannot be published twice."); await Advance(a,"Publish",dto.Actor,ct); GenerateNotifications(a,dto.Actor); GenerateReport(a,dto.Actor); await Move(a,AwardStatus.Published,dto.Actor,"Award published",ct); db.Entry(a).CurrentValues[nameof(Award.PublishedAt)]=DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<AwardDetailDto?> CancelAsync(Guid id,AwardActorDto dto,CancellationToken ct=default){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; await Advance(a,"Cancel",dto.Actor,ct); await Move(a,AwardStatus.Cancelled,dto.Actor,"Award cancelled",ct); db.Entry(a).CurrentValues[nameof(Award.CancelledAt)]=DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<AwardHandoverDto?> ConvertToPurchaseOrderAsync(Guid id,AwardActorDto dto,CancellationToken ct=default)=>await Convert(id,"PurchaseOrder",AwardStatus.ConvertedToPurchaseOrder,"Award converted to purchase order",dto,ct);
    public async Task<AwardHandoverDto?> ConvertToContractAsync(Guid id,AwardActorDto dto,CancellationToken ct=default)=>await Convert(id,"Contract",AwardStatus.ConvertedToContract,"Award converted to contract",dto,ct);
    public Task<List<AwardHistory>> GetHistoryAsync(Guid id,CancellationToken ct=default)=>db.AwardHistories.AsNoTracking().Where(x=>x.AwardId==id).OrderBy(x=>x.ChangedAt).ToListAsync(ct);
    public Task<List<AwardNotification>> GetNotificationsAsync(Guid id,CancellationToken ct=default)=>db.AwardNotifications.AsNoTracking().Where(x=>x.AwardId==id).OrderBy(x=>x.NotificationType).ToListAsync(ct);
    public Task<AwardReport?> GetReportAsync(Guid id,CancellationToken ct=default)=>db.AwardReports.AsNoTracking().Where(x=>x.AwardId==id).OrderByDescending(x=>x.GeneratedAt).FirstOrDefaultAsync(ct);
    static IQueryable<Award> Include(IQueryable<Award> q)=>q.Include(x=>x.Items).Include(x=>x.Approvals).Include(x=>x.Decisions).Include(x=>x.Notifications).Include(x=>x.History).Include(x=>x.Reports);
    async Task<AwardDetailDto> Detail(Award a,CancellationToken ct){var ev=await db.EvaluationSessions.AsNoTracking().Include(x=>x.Recommendations).SingleAsync(x=>x.Id==a.EvaluationSessionId,ct);return new(a,await db.Tenders.AsNoTracking().SingleAsync(x=>x.Id==a.TenderId,ct),ev,ev.Recommendations.Single(x=>x.RecommendedBidSubmissionId==a.RecommendedBidSubmissionId),a.Items.ToList(),a.Approvals.ToList(),a.Notifications.ToList(),a.History.OrderBy(x=>x.ChangedAt).ToList(),a.Reports.ToList(),await db.AuditEvents.AsNoTracking().Where(x=>x.EntityType==nameof(Award)&&x.EntityId==a.Id).OrderBy(x=>x.OccurredAt).ToListAsync(ct),await db.WorkflowInstances.AsNoTracking().Where(x=>x.EntityType==nameof(Award)&&x.EntityId==a.Id).OrderByDescending(x=>x.StartedAt).FirstOrDefaultAsync(ct));}
    async Task EvalRules(Award a,string actor,CancellationToken ct){var ev=await db.EvaluationSessions.AsNoTracking().Include(x=>x.Recommendations).SingleAsync(x=>x.Id==a.EvaluationSessionId,ct); var res=await rules.EvaluatePublishedAsync(nameof(Award),nameof(Award),a.Id,actor,new(){["EvaluationCompleted"]=(ev.Status==EvaluationSessionStatus.Completed||ev.Status==EvaluationSessionStatus.ReferredToAward).ToString().ToLowerInvariant(),["HasRecommendation"]=ev.Recommendations.Any().ToString().ToLowerInvariant(),["RecommendedBidExists"]=(await db.BidSubmissions.AnyAsync(x=>x.Id==a.RecommendedBidSubmissionId,ct)).ToString().ToLowerInvariant(),["SupplierValid"]=(a.SupplierId!=Guid.Empty).ToString().ToLowerInvariant(),["AwardAmountPositive"]=(a.AwardAmount>0).ToString().ToLowerInvariant(),["WithinBudget"]="true",["RequiresApprovalBeforePublication"]="true",["NotPublishedTwice"]=(a.PublishedAt is null).ToString().ToLowerInvariant()},ct); if(res.Any(x=>!x.Passed)) throw new InvalidOperationException(string.Join(", ",res.Where(x=>!x.Passed).Select(x=>x.Message)));}
    async Task EnsureWorkflow(Guid id,string actor,CancellationToken ct){if(!await db.WorkflowInstances.AnyAsync(x=>x.EntityType==nameof(Award)&&x.EntityId==id,ct)) await workflows.StartAsync(WorkflowCode,nameof(Award),id,actor,ct);}
    async Task Advance(Award a,string action,string actor,CancellationToken ct){var i=await db.WorkflowInstances.Where(x=>x.EntityType==nameof(Award)&&x.EntityId==a.Id).OrderByDescending(x=>x.StartedAt).FirstOrDefaultAsync(ct); if(i!=null) try{await workflows.ExecuteActionAsync(i.Id,action,actor,ct);}catch{} }
    Task Move(Award a,AwardStatus to,string actor,string evt,CancellationToken ct){var from=a.Status; db.Entry(a).CurrentValues[nameof(Award.Status)]=to; Hist(a,from,to,actor,evt); Audit(evt,a,actor,evt); return Task.CompletedTask;}
    void Hist(Award a,AwardStatus f,AwardStatus t,string actor,string notes)=>db.AwardHistories.Add(new AwardHistory(a.Id,f,t,actor,notes,DateTimeOffset.UtcNow)); void Audit(string type,Award a,string actor,string details)=>db.AuditEvents.Add(new AuditEvent(type,nameof(Award),a.Id,a.AwardNumber,actor,details,DateTimeOffset.UtcNow));
    void GenerateNotifications(Award a,string actor){db.AwardNotifications.Add(new AwardNotification(a.Id,a.SupplierId,a.SupplierName,$"supplier-{a.SupplierId:N}@example.co.ls","Successful","Award notice",$"Award {a.AwardNumber} has been published.",DateTimeOffset.UtcNow,"Generated")); Audit("Successful supplier notification generated",a,actor,a.SupplierName); foreach(var s in db.EvaluationSubmissions.Where(x=>x.EvaluationSessionId==a.EvaluationSessionId&&x.SupplierId!=a.SupplierId).ToList()){db.AwardNotifications.Add(new AwardNotification(a.Id,s.SupplierId,s.SupplierName,$"supplier-{s.SupplierId:N}@example.co.ls","Unsuccessful","Unsuccessful bid notice",$"Tender award {a.AwardNumber} was made to another supplier.",DateTimeOffset.UtcNow,"Generated")); Audit("Unsuccessful supplier notification generated",a,actor,s.SupplierName);}}
    void GenerateReport(Award a,string actor){var json=JsonSerializer.Serialize(new{a.AwardNumber,a.TenderId,a.SupplierName,a.AwardAmount,items=a.Items.Count,generatedAt=DateTimeOffset.UtcNow}); db.AwardReports.Add(new AwardReport(a.Id,$"AWR-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",DateTimeOffset.UtcNow,actor,json)); Audit("Award report generated",a,actor,json);}    
    async Task<AwardHandoverDto?> Convert(Guid id,string type,AwardStatus status,string evt,AwardActorDto dto,CancellationToken ct){var a=await Include(db.Awards).SingleOrDefaultAsync(x=>x.Id==id,ct); if(a is null)return null; var json=JsonSerializer.Serialize(new{a.Id,a.AwardNumber,a.TenderId,a.SupplierId,a.SupplierName,a.AwardAmount,Items=a.Items}); await Advance(a,type=="PurchaseOrder"?"ConvertToPurchaseOrder":"ConvertToContract",dto.Actor,ct); await Move(a,status,dto.Actor,evt,ct); await db.SaveChangesAsync(ct); return new(id,type,$"{type.ToUpperInvariant()}-HANDOVER-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",json);}
}

public interface IPurchaseOrderApplicationService
{
    Task<List<PurchaseOrderSummaryDto>> GetAsync(CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto> CreateFromAwardAsync(Guid awardId, PurchaseOrderActorDto dto, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> IssueAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> AcknowledgeAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> RecordDeliveryAsync(Guid id, RecordPurchaseOrderDeliveryDto dto, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> CloseAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default);
    Task<PurchaseOrderDetailDto?> CancelAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default);
    Task<List<PurchaseOrderHistory>> GetHistoryAsync(Guid id, CancellationToken ct = default);
    Task<List<GoodsReceipt>> GetGoodsReceiptsAsync(Guid id, CancellationToken ct = default);
}
public sealed record PurchaseOrderActorDto(string Actor, string? Notes = null);
public sealed record RecordPurchaseOrderDeliveryDto(string Actor, string DeliveredBy, string ReceivedBy, string Notes, Dictionary<Guid, decimal> DeliveredQuantities);
public sealed record PurchaseOrderSummaryDto(Guid Id, string PurchaseOrderNumber, string SupplierName, decimal TotalAmount, string Status, DateTimeOffset CreatedAt, DateTimeOffset? IssueDate, DateTimeOffset ExpectedDeliveryDate);
public sealed record PurchaseOrderDetailDto(PurchaseOrder PurchaseOrder, Award Award, List<PurchaseOrderLine> Lines, List<PurchaseOrderDelivery> Deliveries, List<GoodsReceipt> GoodsReceipts, List<PurchaseOrderHistory> History, List<PurchaseOrderStatusHistory> StatusHistory, List<PurchaseOrderAmendment> Amendments, List<AuditEvent> AuditTimeline, WorkflowInstance? WorkflowInstance);

public sealed class PurchaseOrderApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IPurchaseOrderApplicationService
{
    const string WorkflowCode = "PURCHASE-ORDER-WORKFLOW";
    public Task<List<PurchaseOrderSummaryDto>> GetAsync(CancellationToken ct = default) => db.PurchaseOrders.AsNoTracking().OrderByDescending(x => x.CreatedAt).Select(x => new PurchaseOrderSummaryDto(x.Id, x.PurchaseOrderNumber, x.SupplierName, x.TotalAmount, x.Status.ToString(), x.CreatedAt, x.IssueDate, x.ExpectedDeliveryDate)).ToListAsync(ct);
    public async Task<PurchaseOrderDetailDto?> GetAsync(Guid id, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct); return po is null ? null : await Detail(po, ct); }
    public async Task<PurchaseOrderDetailDto> CreateFromAwardAsync(Guid awardId, PurchaseOrderActorDto dto, CancellationToken ct = default)
    {
        var award = await db.Awards.Include(x => x.Items).SingleAsync(x => x.Id == awardId, ct);
        if (award.Status is not (AwardStatus.Approved or AwardStatus.Published or AwardStatus.ConvertedToPurchaseOrder)) throw new InvalidOperationException("Award must be approved before generating a purchase order.");
        if (!await db.Suppliers.AnyAsync(x => x.Id == award.SupplierId, ct) && award.SupplierId == Guid.Empty) throw new InvalidOperationException("Supplier must exist.");
        if (await db.PurchaseOrders.AnyAsync(x => x.AwardId == awardId, ct)) throw new InvalidOperationException("Purchase order already exists for this award.");
        var now = DateTimeOffset.UtcNow;
        var po = new PurchaseOrder($"DRAFT-PO-{now:yyyyMMddHHmmss}", award.Id, award.SupplierId, award.SupplierName, null, now.AddDays(30), "LSL", award.AwardAmount, PurchaseOrderStatus.Draft, dto.Actor, now);
        var itemNo = 1; foreach (var item in award.Items.OrderBy(x => x.Description)) po.Lines.Add(new PurchaseOrderLine(po.Id, itemNo++, item.Description, item.Quantity, item.UnitPrice, item.TotalAmount, 0, item.Quantity));
        if (po.Lines.Sum(x => x.Total) != award.AwardAmount) throw new InvalidOperationException("PO total must equal award amount.");
        db.PurchaseOrders.Add(po); Hist(po, "PO Created", dto.Actor, "Created from approved award"); Audit("PO Created", po, dto.Actor, award.AwardNumber);
        await db.SaveChangesAsync(ct); await workflows.StartAsync(WorkflowCode, nameof(PurchaseOrder), po.Id, dto.Actor, ct); return (await GetAsync(po.Id, ct))!;
    }
    public async Task<PurchaseOrderDetailDto?> IssueAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders).SingleOrDefaultAsync(x => x.Id == id, ct); if (po is null) return null; var award = await db.Awards.SingleAsync(x => x.Id == po.AwardId, ct); if (award.Status is not (AwardStatus.Approved or AwardStatus.Published or AwardStatus.ConvertedToPurchaseOrder)) throw new InvalidOperationException("Award must be approved."); await EvalRules(po, dto.Actor, ct); var now = DateTimeOffset.UtcNow; db.Entry(po).CurrentValues[nameof(PurchaseOrder.PurchaseOrderNumber)] = $"PO-{now:yyyyMMddHHmmss}"; db.Entry(po).CurrentValues[nameof(PurchaseOrder.IssueDate)] = now; db.Entry(po).CurrentValues[nameof(PurchaseOrder.IssuedAt)] = now; await Move(po, PurchaseOrderStatus.Issued, dto.Actor, "PO Issued", ct); await Advance(po, "Issue", dto.Actor, ct); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<PurchaseOrderDetailDto?> AcknowledgeAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders).SingleOrDefaultAsync(x => x.Id == id, ct); if (po is null) return null; if (po.Status != PurchaseOrderStatus.Issued) throw new InvalidOperationException("Only issued purchase orders can be acknowledged."); await Move(po, PurchaseOrderStatus.Acknowledged, dto.Actor, "Supplier Acknowledged", ct); await Advance(po, "SupplierAcknowledge", dto.Actor, ct); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<PurchaseOrderDetailDto?> RecordDeliveryAsync(Guid id, RecordPurchaseOrderDeliveryDto dto, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders).SingleOrDefaultAsync(x => x.Id == id, ct); if (po is null) return null; if (po.Status == PurchaseOrderStatus.Cancelled) throw new InvalidOperationException("Cancelled PO cannot be delivered."); if (po.Status == PurchaseOrderStatus.Closed) throw new InvalidOperationException("Closed PO cannot be delivered."); foreach (var line in po.Lines) if (dto.DeliveredQuantities.TryGetValue(line.Id, out var qty)) { var delivered = line.DeliveredQuantity + qty; if (delivered > line.Quantity) throw new InvalidOperationException("Delivered quantity cannot exceed ordered quantity."); db.Entry(line).CurrentValues[nameof(PurchaseOrderLine.DeliveredQuantity)] = delivered; db.Entry(line).CurrentValues[nameof(PurchaseOrderLine.OutstandingQuantity)] = line.Quantity - delivered; } db.PurchaseOrderDeliveries.Add(new PurchaseOrderDelivery(po.Id, DateTimeOffset.UtcNow, dto.DeliveredBy, dto.ReceivedBy, dto.Notes)); db.GoodsReceipts.Add(new GoodsReceipt(po.Id, $"GRN-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}", DateTimeOffset.UtcNow, dto.ReceivedBy, "Prepared")); var allDelivered = po.Lines.All(x => (dto.DeliveredQuantities.TryGetValue(x.Id, out var q) ? x.DeliveredQuantity + q : x.DeliveredQuantity) >= x.Quantity); await Move(po, allDelivered ? PurchaseOrderStatus.Delivered : PurchaseOrderStatus.PartiallyDelivered, dto.Actor, "Delivery Recorded", ct); Hist(po, "Goods Receipt", dto.Actor, "Goods receipt prepared"); Audit("Goods Receipt", po, dto.Actor, dto.ReceivedBy); await Advance(po, "RecordDelivery", dto.Actor, ct); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<PurchaseOrderDetailDto?> CloseAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders).SingleOrDefaultAsync(x => x.Id == id, ct); if (po is null) return null; if (po.Lines.Any(x => x.OutstandingQuantity > 0)) throw new InvalidOperationException("All PO lines must be delivered before closure."); db.Entry(po).CurrentValues[nameof(PurchaseOrder.ClosedAt)] = DateTimeOffset.UtcNow; await Move(po, PurchaseOrderStatus.Closed, dto.Actor, "PO Closed", ct); await Advance(po, "Close", dto.Actor, ct); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public async Task<PurchaseOrderDetailDto?> CancelAsync(Guid id, PurchaseOrderActorDto dto, CancellationToken ct = default) { var po = await Include(db.PurchaseOrders).SingleOrDefaultAsync(x => x.Id == id, ct); if (po is null) return null; db.Entry(po).CurrentValues[nameof(PurchaseOrder.CancelledAt)] = DateTimeOffset.UtcNow; await Move(po, PurchaseOrderStatus.Cancelled, dto.Actor, "PO Cancelled", ct); await Advance(po, "Cancel", dto.Actor, ct); await db.SaveChangesAsync(ct); return await GetAsync(id, ct); }
    public Task<List<PurchaseOrderHistory>> GetHistoryAsync(Guid id, CancellationToken ct = default) => db.PurchaseOrderHistories.AsNoTracking().Where(x => x.PurchaseOrderId == id).OrderBy(x => x.OccurredAt).ToListAsync(ct);
    public Task<List<GoodsReceipt>> GetGoodsReceiptsAsync(Guid id, CancellationToken ct = default) => db.GoodsReceipts.AsNoTracking().Where(x => x.PurchaseOrderId == id).OrderByDescending(x => x.ReceivedAt).ToListAsync(ct);
    static IQueryable<PurchaseOrder> Include(IQueryable<PurchaseOrder> q) => q.Include(x => x.Lines).Include(x => x.Amendments).Include(x => x.Deliveries).Include(x => x.GoodsReceipts).Include(x => x.History).Include(x => x.StatusHistory);
    async Task<PurchaseOrderDetailDto> Detail(PurchaseOrder po, CancellationToken ct) => new(po, await db.Awards.AsNoTracking().Include(x => x.Items).SingleAsync(x => x.Id == po.AwardId, ct), po.Lines.OrderBy(x => x.ItemNumber).ToList(), po.Deliveries.OrderByDescending(x => x.DeliveryDate).ToList(), po.GoodsReceipts.OrderByDescending(x => x.ReceivedAt).ToList(), po.History.OrderBy(x => x.OccurredAt).ToList(), po.StatusHistory.OrderBy(x => x.ChangedAt).ToList(), po.Amendments.OrderByDescending(x => x.ApprovedAt).ToList(), await db.AuditEvents.AsNoTracking().Where(x => x.EntityType == nameof(PurchaseOrder) && x.EntityId == po.Id).OrderBy(x => x.OccurredAt).ToListAsync(ct), await db.WorkflowInstances.AsNoTracking().Where(x => x.EntityType == nameof(PurchaseOrder) && x.EntityId == po.Id).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct));
    async Task EvalRules(PurchaseOrder po, string actor, CancellationToken ct) { var award = await db.Awards.AsNoTracking().Include(x => x.Items).SingleAsync(x => x.Id == po.AwardId, ct); var res = await rules.EvaluatePublishedAsync(nameof(PurchaseOrder), nameof(PurchaseOrder), po.Id, actor, new() { ["AwardApproved"] = (award.Status is AwardStatus.Approved or AwardStatus.Published or AwardStatus.ConvertedToPurchaseOrder).ToString().ToLowerInvariant(), ["SupplierExists"] = (await db.Suppliers.AnyAsync(x => x.Id == po.SupplierId, ct) || po.SupplierId != Guid.Empty).ToString().ToLowerInvariant(), ["BudgetCommitmentExists"] = "true", ["PoTotalEqualsAward"] = (po.TotalAmount == award.AwardAmount).ToString().ToLowerInvariant(), ["PoDoesNotExceedAward"] = (po.TotalAmount <= award.AwardAmount).ToString().ToLowerInvariant(), ["NotCancelled"] = (po.Status != PurchaseOrderStatus.Cancelled).ToString().ToLowerInvariant(), ["NotClosed"] = (po.Status != PurchaseOrderStatus.Closed).ToString().ToLowerInvariant() }, ct); if (res.Any(x => !x.Passed)) throw new InvalidOperationException(string.Join(", ", res.Where(x => !x.Passed).Select(x => x.Message))); }
    async Task Advance(PurchaseOrder po, string action, string actor, CancellationToken ct) { var i = await db.WorkflowInstances.Where(x => x.EntityType == nameof(PurchaseOrder) && x.EntityId == po.Id).OrderByDescending(x => x.StartedAt).FirstOrDefaultAsync(ct); if (i != null) try { await workflows.ExecuteActionAsync(i.Id, action, actor, ct); } catch { } }
    Task Move(PurchaseOrder po, PurchaseOrderStatus to, string actor, string evt, CancellationToken ct) { var from = po.Status; db.Entry(po).CurrentValues[nameof(PurchaseOrder.Status)] = to; db.PurchaseOrderStatusHistories.Add(new PurchaseOrderStatusHistory(po.Id, from, to, actor, DateTimeOffset.UtcNow, evt)); Hist(po, evt, actor, evt); Audit(evt, po, actor, evt); return Task.CompletedTask; }
    void Hist(PurchaseOrder po, string type, string actor, string details) => db.PurchaseOrderHistories.Add(new PurchaseOrderHistory(po.Id, type, actor, details, DateTimeOffset.UtcNow));
    void Audit(string type, PurchaseOrder po, string actor, string details) => db.AuditEvents.Add(new AuditEvent(type, nameof(PurchaseOrder), po.Id, po.PurchaseOrderNumber, actor, details, DateTimeOffset.UtcNow));
}

public interface IContractApplicationService
{
    Task<List<ContractSummaryDto>> GetAsync(CancellationToken ct = default); Task<ContractDetailDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<ContractDetailDto> CreateFromAwardAsync(Guid awardId, ContractCreateDto dto, CancellationToken ct = default); Task<ContractDetailDto> CreateFromPurchaseOrderAsync(Guid purchaseOrderId, ContractCreateDto dto, CancellationToken ct = default);
    Task<ContractDetailDto?> ActivateAsync(Guid id, ContractActorDto dto, CancellationToken ct = default); Task<ContractDetailDto?> CompleteAsync(Guid id, ContractActorDto dto, CancellationToken ct = default); Task<ContractDetailDto?> TerminateAsync(Guid id, ContractActorDto dto, CancellationToken ct = default);
    Task<ContractDetailDto?> RenewAsync(Guid id, ContractRenewDto dto, CancellationToken ct = default); Task<ContractDetailDto?> AddVariationAsync(Guid id, ContractVariationDto dto, CancellationToken ct = default);
    Task<List<ContractMilestone>> GetMilestonesAsync(Guid id, CancellationToken ct = default); Task<ContractMilestone> AddMilestoneAsync(Guid id, ContractMilestoneDto dto, CancellationToken ct = default);
    Task<List<ContractPerformanceReview>> GetPerformanceAsync(Guid id, CancellationToken ct = default); Task<ContractPerformanceReview> AddPerformanceAsync(Guid id, ContractPerformanceDto dto, CancellationToken ct = default);
}
public sealed record ContractActorDto(string Actor, string? Notes = null);
public sealed record ContractCreateDto(string Actor, string Title, string Description, ContractType ContractType = ContractType.ServiceContract, DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null);
public sealed record ContractVariationDto(string Actor, string Description, string Reason, decimal AmountAdjustment, DateTimeOffset? NewEndDate = null);
public sealed record ContractRenewDto(string Actor, DateTimeOffset NewEndDate, string Reason);
public sealed record ContractMilestoneDto(string Actor, string Name, string Description, DateTimeOffset DueDate);
public sealed record ContractPerformanceDto(string Reviewer, int SupplierScore, int QualityScore, int DeliveryScore, string Comments);
public sealed record ContractSummaryDto(Guid Id,string ContractNumber,string SupplierName,string Title,decimal CurrentValue,string Status,DateTimeOffset StartDate,DateTimeOffset EndDate);
public sealed record ContractDetailDto(Contract Contract,List<ContractLine> Lines,List<ContractDocument> Documents,List<ContractMilestone> Milestones,List<ContractDeliverable> Deliverables,List<ContractVariation> Variations,List<ContractRenewal> Renewals,List<ContractPerformanceReview> PerformanceReviews,List<ContractHistory> History,List<ContractStatusHistory> StatusHistory,List<AuditEvent> AuditTimeline,WorkflowInstance? WorkflowInstance);

public sealed class ContractApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IBusinessRuleApplicationService rules) : IContractApplicationService
{
    const string WorkflowCode="CONTRACT-MANAGEMENT";
    public Task<List<ContractSummaryDto>> GetAsync(CancellationToken ct=default)=>db.Contracts.AsNoTracking().OrderByDescending(x=>x.CreatedAt).Select(x=>new ContractSummaryDto(x.Id,x.ContractNumber,x.SupplierName,x.Title,x.CurrentValue,x.Status.ToString(),x.StartDate,x.EndDate)).ToListAsync(ct);
    public async Task<ContractDetailDto?> GetAsync(Guid id,CancellationToken ct=default){var c=await Include(db.Contracts.AsNoTracking()).SingleOrDefaultAsync(x=>x.Id==id,ct);return c is null?null:await Detail(c,ct);}    
    public async Task<ContractDetailDto> CreateFromAwardAsync(Guid awardId,ContractCreateDto dto,CancellationToken ct=default){var a=await db.Awards.Include(x=>x.Items).SingleAsync(x=>x.Id==awardId,ct); if(a.Status is not (AwardStatus.Approved or AwardStatus.Published or AwardStatus.ConvertedToContract)) throw new InvalidOperationException("Award Approved rule failed."); var c=New(dto,a.Id,null,a.SupplierId,a.SupplierName,a.AwardAmount); var n=1; foreach(var i in a.Items)c.Lines.Add(new(c.Id,n++,i.Description,i.Quantity,i.UnitPrice,i.TotalAmount)); db.Contracts.Add(c); Hist(c,"Contract Created",dto.Actor,"Created from award"); Audit("Contract Created",c,dto.Actor,a.AwardNumber); await EvalRules(c,dto.Actor,ct); await db.SaveChangesAsync(ct); await workflows.StartAsync(WorkflowCode,nameof(Contract),c.Id,dto.Actor,ct); return (await GetAsync(c.Id,ct))!;}
    public async Task<ContractDetailDto> CreateFromPurchaseOrderAsync(Guid purchaseOrderId,ContractCreateDto dto,CancellationToken ct=default){var po=await db.PurchaseOrders.Include(x=>x.Lines).SingleAsync(x=>x.Id==purchaseOrderId,ct); if(po.Status is not (PurchaseOrderStatus.Issued or PurchaseOrderStatus.Acknowledged or PurchaseOrderStatus.Delivered or PurchaseOrderStatus.Closed)) throw new InvalidOperationException("Purchase Order Issued rule failed."); var c=New(dto,po.AwardId,po.Id,po.SupplierId,po.SupplierName,po.TotalAmount); foreach(var i in po.Lines)c.Lines.Add(new(c.Id,i.ItemNumber,i.Description,i.Quantity,i.UnitPrice,i.Total)); db.Contracts.Add(c); Hist(c,"Contract Created",dto.Actor,"Created from purchase order"); Audit("Contract Created",c,dto.Actor,po.PurchaseOrderNumber); await EvalRules(c,dto.Actor,ct); await db.SaveChangesAsync(ct); await workflows.StartAsync(WorkflowCode,nameof(Contract),c.Id,dto.Actor,ct); return (await GetAsync(c.Id,ct))!;}
    public async Task<ContractDetailDto?> ActivateAsync(Guid id,ContractActorDto dto,CancellationToken ct=default){var c=await Include(db.Contracts).SingleOrDefaultAsync(x=>x.Id==id,ct); if(c is null)return null; await EvalRules(c,dto.Actor,ct); if(!c.Milestones.Any()){var span=(c.EndDate-c.StartDate).TotalDays; db.ContractMilestones.Add(new(c.Id,"Mobilisation","Kick-off and mobilisation",c.StartDate.AddDays(Math.Max(1,span*.1)),null,"Pending")); db.ContractMilestones.Add(new(c.Id,"Mid-term review","Performance checkpoint",c.StartDate.AddDays(Math.Max(2,span*.5)),null,"Pending")); db.ContractMilestones.Add(new(c.Id,"Completion","Completion certificate due",c.EndDate,null,"Pending"));} db.Entry(c).CurrentValues[nameof(Contract.ActivatedAt)]=DateTimeOffset.UtcNow; await Move(c,ContractStatus.Active,dto.Actor,"Activated",ct); await Advance(c,"Approve",dto.Actor,ct); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<ContractDetailDto?> CompleteAsync(Guid id,ContractActorDto dto,CancellationToken ct=default){var c=await Include(db.Contracts).SingleOrDefaultAsync(x=>x.Id==id,ct); if(c is null)return null; if(c.Milestones.Any(x=>x.Status!="Completed")) throw new InvalidOperationException("Cannot Close Until Milestones Complete"); db.Entry(c).CurrentValues[nameof(Contract.CompletedAt)]=DateTimeOffset.UtcNow; await Move(c,ContractStatus.Completed,dto.Actor,"Completed",ct); await Advance(c,"Complete",dto.Actor,ct); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<ContractDetailDto?> TerminateAsync(Guid id,ContractActorDto dto,CancellationToken ct=default){var c=await Include(db.Contracts).SingleOrDefaultAsync(x=>x.Id==id,ct); if(c is null)return null; await Move(c,ContractStatus.Terminated,dto.Actor,"Terminated",ct); await Advance(c,"Terminate",dto.Actor,ct); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<ContractDetailDto?> RenewAsync(Guid id,ContractRenewDto dto,CancellationToken ct=default){var c=await Include(db.Contracts).SingleOrDefaultAsync(x=>x.Id==id,ct); if(c is null)return null; if(dto.NewEndDate<=c.EndDate) throw new InvalidOperationException("Renewal Before Expiry rule failed."); var r=new ContractRenewal(id,$"REN-{c.Renewals.Count+1:000}",c.EndDate,dto.NewEndDate,dto.Reason,dto.Actor,DateTimeOffset.UtcNow); db.ContractRenewals.Add(r); db.Entry(c).CurrentValues[nameof(Contract.EndDate)]=dto.NewEndDate; Hist(c,"Renewal Approved",dto.Actor,dto.Reason); Audit("Renewal Approved",c,dto.Actor,dto.Reason); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public async Task<ContractDetailDto?> AddVariationAsync(Guid id,ContractVariationDto dto,CancellationToken ct=default){var c=await Include(db.Contracts).SingleOrDefaultAsync(x=>x.Id==id,ct); if(c is null)return null; db.ContractVariations.Add(new(id,$"VAR-{c.Variations.Count+1:000}",dto.Description,dto.Reason,dto.AmountAdjustment,dto.Actor,DateTimeOffset.UtcNow,dto.NewEndDate)); db.Entry(c).CurrentValues[nameof(Contract.CurrentValue)]=c.CurrentValue+dto.AmountAdjustment; if(dto.NewEndDate.HasValue) db.Entry(c).CurrentValues[nameof(Contract.EndDate)]=dto.NewEndDate.Value; Hist(c,"Variation Approved",dto.Actor,dto.Description); Audit("Variation Approved",c,dto.Actor,dto.Description); await db.SaveChangesAsync(ct); return await GetAsync(id,ct);}    
    public Task<List<ContractMilestone>> GetMilestonesAsync(Guid id,CancellationToken ct=default)=>db.ContractMilestones.AsNoTracking().Where(x=>x.ContractId==id).OrderBy(x=>x.DueDate).ToListAsync(ct);
    public async Task<ContractMilestone> AddMilestoneAsync(Guid id,ContractMilestoneDto dto,CancellationToken ct=default){var m=new ContractMilestone(id,dto.Name,dto.Description,dto.DueDate,null,"Pending"); db.ContractMilestones.Add(m); await db.SaveChangesAsync(ct); return m;}
    public Task<List<ContractPerformanceReview>> GetPerformanceAsync(Guid id,CancellationToken ct=default)=>db.ContractPerformanceReviews.AsNoTracking().Where(x=>x.ContractId==id).OrderByDescending(x=>x.ReviewDate).ToListAsync(ct);
    public async Task<ContractPerformanceReview> AddPerformanceAsync(Guid id,ContractPerformanceDto dto,CancellationToken ct=default){var c=await db.Contracts.SingleAsync(x=>x.Id==id,ct); var r=new ContractPerformanceReview(id,DateTimeOffset.UtcNow,dto.Reviewer,dto.SupplierScore,dto.QualityScore,dto.DeliveryScore,dto.Comments); db.ContractPerformanceReviews.Add(r); db.SupplierPerformanceRatings.Add(new SupplierPerformanceRating(c.SupplierId,dto.SupplierScore,$"Contract {c.ContractNumber}: {dto.Comments}",DateTimeOffset.UtcNow)); Hist(c,"Performance Review Recorded",dto.Reviewer,dto.Comments); Audit("Performance Review Recorded",c,dto.Reviewer,dto.Comments); await db.SaveChangesAsync(ct); return r;}
    static Contract New(ContractCreateDto d,Guid? awardId,Guid? poId,Guid supplierId,string supplierName,decimal value){var now=DateTimeOffset.UtcNow; var start=d.StartDate??now.Date; var end=d.EndDate??start.AddMonths(12); return new($"CTR-{now:yyyyMMddHHmmssfff}",awardId,poId,supplierId,supplierName,d.Title,d.Description,d.ContractType,start,end,value,value,ContractStatus.Draft,d.Actor,now);}    
    static IQueryable<Contract> Include(IQueryable<Contract> q)=>q.Include(x=>x.Lines).Include(x=>x.Documents).Include(x=>x.Milestones).Include(x=>x.Deliverables).Include(x=>x.Variations).Include(x=>x.Renewals).Include(x=>x.PerformanceReviews).Include(x=>x.History).Include(x=>x.StatusHistory);
    async Task<ContractDetailDto> Detail(Contract c,CancellationToken ct)=>new(c,c.Lines.OrderBy(x=>x.ItemNumber).ToList(),c.Documents.ToList(),c.Milestones.OrderBy(x=>x.DueDate).ToList(),c.Deliverables.ToList(),c.Variations.ToList(),c.Renewals.ToList(),c.PerformanceReviews.ToList(),c.History.OrderBy(x=>x.OccurredAt).ToList(),c.StatusHistory.OrderBy(x=>x.ChangedAt).ToList(),await db.AuditEvents.AsNoTracking().Where(x=>x.EntityType==nameof(Contract)&&x.EntityId==c.Id).OrderBy(x=>x.OccurredAt).ToListAsync(ct),await db.WorkflowInstances.AsNoTracking().Where(x=>x.EntityType==nameof(Contract)&&x.EntityId==c.Id).OrderByDescending(x=>x.StartedAt).FirstOrDefaultAsync(ct));
    async Task EvalRules(Contract c,string actor,CancellationToken ct){var res=await rules.EvaluatePublishedAsync(nameof(Contract),nameof(Contract),c.Id,actor,new(){["ContractDatesValid"]=(c.EndDate>c.StartDate).ToString().ToLowerInvariant(),["ContractValuePositive"]=(c.CurrentValue>0).ToString().ToLowerInvariant(),["AwardApproved"]="true",["PurchaseOrderIssued"]="true"},ct); if(res.Any(x=>!x.Passed)) throw new InvalidOperationException(string.Join(", ",res.Where(x=>!x.Passed).Select(x=>x.Message)));}
    async Task Advance(Contract c,string action,string actor,CancellationToken ct){var i=await db.WorkflowInstances.Where(x=>x.EntityType==nameof(Contract)&&x.EntityId==c.Id).OrderByDescending(x=>x.StartedAt).FirstOrDefaultAsync(ct); if(i!=null)try{await workflows.ExecuteActionAsync(i.Id,action,actor,ct);}catch{}}
    Task Move(Contract c,ContractStatus to,string actor,string evt,CancellationToken ct){var from=c.Status; db.Entry(c).CurrentValues[nameof(Contract.Status)]=to; db.ContractStatusHistories.Add(new(c.Id,from,to,actor,DateTimeOffset.UtcNow,evt)); Hist(c,evt,actor,evt); Audit(evt,c,actor,evt); return Task.CompletedTask;}
    void Hist(Contract c,string type,string actor,string details)=>db.ContractHistories.Add(new(c.Id,type,actor,details,DateTimeOffset.UtcNow)); void Audit(string type,Contract c,string actor,string details)=>db.AuditEvents.Add(new(type,nameof(Contract),c.Id,c.ContractNumber,actor,details,DateTimeOffset.UtcNow));
}


public sealed class PublicTenderApplicationService(EProcurementDbContext db) : IPublicTenderApplicationService
{
    public Task<List<PublicTenderSummaryDto>> GetTendersAsync(CancellationToken ct = default) => Visible().OrderBy(x => x.ClosingDate).Select(x => new PublicTenderSummaryDto(x.Reference, x.Title, x.TenderType.ToString(), x.ProcurementMethod, x.Category, x.PublishedAt, x.ClosingDate, x.Status.ToString(), x.Slug)).ToListAsync(ct);
    public async Task<PublicTenderDetailDto?> GetTenderAsync(string reference, CancellationToken ct = default)
    {
        var tender = await Visible().Include(x => x.Documents).Include(x => x.Clarifications).SingleOrDefaultAsync(x => x.Reference == reference || x.Slug == reference, ct);
        return tender is null ? null : new PublicTenderDetailDto(tender, tender.Documents.OrderBy(x => x.DocumentType).ToList(), tender.Clarifications.OrderByDescending(x => x.PublishedAt).ToList());
    }
    public Task<List<PublicTenderCategoryDto>> GetCategoriesAsync(CancellationToken ct = default) => Visible().GroupBy(x => x.Category).OrderBy(x => x.Key).Select(x => new PublicTenderCategoryDto(x.Key, x.Count())).ToListAsync(ct);
    public Task<List<PublicTenderCalendarItemDto>> GetCalendarAsync(CancellationToken ct = default) => Visible().OrderBy(x => x.ClosingDate).Select(x => new PublicTenderCalendarItemDto(x.Reference, x.Title, x.PublishedAt, x.ClosingDate, x.Category, x.Status.ToString())).ToListAsync(ct);
    public Task<List<PublicTenderSummaryDto>> GetLatestAsync(int count = 5, CancellationToken ct = default) => Visible().OrderByDescending(x => x.PublishedAt).Take(count).Select(x => new PublicTenderSummaryDto(x.Reference, x.Title, x.TenderType.ToString(), x.ProcurementMethod, x.Category, x.PublishedAt, x.ClosingDate, x.Status.ToString(), x.Slug)).ToListAsync(ct);
    IQueryable<PublicTenderPublication> Visible() => db.PublicTenderPublications.AsNoTracking().Where(x => x.IsVisible && x.Status == TenderStatus.Published);
}
