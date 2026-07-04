namespace Lca.EProcurement.Domain;

public enum SupplierStatus { Draft, Submitted, UnderVerification, Approved, Rejected, Suspended, Blacklisted }
public enum WorkflowTaskStatus { Open, Completed, Cancelled }
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

public record WorkflowDefinition(string Code, string Name, bool IsActive = true) : Entity(Guid.NewGuid()) { public List<WorkflowStepDefinition> Steps { get; init; } = []; public List<WorkflowTransition> Transitions { get; init; } = []; }
public record WorkflowStepDefinition(Guid WorkflowDefinitionId, string Code, string Name, bool CreatesTask) : Entity(Guid.NewGuid());
public record WorkflowTransition(Guid WorkflowDefinitionId, string FromStepCode, string Action, string ToStepCode, string? RequiredRuleCode = null) : Entity(Guid.NewGuid());
public record WorkflowInstance(Guid WorkflowDefinitionId, string EntityType, Guid EntityId, string CurrentStepCode, bool IsComplete = false) : Entity(Guid.NewGuid());
public record WorkflowTask(Guid WorkflowInstanceId, string StepCode, string AssignedRole, WorkflowTaskStatus Status = WorkflowTaskStatus.Open) : Entity(Guid.NewGuid());
public record WorkflowAction(Guid WorkflowInstanceId, string Action, string FromStepCode, string ToStepCode, string Actor, DateTimeOffset ActionedAt) : Entity(Guid.NewGuid());

public record BusinessRuleDefinition(string Code, string Name, string AppliesTo, string Expression, bool IsActive = true) : Entity(Guid.NewGuid());
public record BusinessRuleExecutionLog(string RuleCode, string EntityType, Guid EntityId, string InputJson, RuleOutcome Outcome, string ResultJson, DateTimeOffset ExecutedAt) : Entity(Guid.NewGuid());

public record AuditEvent(string EventType, string EntityType, Guid EntityId, string EntityReference, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());
