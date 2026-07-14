namespace Lca.EProcurement.Domain;

public enum SupplierStatus { Draft, Submitted, UnderVerification, Approved, Rejected, Suspended, Blacklisted }
public enum SupplierAccountState { PendingVerification, EmailVerified, ProfileSubmitted, UnderReview, Approved, Rejected, Suspended }
public enum MfaMethod { EmailOtp, SmsOtp, AuthenticatorApp }
public enum IdentityProviderType { OpenIdConnect, Saml2, ActiveDirectory, Ldap }
public enum WorkflowTaskStatus { Open, Assigned, Completed, Cancelled }
public enum WorkflowInstanceStatus { Running, Completed, Cancelled }
public enum WorkflowVersionStatus { Draft, Published, Archived }
public enum WorkflowNodeKind { Start, Task, Automatic, Decision, End }
public enum WorkflowActionKind { Transition, TaskAssignment, TaskCompletion, Cancellation }
public enum RuleOutcome { Passed, Failed }
public enum BusinessProcessStatus { Draft, Published, Archived }
public enum BusinessRuleStatus { Draft, Published, Archived }
public enum RequisitionStatus { Draft, Submitted, BudgetValidation, ManagerApproval, ProcurementReview, Approved, Rejected }
public enum TenderType { RFP, RFQ, RFI }
public enum TenderStatus { Draft, Published, Clarification, Cancelled, Closed }
public enum TenderClarificationStatus { Submitted, UnderReview, Answered, Published, Closed }
public enum TenderClarificationVisibility { Private, Public }
public enum BidSubmissionStatus { Draft, Submitted, Locked, Withdrawn, Opened, Evaluated, Awarded, Rejected }
public enum BidOpeningSessionStatus { Draft, Scheduled, InProgress, Completed, ReferredToEvaluation, Cancelled }
public enum BidOpeningSubmissionStatus { Pending, Opened, Late, Disqualified, ReferredToEvaluation }
public enum SealedBidEnvelopeStatus { Draft, Sealed, Opened, Rejected, Withdrawn }
public enum BidAccessType { ViewSummary, ViewPricing, ViewDocument, DownloadDocument, OpenBid, IntegrityCheck }
public enum EvaluationSessionStatus { Draft, Scheduled, InProgress, Consensus, Completed, Cancelled, ReferredToAward }
public enum EvaluationStage { Administrative, Technical, Financial, Consensus }
public enum EvaluationSubmissionStatus { Pending, Responsive, NonResponsive, Evaluated, Recommended, Rejected }
public enum AwardStatus { Draft, Recommended, UnderApproval, Approved, Rejected, Published, Cancelled, ConvertedToPurchaseOrder, ConvertedToContract }
public enum AwardDecisionStatus { Pending, Approved, Rejected, Deferred }
public enum PurchaseOrderStatus { Draft, Issued, Acknowledged, PartiallyDelivered, Delivered, Closed, Cancelled }
public enum ContractStatus { Draft, PendingApproval, Active, Suspended, Expired, Completed, Terminated, Cancelled }
public enum ContractType { FrameworkAgreement, ServiceContract, SupplyContract, Consultancy, WorksContract }
public enum IntegrationSystemType { ContractManagement, DocumentManagement, Finance, Email, Sms, Identity, Other }
public enum IntegrationDirection { Outbound, Inbound }
public enum IntegrationMessageStatus { Pending, Sent, Failed, PendingExternalConfiguration, Simulated }
public enum PurchaseOrderReturnStatus { Draft, Submitted, Approved, Rejected, Completed, Cancelled }
public enum SupplierInvoiceStatus { Draft, Submitted, UnderReview, Matched, Mismatch, ApprovedForPayment, Rejected, Cancelled }
public enum InvoiceMatchStatus { NotMatched, Matched, PartiallyMatched, Mismatch, Exception }
public enum PaymentPreparationStatus { Pending, Prepared, SentToFinance, Failed, Cancelled }

public enum MetadataStatus { Draft, Active, Inactive, Archived }
public enum PageType { Dashboard, DataGrid, DetailPage, Form, Wizard, Report, Timeline, Kanban, Calendar, MasterDetail, SplitView }

public enum RequirementEvidenceSource { RFP, TechnicalProposal }
public enum RequirementEvidenceStatus { Covered, PartiallyCovered, NotCovered, NeedsConfiguration, NotApplicable }
public record KpiDefinition(string Code, string Name, string Description, string Category, string CalculationType, decimal? TargetValue, string Unit, string Status, bool IsActive = true) : Entity(Guid.NewGuid());
public record KpiCalculationResult(string KpiCode, decimal Value, decimal? TargetValue, string Status, DateTimeOffset CalculatedAt, string FilterJson, string Explanation) : Entity(Guid.NewGuid());
public record RequirementEvidenceItem(string Code, RequirementEvidenceSource Source, string RequirementArea, string RequirementText, string ResponseCommitment, string SystemFeature, string Module, string Route, string ApiEndpoint, RequirementEvidenceStatus Status, string EvidenceNotes, DateTimeOffset LastVerifiedAt) : Entity(Guid.NewGuid());
public record ReportSubscription(string ReportCode, string UserId, string Frequency, string Format, string FiltersJson, bool IsActive, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record ReportRunHistory(string ReportCode, string UserId, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt, string Status, string? OutputReference, string? ErrorMessage) : Entity(Guid.NewGuid());

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
    public SupplierAccountState AccountState { get; init; } = Status == SupplierStatus.Approved ? SupplierAccountState.Approved : SupplierAccountState.PendingVerification;
    public bool EmailVerified { get; init; } = Status == SupplierStatus.Approved;
    public bool ProfileComplete { get; init; } = Status == SupplierStatus.Approved;
    public List<SupplierDocument> Documents { get; init; } = [];
    public List<SupplierCategory> Categories { get; init; } = [];
    public List<SupplierPerformanceRating> PerformanceRatings { get; init; } = [];
}
public record SupplierDocument(Guid SupplierId, string DocumentType, string FileName, string UploadedBy, DateTimeOffset UploadedAt) : Entity(Guid.NewGuid());
public record SupplierCategory(string Name) : Entity(Guid.NewGuid());
public record SupplierPerformanceRating(Guid SupplierId, int Score, string Notes, DateTimeOffset RatedAt) : Entity(Guid.NewGuid());

public record Tender(string TenderNumber, string Title, string Description, TenderType TenderType, string ProcurementMethod, TenderStatus Status, DateTimeOffset? PublicationDate, DateTimeOffset ClosingDate, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? PublishedAt = null, string? PublishedBy = null, string Category = "General") : Entity(Guid.NewGuid())
{
    public List<TenderLot> Lots { get; init; } = [];
    public List<TenderDocument> Documents { get; init; } = [];
    public List<TenderSupplierInvitation> SupplierInvitations { get; init; } = [];
    public List<TenderClarification> Clarifications { get; init; } = [];
    public List<TenderStatusHistory> StatusHistory { get; init; } = [];
}
public record TenderLot(Guid TenderId, string LotNumber, string Title, string Description) : Entity(Guid.NewGuid());
public record TenderDocument(Guid TenderId, string DocumentType, string FileName, string Description, bool IsRequired, DateTimeOffset CreatedAt, string CreatedBy, bool IsPublic = false, string? PublicUrl = null, bool IsDownloadable = true) : Entity(Guid.NewGuid());
public record PublicTenderPublication(Guid TenderId, string TenderNumber, string Reference, string Title, string Description, TenderType TenderType, string ProcurementMethod, string Category, DateTimeOffset PublishedAt, DateTimeOffset ClosingDate, TenderStatus Status, bool IsVisible, string Slug, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt) : Entity(Guid.NewGuid())
{
    public List<PublicTenderDocument> Documents { get; init; } = [];
    public List<PublicTenderClarification> Clarifications { get; init; } = [];
}
public record PublicTenderDocument(Guid PublicTenderPublicationId, string DocumentType, string FileName, string PublicUrl, bool IsDownloadable, DateTimeOffset PublishedAt) : Entity(Guid.NewGuid());
public record PublicTenderClarification(Guid PublicTenderPublicationId, Guid TenderClarificationId, string Question, string Response, DateTimeOffset PublishedAt) : Entity(Guid.NewGuid());
public record TenderSupplierInvitation(Guid TenderId, Guid? SupplierId, string SupplierName, string SupplierEmail, DateTimeOffset InvitedAt, string InvitedBy, DateTimeOffset? NotifiedAt = null) : Entity(Guid.NewGuid());
public record TenderClarification(Guid TenderId, string Question, string AskedBy, DateTimeOffset AskedAt, bool IsPublic = true) : Entity(Guid.NewGuid())
{
    public TenderClarificationStatus Status { get; init; } = TenderClarificationStatus.Submitted;
    public TenderClarificationVisibility Visibility { get; init; } = IsPublic ? TenderClarificationVisibility.Public : TenderClarificationVisibility.Private;
    public Guid? SupplierId { get; init; }
    public string SupplierName { get; init; } = AskedBy;
    public string QuestionReference { get; init; } = $"CLR-{AskedAt:yyyyMMddHHmmss}";
    public string? AssignedOfficer { get; init; }
    public List<TenderClarificationResponse> Responses { get; init; } = [];
}
public record TenderClarificationResponse(Guid TenderClarificationId, string Response, string RespondedBy, DateTimeOffset RespondedAt) : Entity(Guid.NewGuid())
{
    public bool IsPublished { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public string? PublishedBy { get; init; }
}

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



public record SealedBidEnvelope(Guid BidSubmissionId, Guid TenderId, Guid SupplierId, string EnvelopeNumber, SealedBidEnvelopeStatus Status, DateTimeOffset? SealedAt, string? SealedBy, DateTimeOffset? OpenedAt, string? OpenedBy, Guid? OpeningSessionId, string SubmissionHash, string DocumentManifestHash, string TimestampReference, string? DigitalSignatureReference, string? SecureVaultReference, string? OpeningKeyReference, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record SealedBidDocumentEvidence(Guid BidSubmissionDocumentId, Guid BidSubmissionId, string FileName, string DocumentType, string StorageReference, long FileSize, string ContentHash, string HashAlgorithm, DateTimeOffset UploadedAt, DateTimeOffset? SealedAt, bool IsPublicBeforeOpening, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record BidAccessLog(Guid BidSubmissionId, string UserId, string UserEmail, BidAccessType AccessType, bool AccessAllowed, string? DeniedReason, DateTimeOffset AccessedAt, string? IpAddress, string? UserAgent) : Entity(Guid.NewGuid());
public record BidOpeningEvidence(Guid BidOpeningSessionId, Guid BidSubmissionId, string OpenedBy, DateTimeOffset OpenedAt, string OpeningReason, string SubmissionHashAtOpening, string DocumentManifestHashAtOpening, bool IntegrityCheckPassed, string IntegrityCheckResultJson, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());

public record BidOpeningSession(string SessionNumber, Guid TenderId, string Title, DateTimeOffset ScheduledAt, BidOpeningSessionStatus Status, string CreatedBy, DateTimeOffset CreatedAt, string Chairperson, string? Notes = null, DateTimeOffset? StartedAt = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid())
{
    public List<BidOpeningCommitteeMember> CommitteeMembers { get; init; } = [];
    public List<BidOpeningSubmission> Submissions { get; init; } = [];
    public List<BidOpeningMinute> Minutes { get; init; } = [];
    public List<BidOpeningChecklistItem> ChecklistItems { get; init; } = [];
    public List<BidOpeningReport> Reports { get; init; } = [];
}
public record BidOpeningCommitteeMember(Guid BidOpeningSessionId, string Name, string Email, string Role, bool AttendanceConfirmed = false, DateTimeOffset? ConfirmedAt = null) : Entity(Guid.NewGuid());
public record BidOpeningSubmission(Guid BidOpeningSessionId, Guid BidSubmissionId, Guid SupplierId, string SupplierName, string SubmissionNumber, DateTimeOffset? SubmittedAt, BidOpeningSubmissionStatus Status, DateTimeOffset? OpenedAt = null, string? OpenedBy = null, string? Notes = null, string? SealedDocumentHash = null, string? OpeningKeyReference = null, string? DigitalSignatureReference = null, string? TimestampAuthorityReference = null, string? SecureVaultReference = null) : Entity(Guid.NewGuid());
public record BidOpeningMinute(Guid BidOpeningSessionId, string MinuteText, string RecordedBy, DateTimeOffset RecordedAt) : Entity(Guid.NewGuid());
public record BidOpeningChecklistItem(Guid BidOpeningSessionId, string Description, bool Completed = false, string? CompletedBy = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid());
public record BidOpeningReport(Guid BidOpeningSessionId, string ReportNumber, DateTimeOffset GeneratedAt, string GeneratedBy, string SummaryJson) : Entity(Guid.NewGuid());


public record EvaluationSession(string SessionNumber, Guid TenderId, Guid BidOpeningSessionId, string Title, EvaluationSessionStatus Status, EvaluationStage CurrentStage, string CreatedBy, DateTimeOffset CreatedAt, string Chairperson, string? Notes = null, DateTimeOffset? StartedAt = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid())
{
    public List<EvaluationCommitteeMember> CommitteeMembers { get; init; } = [];
    public List<EvaluationSubmission> Submissions { get; init; } = [];
    public List<EvaluationDeclaration> Declarations { get; init; } = [];
    public List<EvaluationScore> Scores { get; init; } = [];
    public List<EvaluationConsensusScore> ConsensusScores { get; init; } = [];
    public List<EvaluationRecommendation> Recommendations { get; init; } = [];
    public List<EvaluationReport> Reports { get; init; } = [];
    public List<EvaluationHistory> History { get; init; } = [];
}
public record EvaluationCommitteeMember(Guid EvaluationSessionId, string Name, string Email, string Role, bool IsChairperson = false, bool HasAcceptedDeclaration = false, DateTimeOffset? DeclarationAcceptedAt = null) : Entity(Guid.NewGuid());
public record EvaluationTemplate(string Code, string Name, string Description, TenderType TenderType, decimal TotalWeight, MetadataStatus Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? PublishedAt = null, string? PublishedBy = null) : Entity(Guid.NewGuid())
{ public List<EvaluationTemplateCriterion> Criteria { get; init; } = []; }
public record EvaluationCriterion(string Code, string Name, string Description, EvaluationStage Stage, decimal Weight, decimal MaximumScore, decimal MinimumPassingScore, int DisplayOrder) : Entity(Guid.NewGuid());
public record EvaluationTemplateCriterion(Guid EvaluationTemplateId, Guid EvaluationCriterionId, int DisplayOrder, bool IsMandatory = true) : Entity(Guid.NewGuid());
public record EvaluationSubmission(Guid EvaluationSessionId, Guid BidSubmissionId, Guid SupplierId, string SupplierName, string SubmissionNumber, EvaluationSubmissionStatus Status, bool AdministrativePassed = false, decimal TechnicalScore = 0, decimal FinancialScore = 0, decimal TotalScore = 0, int Rank = 0, string? Notes = null) : Entity(Guid.NewGuid());
public record EvaluationScore(Guid EvaluationSessionId, Guid EvaluationSubmissionId, Guid EvaluationCriterionId, string EvaluatorEmail, EvaluationStage Stage, decimal Score, string Comments, DateTimeOffset ScoredAt) : Entity(Guid.NewGuid());
public record EvaluationConsensusScore(Guid EvaluationSessionId, Guid EvaluationSubmissionId, Guid EvaluationCriterionId, EvaluationStage Stage, decimal ConsensusScore, string ConsensusComments, string RecordedBy, DateTimeOffset RecordedAt) : Entity(Guid.NewGuid());
public record EvaluationDeclaration(Guid EvaluationSessionId, string EvaluatorEmail, string DeclarationType, bool Accepted, DateTimeOffset AcceptedAt, string? Notes = null) : Entity(Guid.NewGuid());
public record EvaluationRecommendation(Guid EvaluationSessionId, Guid RecommendedBidSubmissionId, Guid SupplierId, string SupplierName, string RecommendationText, decimal RecommendedAmount, string RecommendedBy, DateTimeOffset RecommendedAt, string Status) : Entity(Guid.NewGuid());
public record EvaluationReport(Guid EvaluationSessionId, string ReportNumber, DateTimeOffset GeneratedAt, string GeneratedBy, string SummaryJson) : Entity(Guid.NewGuid());
public record EvaluationHistory(Guid EvaluationSessionId, string EventType, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());

public record Award(string AwardNumber, Guid TenderId, Guid EvaluationSessionId, Guid RecommendedBidSubmissionId, Guid SupplierId, string SupplierName, decimal AwardAmount, AwardStatus Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? SubmittedAt = null, DateTimeOffset? ApprovedAt = null, DateTimeOffset? PublishedAt = null, DateTimeOffset? CancelledAt = null, string? Notes = null) : Entity(Guid.NewGuid())
{
    public List<AwardItem> Items { get; init; } = [];
    public List<AwardDecision> Decisions { get; init; } = [];
    public List<AwardApproval> Approvals { get; init; } = [];
    public List<AwardNotification> Notifications { get; init; } = [];
    public List<AwardHistory> History { get; init; } = [];
    public List<AwardReport> Reports { get; init; } = [];
}
public record AwardItem(Guid AwardId, Guid? TenderLotId, string Description, decimal Quantity, decimal UnitPrice, decimal TotalAmount, string? Notes = null) : Entity(Guid.NewGuid());
public record AwardDecision(Guid AwardId, string DecisionType, AwardDecisionStatus DecisionStatus, string DecisionBy, DateTimeOffset DecisionAt, string Comments) : Entity(Guid.NewGuid());
public record AwardApproval(Guid AwardId, string Role, string ApproverEmail, bool Approved, DateTimeOffset? ApprovedAt = null, string? Comments = null) : Entity(Guid.NewGuid());
public record AwardNotification(Guid AwardId, Guid SupplierId, string SupplierName, string SupplierEmail, string NotificationType, string Subject, string Message, DateTimeOffset? SentAt, string Status) : Entity(Guid.NewGuid());
public record AwardHistory(Guid AwardId, AwardStatus FromStatus, AwardStatus ToStatus, string Actor, string Notes, DateTimeOffset ChangedAt) : Entity(Guid.NewGuid());
public record AwardReport(Guid AwardId, string ReportNumber, DateTimeOffset GeneratedAt, string GeneratedBy, string SummaryJson) : Entity(Guid.NewGuid());


public record PurchaseOrder(string PurchaseOrderNumber, Guid AwardId, Guid SupplierId, string SupplierName, DateTimeOffset? IssueDate, DateTimeOffset ExpectedDeliveryDate, string Currency, decimal TotalAmount, PurchaseOrderStatus Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? IssuedAt = null, DateTimeOffset? ClosedAt = null, DateTimeOffset? CancelledAt = null) : Entity(Guid.NewGuid())
{
    public List<PurchaseOrderLine> Lines { get; init; } = [];
    public List<PurchaseOrderAmendment> Amendments { get; init; } = [];
    public List<PurchaseOrderDelivery> Deliveries { get; init; } = [];
    public List<GoodsReceipt> GoodsReceipts { get; init; } = [];
    public List<PurchaseOrderHistory> History { get; init; } = [];
    public List<PurchaseOrderStatusHistory> StatusHistory { get; init; } = [];
}
public record PurchaseOrderLine(Guid PurchaseOrderId, int ItemNumber, string Description, decimal Quantity, decimal UnitPrice, decimal Total, decimal DeliveredQuantity, decimal OutstandingQuantity) : Entity(Guid.NewGuid());
public record PurchaseOrderAmendment(Guid PurchaseOrderId, string Reason, string OldValue, string NewValue, string ApprovedBy, DateTimeOffset ApprovedAt) : Entity(Guid.NewGuid());
public record PurchaseOrderDelivery(Guid PurchaseOrderId, DateTimeOffset DeliveryDate, string DeliveredBy, string ReceivedBy, string Notes) : Entity(Guid.NewGuid());
public record GoodsReceipt(Guid PurchaseOrderId, string ReceiptNumber, DateTimeOffset ReceivedAt, string ReceivedBy, string Status) : Entity(Guid.NewGuid());
public record PurchaseOrderHistory(Guid PurchaseOrderId, string EventType, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());
public record PurchaseOrderStatusHistory(Guid PurchaseOrderId, PurchaseOrderStatus FromStatus, PurchaseOrderStatus ToStatus, string Actor, DateTimeOffset ChangedAt, string Notes) : Entity(Guid.NewGuid());



public record PurchaseOrderReturn(string ReturnNumber, Guid PurchaseOrderId, Guid SupplierId, string SupplierName, string Reason, PurchaseOrderReturnStatus Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? SubmittedAt = null, string? ApprovedBy = null, DateTimeOffset? ApprovedAt = null, DateTimeOffset? CompletedAt = null, DateTimeOffset? CancelledAt = null, string? Notes = null) : Entity(Guid.NewGuid())
{
    public List<PurchaseOrderReturnLine> Lines { get; init; } = [];
}
public record PurchaseOrderReturnLine(Guid PurchaseOrderReturnId, Guid PurchaseOrderLineId, int ItemNumber, string Description, decimal ReturnedQuantity, decimal UnitPrice, decimal TotalAmount, string Reason, string? ConditionNotes = null) : Entity(Guid.NewGuid());
public record SupplierInvoice(string InvoiceNumber, string SupplierInvoiceNumber, Guid PurchaseOrderId, Guid? GoodsReceiptId, Guid SupplierId, string SupplierName, DateTimeOffset InvoiceDate, DateTimeOffset ReceivedDate, string Currency, decimal SubtotalAmount, decimal TaxAmount, decimal TotalAmount, SupplierInvoiceStatus Status, string SubmittedBy, DateTimeOffset? SubmittedAt = null, string? ReviewedBy = null, DateTimeOffset? ReviewedAt = null, DateTimeOffset? ApprovedForPaymentAt = null, DateTimeOffset? RejectedAt = null, string? RejectionReason = null, string? Notes = null) : Entity(Guid.NewGuid())
{
    public List<SupplierInvoiceLine> Lines { get; init; } = [];
    public List<InvoiceAttachment> Attachments { get; init; } = [];
}
public record SupplierInvoiceLine(Guid SupplierInvoiceId, Guid? PurchaseOrderLineId, Guid? GoodsReceiptId, int ItemNumber, string Description, decimal Quantity, decimal UnitPrice, decimal TaxAmount, decimal TotalAmount) : Entity(Guid.NewGuid());
public record InvoiceAttachment(Guid SupplierInvoiceId, string DocumentType, string FileName, string StorageReference, string UploadedBy, DateTimeOffset UploadedAt) : Entity(Guid.NewGuid());
public record InvoiceStatusHistory(Guid SupplierInvoiceId, SupplierInvoiceStatus FromStatus, SupplierInvoiceStatus ToStatus, string Actor, DateTimeOffset ChangedAt, string Notes) : Entity(Guid.NewGuid());
public record InvoiceReview(Guid SupplierInvoiceId, string ReviewerUserId, string ReviewerName, string Decision, string Comments, DateTimeOffset ReviewedAt) : Entity(Guid.NewGuid());
public record PaymentPreparation(Guid SupplierInvoiceId, string PaymentReference, PaymentPreparationStatus Status, string PreparedBy, DateTimeOffset PreparedAt, DateTimeOffset? SentToFinanceAt = null, string? ErrorMessage = null) : Entity(Guid.NewGuid());
public record ThreeWayMatch(Guid SupplierInvoiceId, Guid PurchaseOrderId, Guid? GoodsReceiptId, InvoiceMatchStatus MatchStatus, decimal PoAmount, decimal ReceiptAmount, decimal InvoiceAmount, decimal VarianceAmount, decimal VariancePercentage, bool QuantityMatched, bool PriceMatched, bool ReceiptMatched, bool TaxMatched, string ResultJson, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid())
{
    public List<InvoiceMatchingResult> Results { get; init; } = [];
}
public record InvoiceMatchingResult(Guid ThreeWayMatchId, Guid SupplierInvoiceLineId, Guid? PurchaseOrderLineId, InvoiceMatchStatus MatchStatus, decimal ExpectedQuantity, decimal InvoicedQuantity, decimal ReceivedQuantity, decimal ExpectedUnitPrice, decimal InvoicedUnitPrice, decimal VarianceAmount, string Message) : Entity(Guid.NewGuid());

public record Contract(string ContractNumber, Guid? AwardId, Guid? PurchaseOrderId, Guid SupplierId, string SupplierName, string Title, string Description, ContractType ContractType, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal OriginalValue, decimal CurrentValue, ContractStatus Status, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? ActivatedAt = null, DateTimeOffset? CompletedAt = null) : Entity(Guid.NewGuid())
{
    public List<ContractLine> Lines { get; init; } = [];
    public List<ContractDocument> Documents { get; init; } = [];
    public List<ContractMilestone> Milestones { get; init; } = [];
    public List<ContractDeliverable> Deliverables { get; init; } = [];
    public List<ContractVariation> Variations { get; init; } = [];
    public List<ContractRenewal> Renewals { get; init; } = [];
    public List<ContractPerformanceReview> PerformanceReviews { get; init; } = [];
    public List<ContractHistory> History { get; init; } = [];
    public List<ContractStatusHistory> StatusHistory { get; init; } = [];
}
public record ContractLine(Guid ContractId, int ItemNumber, string Description, decimal Quantity, decimal UnitPrice, decimal Total) : Entity(Guid.NewGuid());
public record ContractDocument(Guid ContractId, string DocumentType, string FileName, string StorageReference, string UploadedBy, DateTimeOffset UploadedAt) : Entity(Guid.NewGuid());
public record ContractMilestone(Guid ContractId, string Name, string Description, DateTimeOffset DueDate, DateTimeOffset? CompletedDate, string Status) : Entity(Guid.NewGuid());
public record ContractDeliverable(Guid ContractId, string Title, string Description, DateTimeOffset DueDate, string? AcceptedBy, DateTimeOffset? AcceptedAt, string Status) : Entity(Guid.NewGuid());
public record ContractVariation(Guid ContractId, string VariationNumber, string Description, string Reason, decimal AmountAdjustment, string ApprovedBy, DateTimeOffset ApprovedAt, DateTimeOffset? NewEndDate = null) : Entity(Guid.NewGuid());
public record ContractRenewal(Guid ContractId, string RenewalNumber, DateTimeOffset OldEndDate, DateTimeOffset NewEndDate, string Reason, string ApprovedBy, DateTimeOffset ApprovedAt) : Entity(Guid.NewGuid());
public record ContractPerformanceReview(Guid ContractId, DateTimeOffset ReviewDate, string Reviewer, int SupplierScore, int QualityScore, int DeliveryScore, string Comments) : Entity(Guid.NewGuid());

public record IntegrationEndpoint(string Code, string Name, IntegrationSystemType SystemType, string BaseUrl, string AuthType, bool IsEnabled, string SettingsJson, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record IntegrationMessage(Guid? EndpointId, string EntityType, Guid EntityId, IntegrationDirection Direction, string PayloadJson, IntegrationMessageStatus Status, DateTimeOffset CreatedAt, DateTimeOffset? ProcessedAt = null, string? ErrorMessage = null) : Entity(Guid.NewGuid());
public record IntegrationLog(Guid? EndpointId, Guid? MessageId, string EntityType, Guid? EntityId, string ExternalSystemCode, IntegrationDirection Direction, string Status, string? Error, DateTimeOffset Timestamp) : Entity(Guid.NewGuid());
public record ExternalSystemReference(string EntityType, Guid EntityId, string ExternalSystemCode, string ExternalReference, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record DocumentIntegrationReference(Guid DocumentId, string EntityType, Guid EntityId, string StorageReference, string? ExternalDocumentId, string SyncStatus, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record ContractHistory(Guid ContractId, string EventType, string Actor, string Details, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());
public record ContractStatusHistory(Guid ContractId, ContractStatus FromStatus, ContractStatus ToStatus, string Actor, DateTimeOffset ChangedAt, string Notes) : Entity(Guid.NewGuid());

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


public enum ProcurementCaseStatus { Active, Completed, Cancelled }
public enum ProcurementCaseRelationshipType { AnnualPlan, Budget, Requisition, Tender, PublicPublication, BidSubmission, BidOpening, Evaluation, Award, PurchaseOrder, Contract, Document, Notification, AuditEvent }
public record ProcurementCase(string CaseNumber, string Title, string Description, Guid FinancialYearId, string Department, ProcurementCaseStatus Status, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid())
{
    public List<ProcurementCaseLink> Links { get; init; } = [];
}
public record ProcurementCaseLink(Guid ProcurementCaseId, string EntityType, Guid EntityId, string EntityReference, ProcurementCaseRelationshipType RelationshipType, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());

public enum UserType { SystemAdministrator, ProcurementOfficer, FinanceUser, Approver, Evaluator, Auditor, Supplier }

public record ApplicationUser(string Email, string FullName, string? PhoneNumber, UserType UserType, bool IsActive, bool IsExternalUser, Guid? SupplierId, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt, string PasswordHash) : Entity(Guid.NewGuid())
{
    public int FailedLoginCount { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public DateTimeOffset? LastPasswordChangedAt { get; init; }
    public bool MustChangePassword { get; init; }
    public List<UserRole> UserRoles { get; init; } = [];
    public UserProfile? Profile { get; init; }
    public SupplierUserLink? SupplierLink { get; init; }
}
public record Role(string Name, string Description, bool IsActive = true) : Entity(Guid.NewGuid())
{
    public List<RolePermission> RolePermissions { get; init; } = [];
    public List<UserRole> UserRoles { get; init; } = [];
}
public record Permission(string Code, string Name, string Description, string Category, bool IsActive = true) : Entity(Guid.NewGuid())
{
    public List<RolePermission> RolePermissions { get; init; } = [];
}
public record RolePermission(Guid RoleId, Guid PermissionId) : Entity(Guid.NewGuid());
public record UserRole(Guid UserId, Guid RoleId) : Entity(Guid.NewGuid());
public record UserProfile(Guid UserId, string Department, string JobTitle, string PreferencesJson = "{}") : Entity(Guid.NewGuid());
public record SupplierUserLink(Guid UserId, Guid SupplierId, bool IsPrimaryContact = false, DateTimeOffset LinkedAt = default) : Entity(Guid.NewGuid());

public record UserMfaSetting(Guid UserId, bool IsEnabled, MfaMethod PreferredMethod, string? AuthenticatorSecretEncrypted, string? PhoneNumber, string? Email, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record UserMfaChallenge(Guid UserId, MfaMethod Method, string CodeHash, DateTimeOffset ExpiresAt, DateTimeOffset? ConsumedAt, DateTimeOffset CreatedAt, int AttemptCount = 0) : Entity(Guid.NewGuid());
public record TrustedDevice(Guid UserId, string DeviceHash, string Name, DateTimeOffset TrustedUntil, DateTimeOffset CreatedAt, DateTimeOffset? LastUsedAt = null) : Entity(Guid.NewGuid());
public record IdentityProviderConfiguration(string Code, string Name, IdentityProviderType ProviderType, string? Authority, string? ClientId, string? ClientSecretEncrypted, string? MetadataUrl, string CallbackPath, bool IsEnabled, string SettingsJson, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record ExternalIdentityLink(Guid UserId, string ProviderCode, string ExternalSubjectId, string? ExternalEmail, DateTimeOffset LinkedAt, DateTimeOffset? LastLoginAt = null) : Entity(Guid.NewGuid());
public record DelegationRule(Guid DelegatorUserId, Guid DelegateUserId, string? RoleCode, DateTimeOffset StartsAt, DateTimeOffset EndsAt, string Reason, bool IsActive, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid());
public record EscalationRule(string EntityType, string WorkflowCode, string NodeCode, string? AssignedRole, int EscalateAfterHours, string? EscalateToRole, Guid? EscalateToUserId, bool IsActive) : Entity(Guid.NewGuid());
public record WorkflowTaskEscalation(Guid WorkflowTaskId, Guid? EscalatedFromUserId, Guid? EscalatedToUserId, string? EscalatedToRole, string Reason, DateTimeOffset EscalatedAt) : Entity(Guid.NewGuid());

public enum NotificationChannel { InApp, Email, Sms }
public enum NotificationStatus { Pending, Sent, Failed, Cancelled, Read, Unread, Skipped, Retrying, Completed }
public enum NotificationPriority { Low, Normal, High, Critical }

public record NotificationTemplate(string Code, string Name, string Description, string EventCode, NotificationChannel Channel, string SubjectTemplate, string BodyTemplate, bool IsActive = true, DateTimeOffset CreatedAt = default, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid());
public record NotificationMessage(string EventCode, string EntityType, Guid? EntityId, NotificationChannel Channel, string Subject, string Body, NotificationPriority Priority, NotificationStatus Status, DateTimeOffset CreatedAt, DateTimeOffset? SentAt = null, string? FailureReason = null, string? RelatedUrl = null) : Entity(Guid.NewGuid()) { public List<NotificationRecipient> Recipients { get; init; } = []; }
public record NotificationRecipient(Guid NotificationMessageId, string UserId, string RecipientType, string Name, string? Email, string? PhoneNumber, string? RoleCode, NotificationStatus Status, DateTimeOffset? ReadAt = null) : Entity(Guid.NewGuid());
public record NotificationDeliveryLog(Guid NotificationMessageId, NotificationChannel Channel, string RequestPayload, string ResponsePayload, NotificationStatus Status, DateTimeOffset SentAt, string? Error = null) : Entity(Guid.NewGuid());
public record NotificationPreference(string UserId, string EventCode, bool InAppEnabled = true, bool EmailEnabled = true, bool SmsEnabled = true) : Entity(Guid.NewGuid());
public record NotificationEventMapping(string EventCode, string TemplateCode, NotificationChannel Channel, string EntityType, string? RecipientRoleCode, bool IsActive = true) : Entity(Guid.NewGuid());
public record NotificationRetryQueue(Guid DeliveryLogId, DateTimeOffset NextAttemptAt, int AttemptCount, int MaxAttempts, NotificationStatus Status, string? LastError = null) : Entity(Guid.NewGuid());
public record CommunicationThread(string ThreadNumber, string EntityType, Guid EntityId, string EntityReference, string Subject, string Visibility, string CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset? ClosedAt = null, string Status = "Open", Guid? SupplierId = null) : Entity(Guid.NewGuid()) { public List<CommunicationMessage> Messages { get; init; } = []; }
public record CommunicationMessage(Guid ThreadId, string SenderUserId, string SenderName, string SenderType, string Body, bool IsInternal, bool IsPublic, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record DeadlineReminderRule(string Code, string Name, string EntityType, string DateField, int ReminderOffsetHours, string TemplateCode, string RecipientRule, bool IsEnabled, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record DeadlineReminderRun(Guid RuleId, string EntityType, Guid EntityId, string EntityReference, DateTimeOffset ScheduledFor, DateTimeOffset? ExecutedAt, NotificationStatus Status, string? ErrorMessage = null) : Entity(Guid.NewGuid());
public record SystemSettingOverride(string Key, string Value, bool IsSecret, string Category, string UpdatedBy, DateTimeOffset UpdatedAt) : Entity(Guid.NewGuid());

public enum BackupRunStatus { Pending, Running, Completed, Failed, Cancelled }
public enum BackupType { Database, Documents, Configuration, Full }
public enum SupportCaseStatus { Open, InProgress, WaitingForUser, Resolved, Closed, Cancelled }
public enum SupportCaseSeverity { Low, Medium, High, Critical }
public record ApiPerformanceSample(string CorrelationId, string Path, string Method, int StatusCode, long DurationMs, string? UserId, DateTimeOffset OccurredAt) : Entity(Guid.NewGuid());
public record BackupPlan(string Code, string Name, BackupType BackupType, string ScheduleDescription, string StorageLocation, bool IsEnabled, int RetentionDays, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid());
public record BackupRun(Guid BackupPlanId, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt, BackupRunStatus Status, string BackupReference, long? SizeBytes, string? ErrorMessage, string TriggeredBy) : Entity(Guid.NewGuid());
public record RestoreRun(Guid BackupRunId, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt, BackupRunStatus Status, string RestoreTarget, string? ErrorMessage, string TriggeredBy) : Entity(Guid.NewGuid());
public record SupportCase(string CaseNumber, string Title, string Description, SupportCaseSeverity Severity, SupportCaseStatus Status, string Module, string ReportedBy, string? AssignedTo, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? ResolvedAt, string? ResolutionNotes, string? CorrelationId) : Entity(Guid.NewGuid());
