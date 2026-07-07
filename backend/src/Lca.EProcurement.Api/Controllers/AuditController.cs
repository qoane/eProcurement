using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/audit-events")]
[RequirePermission("Audit.View")]
public sealed class AuditController(IAuditApplicationService audit) : ControllerBase
{ [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await audit.GetEventsAsync(ct)); }
