using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Lca.EProcurement.Application;

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
public record ConfigurationValidationResult(string Status, IReadOnlyList<string> Warnings, IReadOnlyList<string> Errors, IReadOnlyList<string> Recommendations);
public record SupportCaseCreateDto(string Title, string Description, SupportCaseSeverity Severity, string Module, string? CorrelationId);
public record BackupPlanCreateDto(string Code, string Name, BackupType BackupType, string ScheduleDescription, string StorageLocation, bool IsEnabled, int RetentionDays);
public record PerformanceSummary(int TotalSamples, int ErrorCount, int SlowRequestCount, double AverageDurationMs, long P95DurationMs);

public interface IConfigurationValidationService { ConfigurationValidationResult Validate(); }
public sealed class ConfigurationValidationService(IConfiguration configuration) : IConfigurationValidationService
{
    public ConfigurationValidationResult Validate()
    {
        var warnings = new List<string>(); var errors = new List<string>(); var recommendations = new List<string>();
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Default"))) errors.Add("ConnectionStrings.Default is not configured.");
        if ((configuration["Jwt:SigningKey"] ?? "").Length < 32) errors.Add("Jwt:SigningKey should be at least 32 characters.");
        if (string.IsNullOrWhiteSpace(configuration["Operations:PublicBaseUrl"])) warnings.Add("Operations.PublicBaseUrl is not configured.");
        if (string.IsNullOrWhiteSpace(configuration["Operations:FrontendBaseUrl"])) warnings.Add("Operations.FrontendBaseUrl is not configured.");
        if (string.IsNullOrWhiteSpace(configuration["Cors:AllowedOrigins"])) warnings.Add("Allowed origins are using development defaults unless configured.");
        if (configuration.GetValue<bool>("Notifications:Email:Enabled") && string.IsNullOrWhiteSpace(configuration["Notifications:Email:SmtpHost"])) errors.Add("SMTP is enabled but host is missing.");
        if (configuration.GetValue<bool>("Notifications:Sms:Enabled") && string.IsNullOrWhiteSpace(configuration["Notifications:Sms:EndpointUrl"])) errors.Add("SMS is enabled but endpoint is missing.");
        var documentRoot = configuration["Documents:RootPath"];
        if (!string.IsNullOrWhiteSpace(documentRoot) && !Directory.Exists(documentRoot)) warnings.Add("Configured document root path does not exist on this host.");
        recommendations.Add("Configure production backups at SQL Server and infrastructure level; Phase 1 only records readiness evidence.");
        return new(errors.Count == 0 ? (warnings.Count == 0 ? "Healthy" : "Warning") : "Failed", warnings, errors, recommendations);
    }
}

public interface IOperationsApplicationService
{
    Task RecordPerformanceAsync(ApiPerformanceSample sample, CancellationToken ct = default);
    Task<IReadOnlyList<ApiPerformanceSample>> GetPerformanceAsync(CancellationToken ct = default);
    Task<PerformanceSummary> GetSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BackupPlan>> GetBackupPlansAsync(CancellationToken ct = default);
    Task<BackupPlan> CreateBackupPlanAsync(BackupPlanCreateDto dto, string actor, CancellationToken ct = default);
    Task<IReadOnlyList<BackupRun>> GetBackupRunsAsync(CancellationToken ct = default);
    Task<BackupRun> SimulateBackupRunAsync(Guid? planId, string actor, CancellationToken ct = default);
    Task<RestoreRun> SimulateRestoreRunAsync(Guid backupRunId, string target, string actor, CancellationToken ct = default);
    Task<PagedResult<SupportCase>> GetSupportCasesAsync(int page, int pageSize, string? status, CancellationToken ct = default);
    Task<SupportCase?> GetSupportCaseAsync(Guid id, CancellationToken ct = default);
    Task<SupportCase> CreateSupportCaseAsync(SupportCaseCreateDto dto, string actor, CancellationToken ct = default);
    Task<SupportCase?> AssignSupportCaseAsync(Guid id, string assignedTo, CancellationToken ct = default);
    Task<SupportCase?> ResolveSupportCaseAsync(Guid id, string notes, CancellationToken ct = default);
    Task<SupportCase?> CloseSupportCaseAsync(Guid id, CancellationToken ct = default);
}

public sealed class OperationsApplicationService(EProcurementDbContext db, IConfiguration config) : IOperationsApplicationService
{
    int MaxPageSize => Math.Clamp(config.GetValue("Operations:MaxPageSize", 100), 1, 500);
    public async Task RecordPerformanceAsync(ApiPerformanceSample sample, CancellationToken ct = default) { db.ApiPerformanceSamples.Add(sample); await db.SaveChangesAsync(ct); }
    public async Task<IReadOnlyList<ApiPerformanceSample>> GetPerformanceAsync(CancellationToken ct = default) => await db.ApiPerformanceSamples.OrderByDescending(x => x.OccurredAt).Take(200).AsNoTracking().ToListAsync(ct);
    public async Task<PerformanceSummary> GetSummaryAsync(CancellationToken ct = default) { var s = await db.ApiPerformanceSamples.OrderByDescending(x=>x.OccurredAt).Take(1000).AsNoTracking().ToListAsync(ct); var ordered=s.Select(x=>x.DurationMs).OrderBy(x => x).ToList(); return new(s.Count, s.Count(x=>x.StatusCode>=400), s.Count(x=>x.DurationMs>=config.GetValue("Operations:SlowRequestThresholdMs",1000)), s.Count==0?0:s.Average(x=>x.DurationMs), ordered.Count==0?0:ordered[(int)Math.Floor((ordered.Count-1)*0.95)]); }
    public async Task<IReadOnlyList<BackupPlan>> GetBackupPlansAsync(CancellationToken ct = default) => await db.BackupPlans.OrderBy(x=>x.Code).AsNoTracking().ToListAsync(ct);
    public async Task<BackupPlan> CreateBackupPlanAsync(BackupPlanCreateDto dto, string actor, CancellationToken ct = default) { var p = new BackupPlan(dto.Code, dto.Name, dto.BackupType, dto.ScheduleDescription, dto.StorageLocation, dto.IsEnabled, dto.RetentionDays, DateTimeOffset.UtcNow, actor); db.BackupPlans.Add(p); db.AuditEvents.Add(new AuditEvent("Backup plan created","BackupPlan",p.Id,"Create",actor,p.Code,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return p; }
    public async Task<IReadOnlyList<BackupRun>> GetBackupRunsAsync(CancellationToken ct = default) => await db.BackupRuns.OrderByDescending(x=>x.StartedAt).AsNoTracking().ToListAsync(ct);
    public async Task<BackupRun> SimulateBackupRunAsync(Guid? planId, string actor, CancellationToken ct = default) { var id=planId ?? (await db.BackupPlans.Select(x=>x.Id).FirstOrDefaultAsync(ct)); var r = new BackupRun(id, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, BackupRunStatus.Completed, $"SIM-BACKUP-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}", 0, null, actor); db.BackupRuns.Add(r); db.AuditEvents.Add(new AuditEvent("Backup run simulated","BackupRun",r.Id,"Simulate",actor,r.BackupReference,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return r; }
    public async Task<RestoreRun> SimulateRestoreRunAsync(Guid backupRunId, string target, string actor, CancellationToken ct = default) { var r = new RestoreRun(backupRunId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, BackupRunStatus.Completed, target, null, actor); db.RestoreRuns.Add(r); db.AuditEvents.Add(new AuditEvent("Restore run simulated","RestoreRun",r.Id,"Simulate",actor,target,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return r; }
    public async Task<PagedResult<SupportCase>> GetSupportCasesAsync(int page, int pageSize, string? status, CancellationToken ct = default) { page=Math.Max(1,page); pageSize=Math.Clamp(pageSize<=0?25:pageSize,1,MaxPageSize); var q=db.SupportCases.AsNoTracking().AsQueryable(); if(Enum.TryParse<SupportCaseStatus>(status,true,out var st)) q=q.Where(x=>x.Status==st); var count=await q.CountAsync(ct); var items=await q.OrderByDescending(x=>x.CreatedAt).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct); return new(items,page,pageSize,count,(int)Math.Ceiling(count/(double)pageSize)); }
    public Task<SupportCase?> GetSupportCaseAsync(Guid id, CancellationToken ct = default) => db.SupportCases.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<SupportCase> CreateSupportCaseAsync(SupportCaseCreateDto dto, string actor, CancellationToken ct = default) { var c=new SupportCase($"SUP-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",dto.Title,dto.Description,dto.Severity,SupportCaseStatus.Open,dto.Module,actor,null,DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,null,null,dto.CorrelationId); db.SupportCases.Add(c); db.AuditEvents.Add(new AuditEvent("Support case created","SupportCase",c.Id,"Create",actor,c.CaseNumber,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return c; }
    public async Task<SupportCase?> AssignSupportCaseAsync(Guid id, string assignedTo, CancellationToken ct = default) { var c=await db.SupportCases.FindAsync([id],ct); if(c is null) return null; db.Entry(c).CurrentValues.SetValues(c with { AssignedTo=assignedTo, Status=SupportCaseStatus.InProgress, UpdatedAt=DateTimeOffset.UtcNow }); await db.SaveChangesAsync(ct); return await GetSupportCaseAsync(id,ct); }
    public async Task<SupportCase?> ResolveSupportCaseAsync(Guid id, string notes, CancellationToken ct = default) { var c=await db.SupportCases.FindAsync([id],ct); if(c is null) return null; db.Entry(c).CurrentValues.SetValues(c with { Status=SupportCaseStatus.Resolved, ResolutionNotes=notes, ResolvedAt=DateTimeOffset.UtcNow, UpdatedAt=DateTimeOffset.UtcNow }); db.AuditEvents.Add(new AuditEvent("Support case resolved","SupportCase",id,"Resolve",c.ReportedBy,c.CaseNumber,DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return await GetSupportCaseAsync(id,ct); }
    public async Task<SupportCase?> CloseSupportCaseAsync(Guid id, CancellationToken ct = default) { var c=await db.SupportCases.FindAsync([id],ct); if(c is null) return null; db.Entry(c).CurrentValues.SetValues(c with { Status=SupportCaseStatus.Closed, UpdatedAt=DateTimeOffset.UtcNow }); await db.SaveChangesAsync(ct); return await GetSupportCaseAsync(id,ct); }
}
