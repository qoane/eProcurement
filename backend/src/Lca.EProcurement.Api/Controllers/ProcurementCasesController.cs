using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/procurement-cases")]
[RequirePermission("ProcurementCase.View")]
public sealed class ProcurementCasesController(IProcurementCaseApplicationService cases) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await cases.GetCasesAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await cases.GetCaseAsync(id, ct)) is { } c ? Ok(c) : NotFound();
    [HttpGet("{id:guid}/timeline")] public async Task<IActionResult> Timeline(Guid id, CancellationToken ct) => Ok(await cases.GetTimelineAsync(id, ct));
    [HttpGet("{id:guid}/audit")] public async Task<IActionResult> Audit(Guid id, CancellationToken ct) => Ok(await cases.GetAuditAsync(id, ct));
    [HttpGet("{id:guid}/documents")] public async Task<IActionResult> Documents(Guid id, CancellationToken ct) => Ok(await cases.GetDocumentsAsync(id, ct));
}
