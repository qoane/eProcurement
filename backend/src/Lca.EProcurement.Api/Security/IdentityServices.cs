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
        return Guid.TryParse(id, out var userId) ? await BuildResponseAsync(userId, ct) : null;
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
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email), new Claim("userType", user.UserType.ToString()), ..roles.Select(r => new Claim(ClaimTypes.Role, r)), ..perms.Select(p => new Claim("permission", p))], expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), new(user.Id, user.Email, user.FullName, user.PhoneNumber, user.UserType.ToString(), user.IsActive, user.IsExternalUser, user.SupplierId, user.CreatedAt, user.LastLoginAt), roles, perms);
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
