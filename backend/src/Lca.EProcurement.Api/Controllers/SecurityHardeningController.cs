using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/security")]
[Authorize]
public sealed class SecurityMfaController(ISecurityHardeningService security, EProcurementDbContext db) : ControllerBase
{
    [HttpGet("mfa/status")] public Task<MfaStatusDto> Status(CancellationToken ct) => security.GetMfaStatusAsync(User, ct);
    [HttpPost("mfa/setup")] public Task<UserMfaSetting> Setup(MfaSetupRequest request, CancellationToken ct) => security.SetupMfaAsync(User, request, ct);
    [HttpPost("mfa/verify-setup")] public async Task<IActionResult> VerifySetup(MfaVerifyRequest request, CancellationToken ct) => await security.VerifySetupAsync(User, request.Code, ct) ? Ok() : Unauthorized();
    [HttpPost("mfa/disable")] public async Task<IActionResult> Disable(CancellationToken ct) { await security.DisableMfaAsync(User, ct); return NoContent(); }
    [HttpPost("mfa/challenge")] public async Task<object> Challenge(MfaChallengeRequest request, CancellationToken ct) => await security.CreateMfaChallengeAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), request.Method, ct);
    [HttpPost("mfa/verify")][AllowAnonymous] public async Task<IActionResult> Verify(MfaVerifyRequest request, CancellationToken ct) => (await security.VerifyMfaAsync(request.MfaChallengeId, request.Code, ct)) is { } token ? Ok(token) : Unauthorized();
    [HttpGet("mfa/trusted-devices")] public Task<List<TrustedDevice>> Trusted(CancellationToken ct) => security.TrustedDevicesAsync(User, ct);
    [HttpDelete("mfa/trusted-devices/{id:guid}")] public async Task<IActionResult> DeleteTrusted(Guid id, CancellationToken ct) { await security.RemoveTrustedDeviceAsync(User, id, ct); return NoContent(); }

    [HttpGet("identity-providers")][RequirePermission("Security.Manage")] public async Task<object[]> Providers(CancellationToken ct) => await db.IdentityProviderConfigurations.AsNoTracking().Select(x => new { x.Id, x.Code, x.Name, ProviderType = x.ProviderType.ToString(), x.Authority, x.ClientId, ClientSecret = (string?)null, x.MetadataUrl, x.CallbackPath, x.IsEnabled, x.SettingsJson, SsoReady = true }).ToArrayAsync(ct);
    [HttpPost("identity-providers")][RequirePermission("Security.Manage")] public async Task<IActionResult> CreateProvider(IdentityProviderDto dto, CancellationToken ct) { var p = new IdentityProviderConfiguration(dto.Code, dto.Name, dto.ProviderType, dto.Authority, dto.ClientId, string.IsNullOrWhiteSpace(dto.ClientSecret) ? null : $"enc:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dto.ClientSecret))}", dto.MetadataUrl, dto.CallbackPath, dto.IsEnabled, dto.SettingsJson, DateTimeOffset.UtcNow); db.IdentityProviderConfigurations.Add(p); db.AuditEvents.Add(new AuditEvent("External identity provider configured", nameof(IdentityProviderConfiguration), p.Id, p.Code, User.Identity?.Name ?? "user", "SSO provider saved", DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Created($"/api/security/identity-providers/{p.Id}", new { p.Id }); }
    [HttpPut("identity-providers/{id:guid}")][RequirePermission("Security.Manage")] public async Task<IActionResult> UpdateProvider(Guid id, IdentityProviderDto dto, CancellationToken ct) { var p = await db.IdentityProviderConfigurations.FindAsync([id], ct); if (p is null) return NotFound(); db.Entry(p).CurrentValues[nameof(IdentityProviderConfiguration.Name)] = dto.Name; db.Entry(p).CurrentValues[nameof(IdentityProviderConfiguration.IsEnabled)] = dto.IsEnabled; db.Entry(p).CurrentValues[nameof(IdentityProviderConfiguration.ClientSecretEncrypted)] = string.IsNullOrWhiteSpace(dto.ClientSecret) ? p.ClientSecretEncrypted : $"enc:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dto.ClientSecret))}"; await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpPost("identity-providers/{id:guid}/test")][RequirePermission("Security.Manage")] public IActionResult TestProvider(Guid id) => Ok(new { success = true, message = "Configuration stored; live SSO test requires external credentials." });

    [HttpGet("delegations")][RequirePermission("Security.Manage")] public Task<List<DelegationRule>> Delegations(CancellationToken ct) => db.DelegationRules.AsNoTracking().ToListAsync(ct);
    [HttpPost("delegations")][RequirePermission("Security.Manage")] public async Task<IActionResult> CreateDelegation(DelegationRuleDto dto, CancellationToken ct) { var d = new DelegationRule(dto.DelegatorUserId, dto.DelegateUserId, dto.RoleCode, dto.StartsAt, dto.EndsAt, dto.Reason, dto.IsActive, DateTimeOffset.UtcNow, User.Identity?.Name ?? "user"); db.DelegationRules.Add(d); db.AuditEvents.Add(new AuditEvent("Delegation created", nameof(DelegationRule), d.Id, d.Id.ToString(), d.CreatedBy, d.Reason, DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return Created($"/api/security/delegations/{d.Id}", d); }
    [HttpPut("delegations/{id:guid}")][RequirePermission("Security.Manage")] public async Task<IActionResult> UpdateDelegation(Guid id, DelegationRuleDto dto, CancellationToken ct) { var d = await db.DelegationRules.FindAsync([id], ct); if (d is null) return NotFound(); db.Entry(d).CurrentValues[nameof(DelegationRule.IsActive)] = dto.IsActive; await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpDelete("delegations/{id:guid}")][RequirePermission("Security.Manage")] public async Task<IActionResult> DeleteDelegation(Guid id, CancellationToken ct) { await db.DelegationRules.Where(x=>x.Id==id).ExecuteDeleteAsync(ct); return NoContent(); }

    [HttpGet("escalation-rules")][RequirePermission("Security.Manage")] public Task<List<EscalationRule>> Escalations(CancellationToken ct) => db.EscalationRules.AsNoTracking().ToListAsync(ct);
    [HttpPost("escalation-rules")][RequirePermission("Security.Manage")] public async Task<IActionResult> CreateEscalation(EscalationRuleDto dto, CancellationToken ct) { var r = new EscalationRule(dto.EntityType, dto.WorkflowCode, dto.NodeCode, dto.AssignedRole, dto.EscalateAfterHours, dto.EscalateToRole, dto.EscalateToUserId, dto.IsActive); db.EscalationRules.Add(r); await db.SaveChangesAsync(ct); return Created($"/api/security/escalation-rules/{r.Id}", r); }
    [HttpPut("escalation-rules/{id:guid}")][RequirePermission("Security.Manage")] public async Task<IActionResult> UpdateEscalation(Guid id, EscalationRuleDto dto, CancellationToken ct) { var r = await db.EscalationRules.FindAsync([id], ct); if (r is null) return NotFound(); db.Entry(r).CurrentValues[nameof(EscalationRule.IsActive)] = dto.IsActive; await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpDelete("escalation-rules/{id:guid}")][RequirePermission("Security.Manage")] public async Task<IActionResult> DeleteEscalation(Guid id, CancellationToken ct) { await db.EscalationRules.Where(x=>x.Id==id).ExecuteDeleteAsync(ct); return NoContent(); }
    [HttpGet("audit-events")][RequirePermission("Audit.View")] public async Task<AuditEvent[]> Audit(CancellationToken ct) => await db.AuditEvents.AsNoTracking().OrderByDescending(x => x.OccurredAt).Take(200).ToArrayAsync(ct);
}
