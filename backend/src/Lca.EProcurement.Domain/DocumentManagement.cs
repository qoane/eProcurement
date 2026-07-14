namespace Lca.EProcurement.Domain;

public enum DocumentClassification { Public, Internal, Confidential, SealedBid, Restricted }
public enum DocumentStatus { Draft, Active, Superseded, Archived, Deleted, Quarantined }
public enum DocumentPermission { ViewMetadata, Download, UploadNewVersion, Delete, Publish, Archive }
public enum DocumentAction { Uploaded, Downloaded, Viewed, Versioned, Published, Unpublished, Archived, Deleted, AccessDenied, VirusScanPassed, VirusScanFailed }
public enum VirusScanStatus { Pending, Passed, Failed, Skipped }

public record DocumentRecord(string DocumentNumber, string EntityType, Guid EntityId, string DocumentType, string Title, string? Description, DocumentClassification Classification, DocumentStatus Status, Guid? CurrentVersionId, bool IsPublic, bool IsConfidential, string CreatedBy, DateTimeOffset CreatedAt, string? UpdatedBy = null, DateTimeOffset? UpdatedAt = null) : Entity(Guid.NewGuid())
{
    public List<DocumentVersion> Versions { get; init; } = [];
    public List<DocumentAccessRule> AccessRules { get; init; } = [];
    public List<DocumentAccessLog> AccessLogs { get; init; } = [];
    public List<DocumentArchiveRecord> Archives { get; init; } = [];
}
public record DocumentVersion(Guid DocumentRecordId, int VersionNumber, string FileName, string OriginalFileName, string ContentType, string FileExtension, long FileSize, string StorageProvider, string StorageReference, string ContentHash, string HashAlgorithm, string UploadedBy, DateTimeOffset UploadedAt, bool IsCurrent, string? ChangeReason = null) : Entity(Guid.NewGuid())
{
    public List<DocumentVirusScanResult> VirusScanResults { get; init; } = [];
}
public record DocumentAccessRule(Guid DocumentRecordId, string RoleCode, string? UserId, DocumentPermission Permission, DateTimeOffset? StartsAt, DateTimeOffset? EndsAt, string CreatedBy, DateTimeOffset CreatedAt) : Entity(Guid.NewGuid());
public record DocumentAccessLog(Guid DocumentRecordId, Guid? DocumentVersionId, string UserId, string UserEmail, DocumentAction Action, bool AccessAllowed, string? DeniedReason, string? IpAddress, string? UserAgent, DateTimeOffset AccessedAt) : Entity(Guid.NewGuid());
public record DocumentRetentionPolicy(string EntityType, string DocumentType, int RetentionPeriodDays, int ArchiveAfterDays, int? DeleteAfterDays, bool IsActive, DateTimeOffset CreatedAt, string CreatedBy) : Entity(Guid.NewGuid());
public record DocumentArchiveRecord(Guid DocumentRecordId, DateTimeOffset ArchivedAt, string ArchivedBy, string ArchiveReason, string? ArchiveStorageReference, string Status) : Entity(Guid.NewGuid());
public record DocumentVirusScanResult(Guid DocumentVersionId, string ScanProvider, VirusScanStatus ScanStatus, DateTimeOffset ScannedAt, string Result, string? ThreatName, string? Details) : Entity(Guid.NewGuid());
public record DocumentShareLink(Guid DocumentRecordId, string TokenHash, DateTimeOffset ExpiresAt, string CreatedBy, DateTimeOffset CreatedAt, bool IsRevoked, DateTimeOffset? RevokedAt, string Purpose) : Entity(Guid.NewGuid());
public record DocumentImportBatch(string BatchNumber, string SourceName, string UploadedBy, DateTimeOffset UploadedAt, string Status, int TotalItems, int SuccessfulItems, int FailedItems) : Entity(Guid.NewGuid()) { public List<DocumentImportItem> Items { get; init; } = []; }
public record DocumentImportItem(Guid BatchId, string OriginalReference, string EntityType, string EntityReference, string DocumentType, string FileName, string Status, string? ErrorMessage) : Entity(Guid.NewGuid());
