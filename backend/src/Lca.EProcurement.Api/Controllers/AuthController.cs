using Lca.EProcurement.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IIdentityService identity) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await identity.LoginAsync(request.Email, request.Password, ct);
        return response is null ? Unauthorized() : Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Me(CancellationToken ct)
    {
        var response = await identity.CurrentAsync(User, ct);
        return response is null ? Unauthorized() : Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => NoContent();
}
