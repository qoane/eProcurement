namespace Lca.EProcurement.Domain;

public enum SupplierStatus { Draft, Submitted, UnderVerification, Approved, Rejected, Suspended, Blacklisted }
public enum WorkflowTaskStatus { Open, Assigned, Completed, Cancelled }
public enum WorkflowInstanceStatus { Running, Completed, Cancelled }
public enum WorkflowVersionStatus { Draft, Published, Archived }
public enum WorkflowNodeKind { Start, Task, Automatic, End }
public enum WorkflowActionKind { Transition, TaskAssignment, TaskCompletion, Cancellation }
public enum RuleOutcome { Passed, Failed }

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
