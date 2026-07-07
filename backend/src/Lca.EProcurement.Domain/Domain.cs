namespace Lca.EProcurement.Domain;

public enum SupplierStatus { Draft, Submitted, UnderVerification, Approved, Rejected, Suspended, Blacklisted }
public enum WorkflowTaskStatus { Open, Assigned, Completed, Cancelled }
public enum WorkflowInstanceStatus { Running, Completed, Cancelled }
public enum WorkflowVersionStatus { Draft, Published, Archived }
public enum WorkflowNodeKind { Start, Task, Automatic, End }
public enum WorkflowActionKind { Transition, TaskAssignment, TaskCompletion, Cancellation }
public enum RuleOutcome { Passed, Failed }
public enum BusinessProcessStatus { Draft, Published, Archived }
public enum BusinessRuleStatus { Draft, Published, Archived }
public enum RequisitionStatus { Draft, Submitted, BudgetValidation, ManagerApproval, ProcurementReview, Approved, Rejected }
public enum TenderType { RFP, RFQ, RFI }
public enum TenderStatus { Draft, Published, Clarification, Cancelled, Closed }
public enum BidSubmissionStatus { Draft, Submitted, Locked, Withdrawn, Opened, Evaluated, Awarded, Rejected }

public enum MetadataStatus { Draft, Active, Inactive, Archived }
public enum PageType { Dashboard, DataGrid, DetailPage, Form, Wizard, Report, Timeline, Kanban, Calendar, MasterDetail, SplitView }

public abstract record MetadataEntity(Guid Id, string Code, string Name, string Description, int Version, MetadataStatus Status, DateTimeOffset Created, DateTimeOffset? Modified, string CreatedBy, string? ModifiedBy) : Entity(Id);
public record Application(string Code, string Name, string Description, string Icon = "AppWindow", string Theme = "LCA Default", string DefaultLandingPage = "/app/dashboard", string NavigationRoot = "/app", string ModulesJson = "[]", int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record BusinessProcess(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record EntityDefinition(string Code, string Name, string Description, string DisplayName, string PluralName, string DefaultSearchField, string PropertiesJson = "[]", string RelationshipsJson = "[]", string ValidationsJson = "[]", int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy)
{
    public List<DynamicEntityRecord> Records { get; init; } = [];
}
public record DynamicEntityRecord(Guid EntityDefinitionId, string EntityCode, string DataJson, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : Entity(Guid.NewGuid());
public record PageDefinition(string Code, string Name, string Description, Guid? ApplicationId, PageType PageType, string Route, string Icon, string DatasourceJson = "{}", Guid? LayoutId = null, string LayoutJson = "{}", string ToolbarJson = "[]", string ActionsJson = "[]", string FiltersJson = "[]", string ColumnsJson = "[]", string ComponentsJson = "[]", string PermissionsJson = "[]", string NavigationJson = "{}", Guid? PublishedVersionId = null, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record LayoutDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record ComponentDefinition(string Code, string Name, string Description, string Category = "General", string RendererKey = "", string PropertiesJson = "[]", string EventsJson = "[]", string ValidationJson = "[]", string DesignMetadataJson = "{}", int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy);
public record NavigationDefinition(string Code, string Name, string Description, int Version = 1, MetadataStatus Status = MetadataStatus.Draft, DateTimeOffset Created = default, DateTimeOffset? Modified = null, string CreatedBy = "system", string? ModifiedBy = null) : MetadataEntity(Guid.NewGuid(), Code, Name, Description, Version, Status, Created == default ? DateTimeOffset.UtcNow : Created, Modified, CreatedBy, ModifiedBy)
{
    public List<NavigationItem> Items { get; init; } = [];
}
public record NavigationItem(Guid NavigationDefinitionId, string Code, string Label, string ItemType, string? Url, string Icon, int DisplayOrder, Guid? ParentId = null, bool IsCollapsible = false, bool IsExpandedByDefault = true, string PermissionsJson = "[]", string VisibilityRule = "", bool IsVisible = true) : Entity(Guid.NewGuid())
{
    public List<NavigationItem> Children { get; init; } = [];
}
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

public record Tender(string TenderNumber, string Title, string Description, TenderType TenderType, string ProcurementMethod, TenderStatus Status, DateTimeOffset? PublicationDate, DateTimeOffset ClosingDate, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid())
{
    public List<TenderLot> Lots { get; init; } = [];
    public List<TenderDocument> Documents { get; init; } = [];
    public List<TenderSupplierInvitation> SupplierInvitations { get; init; } = [];
    public List<TenderClarification> Clarifications { get; init; } = [];
    public List<TenderStatusHistory> StatusHistory { get; init; } = [];
}
public record TenderLot(Guid TenderId, string LotNumber, string Title, string Description) : Entity(Guid.NewGuid());
public record TenderDocument(Guid TenderId, string DocumentType, string FileName, string Description, bool IsRequired, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid());
public record TenderSupplierInvitation(Guid TenderId, Guid? SupplierId, string SupplierName, string SupplierEmail, DateTimeOffset InvitedAt, string InvitedBy, DateTimeOffset? NotifiedAt = null) : Entity(Guid.NewGuid());
public record TenderClarification(Guid TenderId, string Question, string AskedBy, DateTimeOffset AskedAt, bool IsPublic = true) : Entity(Guid.NewGuid())
{
    public List<TenderClarificationResponse> Responses { get; init; } = [];
}
public record TenderClarificationResponse(Guid TenderClarificationId, string Response, string RespondedBy, DateTimeOffset RespondedAt) : Entity(Guid.NewGuid());
public record TenderStatusHistory(Guid TenderId, TenderStatus FromStatus, TenderStatus ToStatus, string Actor, DateTimeOffset ChangedAt, string Notes) : Entity(Guid.NewGuid());

public record BidSubmission(string SubmissionNumber, Guid TenderId, Guid SupplierId, BidSubmissionStatus Status, DateTimeOffset? SubmissionDate, string SubmittedBy, DateTimeOffset? SubmittedAt = null, DateTimeOffset? WithdrawnAt = null, DateTimeOffset? LockedAt = null, DateTimeOffset? OpenedAt = null, int CurrentVersion = 1) : Entity(Guid.NewGuid())
{
    public List<BidSubmissionItem> Items { get; init; } = [];
    public List<BidSubmissionDocument> Documents { get; init; } = [];
    public List<BidSubmissionHistory> History { get; init; } = [];
    public List<BidSubmissionDeclaration> Declarations { get; init; } = [];
    public List<BidSubmissionVersion> Versions { get; init; } = [];
    public List<BidSubmissionStatusHistory> StatusHistory { get; init; } = [];
}
public record BidSubmissionItem(Guid BidSubmissionId, Guid? TenderLotId, string Description, decimal Quantity, decimal UnitPrice, decimal Total, string? Notes = null) : Entity(Guid.NewGuid());
public record BidSubmissionDocument(Guid BidSubmissionId, string DocumentType, string Filename, string StorageReference, string UploadedBy, DateTimeOffset UploadedAt, int Version = 1) : Entity(Guid.NewGuid());
public record BidSubmissionHistory(Guid BidSubmissionId, string EventType, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());
public record BidSubmissionDeclaration(Guid BidSubmissionId, string DeclarationType, bool Accepted, string AcceptedBy, DateTimeOffset AcceptedAt) : Entity(Guid.NewGuid());
public record BidSubmissionVersion(Guid BidSubmissionId, int VersionNumber, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid());
public record BidSubmissionStatusHistory(Guid BidSubmissionId, BidSubmissionStatus FromStatus, BidSubmissionStatus ToStatus, string Actor, DateTimeOffset ChangedAt, string Notes) : Entity(Guid.NewGuid());

public record WorkflowDefinition(string Code, string Name, string EntityType, bool IsActive = true, Guid? PublishedVersionId = null) : Entity(Guid.NewGuid())
{
    public List<WorkflowVersion> Versions { get; init; } = [];
}

public record WorkflowVersion(Guid WorkflowDefinitionId, int VersionNumber, WorkflowVersionStatus Status = WorkflowVersionStatus.Draft, DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid())
{
    public List<WorkflowNode> Nodes { get; init; } = [];
    public List<WorkflowTransition> Transitions { get; init; } = [];
}

public record WorkflowNode(Guid WorkflowVersionId, string Code, string Name, WorkflowNodeKind Kind, bool CreatesTask = false, string? DefaultAssignedRole = null, bool IsStart = false, bool IsTerminal = false, int PositionX = 0, int PositionY = 0, string ActionConfigurationJson = "{}", string ConditionConfigurationJson = "{}", string BusinessRuleCodesJson = "[]", string AssignedRolesJson = "[]") : Entity(Guid.NewGuid());
public record WorkflowTransition(Guid WorkflowVersionId, string FromNodeCode, string ActionCode, string ActionName, string ToNodeCode, string? RequiredRuleCode = null, string ConditionExpression = "", string ActionConfigurationJson = "{}", string BusinessRuleCodesJson = "[]", string AssignedRolesJson = "[]") : Entity(Guid.NewGuid());
public record WorkflowAction(Guid WorkflowInstanceId, string ActionCode, string ActionName, WorkflowActionKind Kind, string FromNodeCode, string ToNodeCode, string Actor, DateTimeOffset ActionedAt, Guid? WorkflowTaskId = null, string? Details = null) : Entity(Guid.NewGuid());
public record WorkflowInstance(Guid WorkflowDefinitionId, Guid WorkflowVersionId, string EntityType, Guid EntityId, string CurrentNodeCode, WorkflowInstanceStatus Status = WorkflowInstanceStatus.Running, DateTimeOffset StartedAt = default, DateTimeOffset? CompletedAt = null, DateTimeOffset? CancelledAt = null) : Entity(Guid.NewGuid());
public record WorkflowTask(Guid WorkflowInstanceId, string NodeCode, string? AssignedRole, string? AssignedTo = null, WorkflowTaskStatus Status = WorkflowTaskStatus.Open, DateTimeOffset CreatedAt = default, DateTimeOffset? AssignedAt = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid());
public record WorkflowHistory(Guid WorkflowInstanceId, string EventType, string NodeCode, string Actor, string Details, DateTimeOffset OccurredAt, Guid? WorkflowTaskId = null) : Entity(Guid.NewGuid());

public record BusinessRuleDefinition(string Code, string Name, string AppliesTo, string Expression, bool IsActive = true, string Category = "General", BusinessRuleStatus Status = BusinessRuleStatus.Draft, string FailureMessage = "Rule failed", DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid());
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

public record AnnualProcurementPlan(string PlanNumber, string Title, Guid FinancialYearId, string Department, string Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? SubmittedAt = null, DateTimeOffset? ApprovedAt = null) : Entity(Guid.NewGuid())
{
    public List<ProcurementPlanItem> Items { get; init; } = [];
}
public record ProcurementPlanItem(Guid AnnualProcurementPlanId, string ItemCode, string Description, Guid ProcurementCategoryId, decimal EstimatedAmount, string PlannedQuarter, string ProcurementMethod, string Status) : Entity(Guid.NewGuid());
public record Budget(Guid FinancialYearId, string Department, decimal TotalAmount, decimal CommittedAmount, decimal AvailableAmount) : Entity(Guid.NewGuid())
{
    public List<BudgetLine> Lines { get; init; } = [];
}
public record BudgetLine(Guid BudgetId, Guid CostCentreId, Guid ProcurementCategoryId, decimal AllocatedAmount, decimal CommittedAmount, decimal AvailableAmount) : Entity(Guid.NewGuid());
public record CostCentre(string Code, string Name, string Department, bool IsActive = true) : Entity(Guid.NewGuid());
public record ProcurementCategory(string Code, string Name, bool IsActive = true) : Entity(Guid.NewGuid());
public record FinancialYear(string Code, DateTimeOffset StartDate, DateTimeOffset EndDate, bool IsActive = true) : Entity(Guid.NewGuid());

public record Requisition(string RequisitionNumber, string Title, string Description, string Department, Guid CostCentreId, Guid FinancialYearId, string RequestedBy, DateTimeOffset RequiredDate, string Priority, decimal EstimatedTotal, RequisitionStatus Status, DateTimeOffset CreatedAt, DateTimeOffset? SubmittedAt = null, DateTimeOffset? ApprovedAt = null, DateTimeOffset? RejectedAt = null) : Entity(Guid.NewGuid())
{
    public List<RequisitionItem> Items { get; init; } = [];
    public List<RequisitionAttachment> Attachments { get; init; } = [];
    public List<RequisitionStatusHistory> StatusHistory { get; init; } = [];
}
public record RequisitionItem(Guid RequisitionId, string Description, decimal Quantity, string UnitOfMeasure, decimal EstimatedUnitPrice, decimal EstimatedTotal, Guid ProcurementCategoryId, Guid? ProcurementPlanItemId = null) : Entity(Guid.NewGuid());
public record RequisitionAttachment(Guid RequisitionId, string FileName, string ContentType, string UploadedBy, DateTimeOffset UploadedAt, string StorageReference) : Entity(Guid.NewGuid());
public record RequisitionStatusHistory(Guid RequisitionId, RequisitionStatus FromStatus, RequisitionStatus ToStatus, string Actor, string Notes, DateTimeOffset ChangedAt) : Entity(Guid.NewGuid());
public record BudgetCommitment(Guid RequisitionId, Guid BudgetId, Guid BudgetLineId, Guid FinancialYearId, Guid CostCentreId, Guid ProcurementCategoryId, decimal Amount, string CommittedBy, DateTimeOffset CommittedAt, string Reference) : Entity(Guid.NewGuid());
