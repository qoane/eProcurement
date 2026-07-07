using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/navigation")]
[RequirePermission("Studio.Navigation")]
public sealed class NavigationController(INavigationApplicationService navigation) : ControllerBase
{
    [HttpGet("main")]
    public async Task<IActionResult> GetMain(CancellationToken ct) => Ok(await navigation.GetAsync("MAIN", ct));

    [HttpPut("main")]
    public async Task<IActionResult> SaveMain(NavigationDesignerDto dto, CancellationToken ct) => Ok(await navigation.SaveAsync(dto with { Code = "MAIN" }, ct));
}
