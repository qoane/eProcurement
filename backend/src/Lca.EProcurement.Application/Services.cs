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
    Task<PlatformDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<WorkflowInstance> StartAsync(string workflowCode, string entityType, Guid entityId, string actor, CancellationToken ct = default);
    Task<WorkflowInstance?> ExecuteActionAsync(Guid instanceId, string actionCode, string actor, CancellationToken ct = default);
    Task<List<WorkflowTask>> GetTasksAsync(CancellationToken ct = default);
    Task<WorkflowTask?> AssignTaskAsync(Guid id, string assignedTo, string actor, CancellationToken ct = default);
    Task<WorkflowTask?> CompleteTaskAsync(Guid id, string actor, CancellationToken ct = default);
}
public interface IBusinessRuleApplicationService { Task<List<BusinessRuleDefinition>> GetRulesAsync(CancellationToken ct = default); Task<BusinessRuleDefinition> CreateRuleAsync(CreateBusinessRuleDto dto, CancellationToken ct = default); Task<RuleResult> EvaluateAsync(string ruleCode, string entityType, Guid entityId, string actor, CancellationToken ct = default); }
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
public sealed record WorkflowNodeDto(string Code, string Name, WorkflowNodeKind Kind, bool CreatesTask = false, string? DefaultAssignedRole = null, bool IsStart = false, bool IsTerminal = false);
public sealed record WorkflowTransitionDto(string FromNodeCode, string ActionCode, string ActionName, string ToNodeCode, string? RequiredRuleCode = null);
public sealed record WorkflowTransitionEffectDto(string EntityType, string PropertyName, string ValueExpression, string TriggerActionCode, string TriggerFromNodeCode);
public sealed record CreateFormDefinitionDto(string Code, string Name, string EntityType, List<FormSectionDto> Sections);
public sealed record FormSectionDto(string Code, string Title, int DisplayOrder, List<FormFieldDto> Fields);
public sealed record FormFieldDto(string Code, string Label, string FieldType, int DisplayOrder, bool IsRequired);
public sealed record SubmitFormDto(string FormCode, string EntityType, Guid EntityId, string SubmittedBy, Dictionary<string, string?> Values);
public sealed record CreateBusinessRuleDto(string Code, string Name, string AppliesTo, string Expression, bool IsActive = true);
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


public sealed class BusinessRuleApplicationService(EProcurementDbContext db) : IBusinessRuleApplicationService
{
    public Task<List<BusinessRuleDefinition>> GetRulesAsync(CancellationToken ct = default) => db.BusinessRuleDefinitions.AsNoTracking().OrderBy(r => r.Code).ToListAsync(ct);
    public async Task<BusinessRuleDefinition> CreateRuleAsync(CreateBusinessRuleDto dto, CancellationToken ct = default) { var rule = new BusinessRuleDefinition(dto.Code, dto.Name, dto.AppliesTo, dto.Expression, dto.IsActive); db.BusinessRuleDefinitions.Add(rule); await db.SaveChangesAsync(ct); return rule; }
    public async Task<RuleResult> EvaluateAsync(string ruleCode, string entityType, Guid entityId, string actor, CancellationToken ct = default)
    {
        var rule = await db.BusinessRuleDefinitions.SingleAsync(r => r.Code == ruleCode && r.IsActive, ct);
        object entity = entityType == nameof(Supplier)
            ? await db.Suppliers.Include(s => s.Documents).Include(s => s.Categories).SingleAsync(s => s.Id == entityId, ct)
            : throw new NotSupportedException($"Rules for entity type '{entityType}' are not configured.");
        var passed = SimpleExpressionEvaluator.Evaluate(rule.Expression, entity);
        var result = new RuleResult(rule.Code, passed, passed ? "Rule passed" : $"Rule failed: {rule.Name}");
        db.BusinessRuleExecutionLogs.Add(new BusinessRuleExecutionLog(rule.Code, entityType, entityId, JsonSerializer.Serialize(entity), passed ? RuleOutcome.Passed : RuleOutcome.Failed, JsonSerializer.Serialize(result), DateTimeOffset.UtcNow));
        db.AuditEvents.Add(new AuditEvent("Rule evaluated", entityType, entityId, EntityReference(entity), actor, result.Message, DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct);
        return result;
    }
    static string EntityReference(object e) => e is Supplier s ? s.ReferenceNumber : e.ToString() ?? string.Empty;
}

public static class SimpleExpressionEvaluator
{
    public static bool Evaluate(string expression, object entity)
    {
        if (entity is Supplier supplier && expression.StartsWith("Supplier.", StringComparison.Ordinal))
        {
            var body = expression["Supplier.".Length..];
            if (body.StartsWith("Documents.Any(", StringComparison.Ordinal)) return AnyEquals(body, supplier.Documents.Select(d => d.DocumentType), "DocumentType");
            if (body == "Categories.Any()") return supplier.Categories.Any();
            if (body.StartsWith("Status == ", StringComparison.Ordinal)) return string.Equals(supplier.Status.ToString(), Quoted(body[10..]), StringComparison.OrdinalIgnoreCase);
        }
        throw new InvalidOperationException($"Expression '{expression}' is not supported by the safe evaluator.");
    }
    static bool AnyEquals(string body, IEnumerable<string> values, string property) => values.Any(v => string.Equals(v, Quoted(body.Replace("Documents.Any(", "").TrimEnd(')').Replace(property + " == ", "")), StringComparison.OrdinalIgnoreCase));
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
        version.Nodes.AddRange(request.Nodes.Select(n => new WorkflowNode(version.Id, n.Code, n.Name, n.Kind, n.CreatesTask, n.DefaultAssignedRole, n.IsStart, n.IsTerminal)));
        version.Transitions.AddRange(request.Transitions.Select(t => new WorkflowTransition(version.Id, t.FromNodeCode, t.ActionCode, t.ActionName, t.ToNodeCode, t.RequiredRuleCode)));
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
        var transition = new WorkflowTransition(version.Id, dto.FromNodeCode, dto.ActionCode, dto.ActionName, dto.ToNodeCode, dto.RequiredRuleCode);
        db.WorkflowTransitions.Add(transition); await db.SaveChangesAsync(ct); return transition;
    }
    public async Task<WorkflowDefinition?> PublishAsync(string code, string actor, CancellationToken ct = default) { var d = await db.WorkflowDefinitions.Include(x => x.Versions).SingleOrDefaultAsync(x => x.Code == code, ct); var v = d?.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault(x => x.Status == WorkflowVersionStatus.Draft); if (d is null || v is null) return null; db.Entry(v).CurrentValues[nameof(WorkflowVersion.Status)] = WorkflowVersionStatus.Published; db.Entry(v).CurrentValues[nameof(WorkflowVersion.PublishedAt)] = DateTimeOffset.UtcNow; db.Entry(v).CurrentValues[nameof(WorkflowVersion.PublishedBy)] = actor; db.Entry(d).CurrentValues[nameof(WorkflowDefinition.PublishedVersionId)] = v.Id; await db.SaveChangesAsync(ct); return d; }
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

public sealed class SupplierApplicationService(EProcurementDbContext db, IWorkflowApplicationService workflows, IDynamicFormApplicationService forms) : ISupplierApplicationService
{
    public async Task<List<object>> GetSuppliersAsync(CancellationToken ct = default) => await db.Suppliers.AsNoTracking().Include(s => s.Documents).Include(s => s.Categories).OrderBy(s => s.ReferenceNumber).Select(s => new { s.Id, s.ReferenceNumber, s.LegalName, Status = s.Status.ToString(), Documents = s.Documents, Categories = s.Categories.Select(c => c.Name) }).Cast<object>().ToListAsync(ct);

    public async Task<SupplierRegistrationConfigurationDto?> GetRegistrationConfigurationAsync(CancellationToken ct = default)
    {
        var process = await ActiveSupplierProcess(ct);
        if (process?.ActiveFormDefinitionId is null || process.ActiveWorkflowDefinitionId is null || process.ActiveDocumentRequirementSetId is null) return null;
        var form = await db.FormDefinitions.AsNoTracking().Include(x => x.Versions.Where(v => v.Id == x.ActiveVersionId || v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).SingleAsync(x => x.Id == process.ActiveFormDefinitionId, ct);
        var docs = await db.DocumentRequirementSets.AsNoTracking().Include(x => x.Requirements).SingleAsync(x => x.Id == process.ActiveDocumentRequirementSetId, ct);
        var matrix = process.ActiveApprovalMatrixId is null ? null : await db.ApprovalMatrices.AsNoTracking().Include(x => x.Steps).SingleAsync(x => x.Id == process.ActiveApprovalMatrixId, ct);
        var workflow = await db.WorkflowDefinitions.AsNoTracking().Include(x => x.Versions.Where(v => v.Id == x.PublishedVersionId || v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Nodes).Include(x => x.Versions.Where(v => v.Id == x.PublishedVersionId || v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Transitions).SingleAsync(x => x.Id == process.ActiveWorkflowDefinitionId, ct);
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
    public Task<List<FormDefinition>> GetDefinitionsAsync(CancellationToken ct = default) => db.FormDefinitions.AsNoTracking().Include(f => f.Versions).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).OrderBy(f => f.Code).ToListAsync(ct);
    public async Task<FormDefinition> CreateDefinitionAsync(CreateFormDefinitionDto dto, CancellationToken ct = default)
    {
        var def = new FormDefinition(dto.Code, dto.Name, dto.EntityType); var version = new FormVersion(def.Id, 1);
        foreach (var s in dto.Sections) { var section = new FormSection(version.Id, s.Code, s.Title, s.DisplayOrder); section.Fields.AddRange(s.Fields.Select(f => new FormField(section.Id, f.Code, f.Label, f.FieldType, f.DisplayOrder, f.IsRequired))); version.Sections.Add(section); }
        def.Versions.Add(version); db.FormDefinitions.Add(def); await db.SaveChangesAsync(ct); return def;
    }
    public async Task<FormSection?> AddSectionAsync(string formCode, FormSectionDto dto, CancellationToken ct = default)
    {
        var version = await DraftFormVersion(formCode, ct);
        if (version is null) return null;
        var section = new FormSection(version.Id, dto.Code, dto.Title, dto.DisplayOrder);
        section.Fields.AddRange(dto.Fields.Select(f => new FormField(section.Id, f.Code, f.Label, f.FieldType, f.DisplayOrder, f.IsRequired)));
        db.FormSections.Add(section); await db.SaveChangesAsync(ct); return section;
    }
    public async Task<FormField?> AddFieldAsync(string formCode, string sectionCode, FormFieldDto dto, CancellationToken ct = default)
    {
        var version = await DraftFormVersion(formCode, ct);
        if (version is null) return null;
        var section = await db.FormSections.SingleOrDefaultAsync(s => s.FormVersionId == version.Id && s.Code == sectionCode, ct);
        if (section is null) return null;
        var field = new FormField(section.Id, dto.Code, dto.Label, dto.FieldType, dto.DisplayOrder, dto.IsRequired);
        db.FormFields.Add(field); await db.SaveChangesAsync(ct); return field;
    }
    public async Task<FormDefinition?> PublishVersionAsync(string code, string actor, CancellationToken ct = default) { var d = await db.FormDefinitions.Include(x => x.Versions).SingleOrDefaultAsync(x => x.Code == code, ct); var v = d?.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault(x => x.Status == WorkflowVersionStatus.Draft); if (d is null || v is null) return null; db.Entry(v).CurrentValues[nameof(FormVersion.Status)] = WorkflowVersionStatus.Published; db.Entry(v).CurrentValues[nameof(FormVersion.PublishedAt)] = DateTimeOffset.UtcNow; db.Entry(v).CurrentValues[nameof(FormVersion.PublishedBy)] = actor; db.Entry(d).CurrentValues[nameof(FormDefinition.ActiveVersionId)] = v.Id; await db.SaveChangesAsync(ct); return d; }
    public Task<FormDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default) => db.FormDefinitions.AsNoTracking().Include(d => d.Versions.Where(v => v.Status == WorkflowVersionStatus.Published)).ThenInclude(v => v.Sections).ThenInclude(s => s.Fields).SingleOrDefaultAsync(d => d.Code == code && d.IsActive, ct);
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
