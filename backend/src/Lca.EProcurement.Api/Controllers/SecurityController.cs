using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/security")]
[RequirePermission("Security.View")]
public sealed class SecurityController(EProcurementDbContext db, IPasswordService passwords) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<object> Dashboard(CancellationToken ct) => new { usersCount = await db.ApplicationUsers.CountAsync(ct), rolesCount = await db.Roles.CountAsync(ct), permissionsCount = await db.Permissions.CountAsync(ct), activeExternalSuppliers = await db.ApplicationUsers.CountAsync(x => x.IsExternalUser && x.IsActive, ct) };
    [HttpGet("users")]
    [RequirePermission("Security.Users")]
    public async Task<object[]> Users(CancellationToken ct) => await db.ApplicationUsers.AsNoTracking().Select(u => new { u.Id, u.Email, u.FullName, u.PhoneNumber, UserType = u.UserType.ToString(), u.IsActive, u.IsExternalUser, u.SupplierId, u.CreatedAt, u.LastLoginAt, Roles = db.UserRoles.Where(ur => ur.UserId == u.Id).Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name).ToArray() }).ToArrayAsync(ct);
    [HttpPost("users")]
    [RequirePermission("Security.Users")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken ct)
    {
        var user = new ApplicationUser(request.Email, request.FullName, request.PhoneNumber, request.UserType, true, request.IsExternalUser, request.SupplierId, DateTimeOffset.UtcNow, null, passwords.Hash(request.Password ?? "demo"));
        db.ApplicationUsers.Add(user); await db.SaveChangesAsync(ct);
        var roles = await db.Roles.Where(r => request.Roles.Contains(r.Name)).ToListAsync(ct);
        db.UserRoles.AddRange(roles.Select(r => new UserRole(user.Id, r.Id))); await db.SaveChangesAsync(ct); return Created($"/api/security/users/{user.Id}", user.Id);
    }
    [HttpPost("users/{id:guid}/deactivate")]
    [RequirePermission("Security.Users")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct) { await db.ApplicationUsers.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct); return NoContent(); }
    [HttpPost("users/{id:guid}/roles")]
    [RequirePermission("Security.Users")]
    public async Task<IActionResult> AssignRoles(Guid id, AssignRolesRequest request, CancellationToken ct) { db.UserRoles.RemoveRange(db.UserRoles.Where(x => x.UserId == id)); var roles = await db.Roles.Where(r => request.Roles.Contains(r.Name)).ToListAsync(ct); db.UserRoles.AddRange(roles.Select(r => new UserRole(id, r.Id))); await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpGet("roles")]
    [RequirePermission("Security.Roles")]
    public async Task<object[]> Roles(CancellationToken ct) => await db.Roles.AsNoTracking().Select(r => new { r.Id, r.Name, r.Description, r.IsActive, Permissions = db.RolePermissions.Where(rp => rp.RoleId == r.Id).Join(db.Permissions, rp => rp.PermissionId, p => p.Id, (_, p) => p.Code).ToArray() }).ToArrayAsync(ct);
    [HttpPost("roles/{id:guid}/permissions")]
    [RequirePermission("Security.Permissions")]
    public async Task<IActionResult> AssignPermissions(Guid id, AssignPermissionsRequest request, CancellationToken ct) { db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => x.RoleId == id)); var perms = await db.Permissions.Where(p => request.Permissions.Contains(p.Code)).ToListAsync(ct); db.RolePermissions.AddRange(perms.Select(p => new RolePermission(id, p.Id))); await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpGet("permissions")]
    [RequirePermission("Security.Permissions")]
    public async Task<Permission[]> Permissions(CancellationToken ct) => await db.Permissions.AsNoTracking().OrderBy(x => x.Category).ThenBy(x => x.Code).ToArrayAsync(ct);
}
