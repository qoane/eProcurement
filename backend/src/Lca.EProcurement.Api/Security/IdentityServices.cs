using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lca.EProcurement.Api.Security;

public sealed class JwtSettings { public string Issuer { get; set; } = "ProcuraFlow"; public string Audience { get; set; } = "ProcuraFlow"; public string SigningKey { get; set; } = "ProcuraFlow development signing key - replace in production"; public int ExpiryMinutes { get; set; } = 120; }
public sealed record UserProfileDto(Guid Id, string Email, string FullName, string? PhoneNumber, string UserType, bool IsActive, bool IsExternalUser, Guid? SupplierId, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt);
public sealed record AuthResponse(string AccessToken, UserProfileDto UserProfile, IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions);
public sealed record LoginRequest(string Email, string Password);
public sealed record CreateUserRequest(string Email, string FullName, string? PhoneNumber, UserType UserType, bool IsExternalUser, Guid? SupplierId, string[] Roles, string? Password);
public sealed record AssignRolesRequest(string[] Roles);
public sealed record AssignPermissionsRequest(string[] Permissions);

public interface IPasswordService { string Hash(string password); bool Verify(string hash, string password); }
public sealed class PasswordService : IPasswordService
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string hash, string password) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public interface IIdentityService
{
    Task<AuthResponse?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResponse?> CurrentAsync(ClaimsPrincipal principal, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(ClaimsPrincipal principal, string permission, CancellationToken ct = default);
}
public sealed class IdentityService(EProcurementDbContext db, IPasswordService passwords, IOptions<JwtSettings> jwtOptions) : IIdentityService
{
    private const string FullNameClaim = "fullName";
    private const string PhoneNumberClaim = "phoneNumber";
    private const string IsExternalUserClaim = "isExternalUser";
    private const string SupplierIdClaim = "supplierId";
    private const string CreatedAtClaim = "createdAt";
    private const string LastLoginAtClaim = "lastLoginAt";
    public async Task<AuthResponse?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await db.ApplicationUsers.SingleOrDefaultAsync(x => x.Email == email, ct);
        if (user is null || !user.IsActive || !passwords.Verify(user.PasswordHash, password)) return null;
        await db.ApplicationUsers.Where(x => x.Id == user.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.LastLoginAt, DateTimeOffset.UtcNow), ct);
        return await BuildResponseAsync(user.Id, ct);
    }
    public async Task<AuthResponse?> CurrentAsync(ClaimsPrincipal principal, CancellationToken ct = default)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId)) return null;

        try
        {
            return await BuildResponseAsync(userId, ct);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return BuildResponseFromClaims(principal, userId);
        }
    }
    public async Task<bool> HasPermissionAsync(ClaimsPrincipal principal, string permission, CancellationToken ct = default)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId)) return false;
        return await db.UserRoles.Where(ur => ur.UserId == userId).Join(db.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (_, rp) => rp.PermissionId).Join(db.Permissions, pid => pid, p => p.Id, (_, p) => p).AnyAsync(p => p.Code == permission && p.IsActive, ct);
    }
    private async Task<AuthResponse?> BuildResponseAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.ApplicationUsers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId && x.IsActive, ct);
        if (user is null) return null;
        var roles = await db.UserRoles.Where(ur => ur.UserId == userId).Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name).OrderBy(x => x).ToListAsync(ct);
        var perms = await db.UserRoles.Where(ur => ur.UserId == userId).Join(db.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (_, rp) => rp.PermissionId).Join(db.Permissions, pid => pid, p => p.Id, (_, p) => p.Code).Distinct().OrderBy(x => x).ToListAsync(ct);
        var settings = jwtOptions.Value; var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        var profile = new UserProfileDto(user.Id, user.Email, user.FullName, user.PhoneNumber, user.UserType.ToString(), user.IsActive, user.IsExternalUser, user.SupplierId, user.CreatedAt, user.LastLoginAt);
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, [..CreateUserClaims(profile), ..roles.Select(r => new Claim(ClaimTypes.Role, r)), ..perms.Select(p => new Claim("permission", p))], expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), profile, roles, perms);
    }

    private AuthResponse? BuildResponseFromClaims(ClaimsPrincipal principal, Guid userId)
    {
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var fullName = principal.FindFirstValue(FullNameClaim) ?? email;
        var userType = principal.FindFirstValue("userType");
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(userType)) return null;

        var supplierIdClaim = principal.FindFirstValue(SupplierIdClaim);
        var lastLoginAtClaim = principal.FindFirstValue(LastLoginAtClaim);
        var profile = new UserProfileDto(
            userId,
            email,
            fullName,
            principal.FindFirstValue(PhoneNumberClaim),
            userType,
            IsActive: true,
            IsExternalUser: bool.TryParse(principal.FindFirstValue(IsExternalUserClaim), out var isExternalUser) && isExternalUser,
            SupplierId: Guid.TryParse(supplierIdClaim, out var supplierId) ? supplierId : null,
            CreatedAt: DateTimeOffset.TryParse(principal.FindFirstValue(CreatedAtClaim), out var createdAt) ? createdAt : DateTimeOffset.UnixEpoch,
            LastLoginAt: DateTimeOffset.TryParse(lastLoginAtClaim, out var lastLoginAt) ? lastLoginAt : null);
        return new AuthResponse(string.Empty, profile, principal.FindAll(ClaimTypes.Role).Select(x => x.Value).OrderBy(x => x).ToList(), principal.FindAll("permission").Select(x => x.Value).Distinct().OrderBy(x => x).ToList());
    }

    private static IEnumerable<Claim> CreateUserClaims(UserProfileDto profile)
    {
        yield return new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString());
        yield return new Claim(ClaimTypes.Email, profile.Email);
        yield return new Claim(FullNameClaim, profile.FullName);
        yield return new Claim("userType", profile.UserType);
        yield return new Claim(IsExternalUserClaim, profile.IsExternalUser.ToString());
        yield return new Claim(CreatedAtClaim, profile.CreatedAt.ToString("O"));
        if (!string.IsNullOrWhiteSpace(profile.PhoneNumber)) yield return new Claim(PhoneNumberClaim, profile.PhoneNumber);
        if (profile.SupplierId is not null) yield return new Claim(SupplierIdClaim, profile.SupplierId.Value.ToString());
        if (profile.LastLoginAt is not null) yield return new Claim(LastLoginAtClaim, profile.LastLoginAt.Value.ToString("O"));
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string permission) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) { context.Result = new UnauthorizedResult(); return; }
        var svc = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
        if (!await svc.HasPermissionAsync(context.HttpContext.User, permission, context.HttpContext.RequestAborted)) context.Result = new ForbidResult();
    }
}
