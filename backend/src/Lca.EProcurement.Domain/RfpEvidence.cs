namespace Lca.EProcurement.Domain;

public enum ComplianceSource { RFP, TechnicalProposal, Internal }
public enum ComplianceRequirementType { Functional, Technical, Security, Integration, Reporting, Operational, Implementation, Training, Support, Governance }
public enum ComplianceStatus { Covered, PartiallyCovered, NotCovered, NeedsConfiguration, NotApplicable }
public enum UatResultStatus { Pass, Fail, Blocked, NotRun }
public enum ReadinessStatus { Ready, MostlyReady, NeedsAttention, NotReady }
public enum TrainingAudience { ProcurementOfficer, FinanceUser, Approver, Evaluator, Auditor, Supplier, SystemAdministrator, Management }

public record ComplianceRequirement(string RequirementCode, ComplianceSource Source, string Section, string RequirementArea, string RequirementText, ComplianceRequirementType RequirementType, string Priority, ComplianceStatus Status, string SystemModule, string Route, string ApiEndpoint, string EvidenceNotes, string Owner, DateTimeOffset? LastVerifiedAt, string? VerifiedBy, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt) : Entity(Guid.NewGuid());
public record ProposalCommitment(string CommitmentCode, string Section, string CommitmentText, string SystemFeature, string Module, string Route, string ApiEndpoint, ComplianceStatus Status, string EvidenceNotes, DateTimeOffset? LastVerifiedAt) : Entity(Guid.NewGuid());
public record DemoStep(int StepNumber, string Title, string Description, string Role, string Route, string EntityReference, string TalkingPoints, string ExpectedOutcome, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt) : Entity(Guid.NewGuid());
public record UatTestSuite(string Code, string Name, string Description, string Module, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid()) { public List<UatTestCase> TestCases { get; init; } = []; }
public record UatTestCase(Guid SuiteId, string TestCaseNumber, string Title, string Preconditions, string Steps, string ExpectedResult, string RequirementCode, string Priority, string Status) : Entity(Guid.NewGuid());
public record UatTestRun(string RunNumber, Guid SuiteId, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt, string Status, string ExecutedBy, string Notes) : Entity(Guid.NewGuid()) { public List<UatTestResult> Results { get; init; } = []; }
public record UatTestResult(Guid RunId, Guid TestCaseId, UatResultStatus Result, string ActualResult, string EvidenceNotes, string? DefectReference, DateTimeOffset ExecutedAt, string ExecutedBy) : Entity(Guid.NewGuid());
public record TrainingModule(string Code, string Name, string Description, TrainingAudience Audience, string ModuleArea, int EstimatedMinutes, string Status, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid()) { public List<TrainingLesson> Lessons { get; init; } = []; }
public record TrainingLesson(Guid TrainingModuleId, string Title, string Content, string Route, string? VideoUrl, string? DocumentUrl, int DisplayOrder) : Entity(Guid.NewGuid());
public record TrainingCompletion(Guid TrainingModuleId, string UserId, DateTimeOffset CompletedAt) : Entity(Guid.NewGuid());
public record ImplementationPhase(string Code, string Name, string Description, DateTimeOffset StartDate, DateTimeOffset EndDate, string Status) : Entity(Guid.NewGuid()) { public List<ImplementationMilestone> Milestones { get; init; } = []; }
public record ImplementationMilestone(Guid PhaseId, string Name, string Description, DateTimeOffset DueDate, string Status, string Owner, DateTimeOffset? CompletedAt) : Entity(Guid.NewGuid()) { public List<ImplementationTask> Tasks { get; init; } = []; }
public record ImplementationTask(Guid MilestoneId, string Title, string Description, string Owner, string Status, DateTimeOffset DueDate, DateTimeOffset? CompletedAt) : Entity(Guid.NewGuid());
public record SupportServiceLevel(string Code, string Severity, int ResponseTimeHours, int ResolutionTargetHours, string EscalationRole, bool IsActive) : Entity(Guid.NewGuid());
public record HandoverChecklist(string Code, string Name, string Description, string Status, DateTimeOffset CreatedAt, DateTimeOffset? CompletedAt) : Entity(Guid.NewGuid()) { public List<HandoverChecklistItem> Items { get; init; } = []; }
public record HandoverChecklistItem(Guid ChecklistId, string Category, string Description, bool Required, string Status, string EvidenceRoute, string EvidenceNotes, string? CompletedBy, DateTimeOffset? CompletedAt) : Entity(Guid.NewGuid());
