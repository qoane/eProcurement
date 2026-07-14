using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Security;

public sealed record MfaStatusDto(bool IsEnabled, string PreferredMethod, string? Email, string? PhoneNumber);
public sealed record MfaSetupRequest(MfaMethod Method, string? PhoneNumber, string? Email);
public sealed record MfaChallengeRequest(MfaMethod? Method);
public sealed record MfaVerifyRequest(Guid MfaChallengeId, string Code);
public sealed record MfaLoginResponse(bool RequiresMfa, Guid MfaChallengeId, string[] AvailableMethods);
public sealed record IdentityProviderDto(Guid? Id, string Code, string Name, IdentityProviderType ProviderType, string? Authority, string? ClientId, string? ClientSecret, string? MetadataUrl, string CallbackPath, bool IsEnabled, string SettingsJson);
public sealed record DelegationRuleDto(Guid? Id, Guid DelegatorUserId, Guid DelegateUserId, string? RoleCode, DateTimeOffset StartsAt, DateTimeOffset EndsAt, string Reason, bool IsActive);
public sealed record EscalationRuleDto(Guid? Id, string EntityType, string WorkflowCode, string NodeCode, string? AssignedRole, int EscalateAfterHours, string? EscalateToRole, Guid? EscalateToUserId, bool IsActive);

public interface ISecurityHardeningService
{
    Task<MfaStatusDto> GetMfaStatusAsync(ClaimsPrincipal principal, CancellationToken ct);
    Task<UserMfaChallenge> CreateMfaChallengeAsync(Guid userId, MfaMethod? method, CancellationToken ct);
    Task<AuthResponse?> VerifyMfaAsync(Guid challengeId, string code, CancellationToken ct);
    Task<UserMfaSetting> SetupMfaAsync(ClaimsPrincipal principal, MfaSetupRequest request, CancellationToken ct);
    Task<bool> VerifySetupAsync(ClaimsPrincipal principal, string code, CancellationToken ct);
    Task DisableMfaAsync(ClaimsPrincipal principal, CancellationToken ct);
    Task<List<TrustedDevice>> TrustedDevicesAsync(ClaimsPrincipal principal, CancellationToken ct);
    Task RemoveTrustedDeviceAsync(ClaimsPrincipal principal, Guid id, CancellationToken ct);
}

public sealed class SecurityHardeningService(EProcurementDbContext db, IPasswordService passwords, IIdentityService identity) : ISecurityHardeningService
{
    public static string HashCode(string code) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
    static Guid CurrentUserId(ClaimsPrincipal p) => Guid.TryParse(p.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException("Missing authenticated user identity.");
    static string NewOtp() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();

    public async Task<MfaStatusDto> GetMfaStatusAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = CurrentUserId(principal);
        var setting = await db.UserMfaSettings.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId, ct);
        return setting is null ? new(false, MfaMethod.EmailOtp.ToString(), null, null) : new(setting.IsEnabled, setting.PreferredMethod.ToString(), setting.Email, setting.PhoneNumber);
    }
    public async Task<UserMfaSetting> SetupMfaAsync(ClaimsPrincipal principal, MfaSetupRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId(principal);
        var existing = await db.UserMfaSettings.SingleOrDefaultAsync(x => x.UserId == userId, ct);
        if (existing is not null) db.UserMfaSettings.Remove(existing);
        var setting = new UserMfaSetting(userId, false, request.Method, null, request.PhoneNumber, request.Email, DateTimeOffset.UtcNow);
        db.UserMfaSettings.Add(setting); await db.SaveChangesAsync(ct); return setting;
    }
    public async Task<bool> VerifySetupAsync(ClaimsPrincipal principal, string code, CancellationToken ct)
    {
        var userId = CurrentUserId(principal);
        var challenge = await db.UserMfaChallenges.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x => x.UserId == userId && x.ConsumedAt == null, ct);
        if (challenge is null || challenge.ExpiresAt < DateTimeOffset.UtcNow || challenge.CodeHash != HashCode(code)) return false;
        db.Entry(challenge).CurrentValues[nameof(UserMfaChallenge.ConsumedAt)] = DateTimeOffset.UtcNow;
        var setting = await db.UserMfaSettings.SingleAsync(x => x.UserId == userId, ct);
        db.Entry(setting).CurrentValues[nameof(UserMfaSetting.IsEnabled)] = true;
        db.AuditEvents.Add(new AuditEvent("MFA enabled", nameof(ApplicationUser), userId, userId.ToString(), principal.Identity?.Name ?? "user", "User enabled MFA", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return true;
    }
    public async Task<UserMfaChallenge> CreateMfaChallengeAsync(Guid userId, MfaMethod? method, CancellationToken ct)
    {
        var setting = await db.UserMfaSettings.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId, ct);
        var selected = method ?? setting?.PreferredMethod ?? MfaMethod.EmailOtp;
        var code = NewOtp();
        var challenge = new UserMfaChallenge(userId, selected, HashCode(code), DateTimeOffset.UtcNow.AddMinutes(10), null, DateTimeOffset.UtcNow);
        db.UserMfaChallenges.Add(challenge);
        db.AuditEvents.Add(new AuditEvent("MFA challenge created", nameof(ApplicationUser), userId, userId.ToString(), "system", $"MFA challenge created by {selected}", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return challenge;
    }
    public async Task<AuthResponse?> VerifyMfaAsync(Guid challengeId, string code, CancellationToken ct)
    {
        var challenge = await db.UserMfaChallenges.SingleOrDefaultAsync(x => x.Id == challengeId, ct);
        if (challenge is null || challenge.ConsumedAt is not null || challenge.ExpiresAt < DateTimeOffset.UtcNow) return null;
        if (challenge.CodeHash != HashCode(code)) { db.Entry(challenge).CurrentValues[nameof(UserMfaChallenge.AttemptCount)] = challenge.AttemptCount + 1; db.AuditEvents.Add(new AuditEvent("MFA failed", nameof(ApplicationUser), challenge.UserId, challenge.UserId.ToString(), "system", "Invalid MFA code", DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); return null; }
        db.Entry(challenge).CurrentValues[nameof(UserMfaChallenge.ConsumedAt)] = DateTimeOffset.UtcNow;
        db.AuditEvents.Add(new AuditEvent("MFA verified", nameof(ApplicationUser), challenge.UserId, challenge.UserId.ToString(), "system", "MFA verified", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(ct); return await identity.BuildTokenForUserAsync(challenge.UserId, ct);
    }
    public async Task DisableMfaAsync(ClaimsPrincipal principal, CancellationToken ct) { var userId = CurrentUserId(principal); await db.UserMfaSettings.Where(x => x.UserId == userId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, false), ct); db.AuditEvents.Add(new AuditEvent("MFA disabled", nameof(ApplicationUser), userId, userId.ToString(), principal.Identity?.Name ?? "user", "User disabled MFA", DateTimeOffset.UtcNow)); await db.SaveChangesAsync(ct); }
    public async Task<List<TrustedDevice>> TrustedDevicesAsync(ClaimsPrincipal principal, CancellationToken ct) => await db.TrustedDevices.AsNoTracking().Where(x => x.UserId == CurrentUserId(principal)).ToListAsync(ct);
    public async Task RemoveTrustedDeviceAsync(ClaimsPrincipal principal, Guid id, CancellationToken ct) { var userId = CurrentUserId(principal); await db.TrustedDevices.Where(x => x.UserId == userId && x.Id == id).ExecuteDeleteAsync(ct); }
}
