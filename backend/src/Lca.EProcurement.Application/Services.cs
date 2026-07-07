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
        object entity = entityType == nameof(Supplier) ? await db.Suppliers.Include(s => s.Documents).Include(s => s.Categories).SingleAsync(s => s.Id == entityId, ct) : throw new NotSupportedException($"Rules for entity type '{entityType}' are not configured.");
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
    static string EntityReference(object e) => e is Supplier s ? s.ReferenceNumber : e.ToString() ?? string.Empty;
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
        if (e == "Supplier.Categories.Any()") return Supplier(c).Categories.Any();
        if (e.Contains(" != ")) { var parts = e.Split(" != ", 2, StringSplitOptions.TrimEntries); return !string.Equals(Value(parts[0], c), Quoted(parts[1]), StringComparison.OrdinalIgnoreCase); }
        if (e.Contains(" == ")) { var parts = e.Split(" == ", 2, StringSplitOptions.TrimEntries); return string.Equals(Value(parts[0], c), Quoted(parts[1]), StringComparison.OrdinalIgnoreCase); }
        throw new InvalidOperationException($"Expression '{e}' is not supported by the safe evaluator.");
    }
    static Supplier Supplier(RuleEvaluationContext c) => c.Entity as Supplier ?? throw new InvalidOperationException("Supplier rules require a Supplier context.");
    static string? Value(string token, RuleEvaluationContext c)
    {
        token = token.Trim(); var s = Supplier(c);
        if (token.StartsWith("Field(", StringComparison.Ordinal)) return c.Values.GetValueOrDefault(Arg(token));
        return token switch { "Supplier.ReferenceNumber" => s.ReferenceNumber, "Supplier.LegalName" => s.LegalName, "Supplier.Status" => s.Status.ToString(), _ => c.Values.GetValueOrDefault(token) ?? Quoted(token) };
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
    async Task<string> Reference(string entityType, Guid entityId, CancellationToken ct) => entityType == nameof(Supplier) ? (await db.Suppliers.SingleAsync(s => s.Id == entityId, ct)).ReferenceNumber : entityId.ToString();
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
    Task<List<CostCentre>> GetCostCentresAsync(CancellationToken ct = default);
    Task<CostCentre> CreateCostCentreAsync(CreateCostCentreDto dto, CancellationToken ct = default);
    Task<List<ProcurementCategory>> GetProcurementCategoriesAsync(CancellationToken ct = default);
    Task<ProcurementCategory> CreateProcurementCategoryAsync(CreateProcurementCategoryDto dto, CancellationToken ct = default);
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
    public Task<List<CostCentre>> GetCostCentresAsync(CancellationToken ct = default) => db.CostCentres.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    public async Task<CostCentre> CreateCostCentreAsync(CreateCostCentreDto dto, CancellationToken ct = default) { var c = new CostCentre(dto.Code, dto.Name, dto.Department); db.CostCentres.Add(c); await db.SaveChangesAsync(ct); return c; }
    public Task<List<ProcurementCategory>> GetProcurementCategoriesAsync(CancellationToken ct = default) => db.ProcurementCategories.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    public async Task<ProcurementCategory> CreateProcurementCategoryAsync(CreateProcurementCategoryDto dto, CancellationToken ct = default) { var c = new ProcurementCategory(dto.Code, dto.Name); db.ProcurementCategories.Add(c); await db.SaveChangesAsync(ct); return c; }
}
