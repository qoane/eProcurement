using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
public sealed class OperationsController(IOperationsApplicationService ops, IConfigurationValidationService validation, EProcurementDbContext db, IConfiguration config, IWebHostEnvironment env) : ControllerBase
{
    [AllowAnonymous, HttpGet("health")]
    [AllowAnonymous, HttpGet("health/live")]
    public IActionResult Live() => Ok(Health("Healthy", new[] { new { name = "process", status = "Healthy", description = "Application process is running." } }, 0));

    [AllowAnonymous, HttpGet("health/ready")]
    [AllowAnonymous, HttpGet("health/database")]
    public async Task<IActionResult> Ready(CancellationToken ct) { var sw=Stopwatch.StartNew(); var ok=await db.Database.CanConnectAsync(ct); var checks=new[] { new { name="database", status= ok?"Healthy":"Failed", description=db.Database.ProviderName ?? "Database provider" }, new { name="configuration", status=validation.Validate().Errors.Count==0?"Healthy":"Failed", description="Required settings validation" } }; return StatusCode(ok?200:503, Health(ok?"Healthy":"Failed", checks, sw.ElapsedMilliseconds)); }
    [AllowAnonymous, HttpGet("health/storage")] public IActionResult Storage() { var root=config["Documents:RootPath"]; var status=string.IsNullOrWhiteSpace(root)?"NotConfigured":(Directory.Exists(root)?"Healthy":"Failed"); return Ok(Health(status, new[]{new{name="documentStorage",status,description=string.IsNullOrWhiteSpace(root)?"Document root not configured":"Document root configured"}}, 0)); }
    [AllowAnonymous, HttpGet("health/integrations")] public async Task<IActionResult> Integrations(CancellationToken ct) { var items=await db.IntegrationEndpoints.AsNoTracking().Select(x=>new{name=x.Name,status=x.IsEnabled?"Healthy":"NotConfigured",description=x.IsEnabled?"Enabled endpoint registered":"Integration disabled"}).ToListAsync(ct); return Ok(Health(items.Any(x=>x.status=="Failed")?"Failed":"Healthy", items, 0)); }
    object Health(string status, object checks, long duration) => new { status, timestamp=DateTimeOffset.UtcNow, environment=env.EnvironmentName, checks, durationMs=duration };

    [RequirePermission("Operations.View"), HttpGet("api/operations/performance")] public async Task<IActionResult> Performance(CancellationToken ct) => Ok(await ops.GetPerformanceAsync(ct));
    [RequirePermission("Operations.View"), HttpGet("api/operations/performance/slow-requests")] public async Task<IActionResult> Slow(CancellationToken ct) => Ok((await ops.GetPerformanceAsync(ct)).Where(x=>x.DurationMs>=config.GetValue("Operations:SlowRequestThresholdMs",1000)));
    [RequirePermission("Operations.View"), HttpGet("api/operations/performance/errors")] public async Task<IActionResult> Errors(CancellationToken ct) => Ok((await ops.GetPerformanceAsync(ct)).Where(x=>x.StatusCode>=400));
    [RequirePermission("Operations.View"), HttpGet("api/operations/performance/summary")] public async Task<IActionResult> Summary(CancellationToken ct) { db.AuditEvents.Add(new Lca.EProcurement.Domain.AuditEvent("Performance report viewed","Operations",Guid.Empty,"View",User.Identity?.Name??"unknown","summary",DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Ok(await ops.GetSummaryAsync(ct)); }
    [RequirePermission("Operations.View"), HttpGet("api/operations/configuration/validate")] public IActionResult ValidateConfiguration() => Ok(validation.Validate());
    [RequirePermission("Operations.View"), HttpGet("api/operations/backups/plans")] public async Task<IActionResult> Plans(CancellationToken ct) => Ok(await ops.GetBackupPlansAsync(ct));
    [RequirePermission("Operations.Backup.Manage"), HttpPost("api/operations/backups/plans")] public async Task<IActionResult> CreatePlan(BackupPlanCreateDto dto, CancellationToken ct) => Ok(await ops.CreateBackupPlanAsync(dto, User.Identity?.Name ?? "system", ct));
    [RequirePermission("Operations.View"), HttpGet("api/operations/backups/runs")] public async Task<IActionResult> Runs(CancellationToken ct) => Ok(await ops.GetBackupRunsAsync(ct));
    [RequirePermission("Operations.Backup.Manage"), HttpPost("api/operations/backups/runs/simulate")] public async Task<IActionResult> SimBackup([FromBody] Guid? planId, CancellationToken ct) => Ok(await ops.SimulateBackupRunAsync(planId, User.Identity?.Name ?? "system", ct));
    [RequirePermission("Operations.Backup.Manage"), HttpPost("api/operations/backups/restore/simulate")] public async Task<IActionResult> SimRestore([FromBody] RestoreRequest dto, CancellationToken ct) => Ok(await ops.SimulateRestoreRunAsync(dto.BackupRunId, dto.RestoreTarget, User.Identity?.Name ?? "system", ct));
}
public record RestoreRequest(Guid BackupRunId, string RestoreTarget);
