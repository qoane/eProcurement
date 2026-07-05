namespace Lca.EProcurement.Domain;

public enum SupplierStatus { Draft, Submitted, UnderVerification, Approved, Rejected, Suspended, Blacklisted }
public enum WorkflowTaskStatus { Open, Assigned, Completed, Cancelled }
public enum WorkflowInstanceStatus { Running, Completed, Cancelled }
public enum WorkflowVersionStatus { Draft, Published, Archived }
public enum WorkflowNodeKind { Start, Task, Automatic, End }
public enum WorkflowActionKind { Transition, TaskAssignment, TaskCompletion, Cancellation }
public enum RuleOutcome { Passed, Failed }
public enum BusinessProcessStatus { Draft, Published, Archived }

public enum MetadataStatus { Draft, Active, Inactive, Archived }

public abstract record MetadataEntity(Guid Id, string Code, string Name, string Description, int Version, MetadataStatus Status, DateTimeOffset Created, DateTimeOffset? Modified, string CreatedBy, string? ModifiedBy) : Entity(Id);
public record Application(string Code, string Name, string Description, string Icon = "AppWindow", string Theme = "LCA Default", string DefaultLandingPage = "/app/dashboard", string NavigationRoot = "/app", string ModulesJson = "[]", int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record BusinessProcess(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record EntityDefinition(string Code, string Name, string Description, string DisplayName, string PluralName, string DefaultSearchField, string PropertiesJson = "[]", string RelationshipsJson = "[]", string ValidationsJson = "[]", int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record PageDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record LayoutDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record ComponentDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record NavigationDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record MenuDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record DashboardDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record ReportDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record ThemeDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record LookupDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record DocumentTypeDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record SystemSetting(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);


public interface IEntity { Guid Id { get; init; } }
public abstract record Entity(Guid Id) : IEntity;

public record SeedMetadata(string Kind, string Code, string Name) : Entity(Guid.NewGuid());

public record Supplier(string ReferenceNumber, string LegalName, SupplierStatus Status) : Entity(Guid.NewGuid())
{
    public List<SupplierDocument> Documents { get; init; } = [];
    public List<SupplierCategory> Categories { get; init; } = [];
    public List<SupplierPerformanceRating> PerformanceRatings { get; init; } = [];
}
public record SupplierDocument(Guid SupplierId, string DocumentType, string FileName, string UploadedBy, DateTimeOffset UploadedAt) : Entity(Guid.NewGuid());
public record SupplierCategory(string Name) : Entity(Guid.NewGuid());
public record SupplierPerformanceRating(Guid SupplierId, int Score, string Notes, DateTimeOffset RatedAt) : Entity(Guid.NewGuid());

public record WorkflowDefinition(string Code, string Name, string EntityType, bool IsActive = true, Guid? PublishedVersionId = null) : Entity(Guid.NewGuid())
{
    public List<WorkflowVersion> Versions { get; init; } = [];
}

public record WorkflowVersion(Guid WorkflowDefinitionId, int VersionNumber, WorkflowVersionStatus Status = WorkflowVersionStatus.Draft, DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid())
{
    public List<WorkflowNode> Nodes { get; init; } = [];
    public List<WorkflowTransition> Transitions { get; init; } = [];
}

public record WorkflowNode(Guid WorkflowVersionId, string Code, string Name, WorkflowNodeKind Kind, bool CreatesTask = false, string? DefaultAssignedRole = null, bool IsStart = false, bool IsTerminal = false) : Entity(Guid.NewGuid());
public record WorkflowTransition(Guid WorkflowVersionId, string FromNodeCode, string ActionCode, string ActionName, string ToNodeCode, string? RequiredRuleCode = null) : Entity(Guid.NewGuid());
public record WorkflowAction(Guid WorkflowInstanceId, string ActionCode, string ActionName, WorkflowActionKind Kind, string FromNodeCode, string ToNodeCode, string Actor, DateTimeOffset ActionedAt, Guid? WorkflowTaskId = null, string? Details = null) : Entity(Guid.NewGuid());
public record WorkflowInstance(Guid WorkflowDefinitionId, Guid WorkflowVersionId, string EntityType, Guid EntityId, string CurrentNodeCode, WorkflowInstanceStatus Status = WorkflowInstanceStatus.Running, DateTimeOffset StartedAt = default, DateTimeOffset? CompletedAt = null, DateTimeOffset? CancelledAt = null) : Entity(Guid.NewGuid());
public record WorkflowTask(Guid WorkflowInstanceId, string NodeCode, string? AssignedRole, string? AssignedTo = null, WorkflowTaskStatus Status = WorkflowTaskStatus.Open, DateTimeOffset CreatedAt = default, DateTimeOffset? AssignedAt = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid());
public record WorkflowHistory(Guid WorkflowInstanceId, string EventType, string NodeCode, string Actor, string Details, DateTimeOffset OccurredAt, Guid? WorkflowTaskId = null) : Entity(Guid.NewGuid());

public record BusinessRuleDefinition(string Code, string Name, string AppliesTo, string Expression, bool IsActive = true) : Entity(Guid.NewGuid());
public record BusinessRuleExecutionLog(string RuleCode, string EntityType, Guid EntityId, string InputJson, RuleOutcome Outcome, string ResultJson, DateTimeOffset ExecutedAt) : Entity(Guid.NewGuid());

public record AuditEvent(string EventType, string EntityType, Guid EntityId, string EntityReference, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());


public record BusinessProcessDefinition(string Code, string Name, string Description, string EntityType, Guid? ActiveWorkflowDefinitionId = null, Guid? ActiveFormDefinitionId = null, Guid? ActiveDocumentRequirementSetId = null, Guid? ActiveApprovalMatrixId = null, BusinessProcessStatus Status = BusinessProcessStatus.Draft) : Entity(Guid.NewGuid());
public record DocumentRequirementSet(string Name, string Description, string EntityType) : Entity(Guid.NewGuid())
{
    public List<DocumentRequirement> Requirements { get; init; } = [];
}
public record DocumentRequirement(Guid DocumentRequirementSetId, string DocumentType, bool Required, int MinimumFiles, int MaximumFiles, string AllowedExtensions, long MaximumFileSize, string? RuleCode = null) : Entity(Guid.NewGuid());
public record ApprovalMatrix(string Name, string Description, string EntityType) : Entity(Guid.NewGuid())
{
    public List<ApprovalStep> Steps { get; init; } = [];
}
public record ApprovalStep(Guid ApprovalMatrixId, string Role, int Sequence, decimal? MinimumAmount = null, decimal? MaximumAmount = null, string? RuleCode = null) : Entity(Guid.NewGuid());

public record WorkflowTransitionEffect(string EntityType, string PropertyName, string ValueExpression, Guid TriggerTransitionId) : Entity(Guid.NewGuid());
public record WorkflowMapping(string EntityType, string ActionCode, string WorkflowCode, bool IsActive = true) : Entity(Guid.NewGuid());
public record DocumentTypeRequirement(string EntityType, string DocumentType, string Name, bool IsRequired = true) : Entity(Guid.NewGuid());
public record LookupValue(string LookupType, string Code, string Name, int DisplayOrder = 0, bool IsActive = true) : Entity(Guid.NewGuid());

public record FormDefinition(string Code, string Name, string EntityType, bool IsActive = true, Guid? ActiveVersionId = null) : Entity(Guid.NewGuid())
{
    public List<FormVersion> Versions { get; init; } = [];
}
public record FormVersion(Guid FormDefinitionId, int VersionNumber, WorkflowVersionStatus Status = WorkflowVersionStatus.Draft, DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid())
{
    public List<FormSection> Sections { get; init; } = [];
}
public record FormSection(Guid FormVersionId, string Code, string Title, int DisplayOrder) : Entity(Guid.NewGuid())
{
    public List<FormField> Fields { get; init; } = [];
}
public record FormField(Guid FormSectionId, string Code, string Label, string FieldType, int DisplayOrder, bool IsRequired = false) : Entity(Guid.NewGuid())
{
    public List<FormFieldValidation> Validations { get; init; } = [];
    public List<FormFieldVisibilityRule> VisibilityRules { get; init; } = [];
}
public record FormFieldValidation(Guid FormFieldId, string ValidationType, string? ConfigurationJson, string Message) : Entity(Guid.NewGuid());
public record FormFieldVisibilityRule(Guid FormFieldId, string Expression) : Entity(Guid.NewGuid());
public record FormSubmission(Guid FormDefinitionId, Guid FormVersionId, string EntityType, Guid EntityId, string SubmittedBy, DateTimeOffset SubmittedAt) : Entity(Guid.NewGuid())
{
    public List<FormSubmissionValue> Values { get; init; } = [];
}
public record FormSubmissionValue(Guid FormSubmissionId, string FieldCode, string? Value) : Entity(Guid.NewGuid());
