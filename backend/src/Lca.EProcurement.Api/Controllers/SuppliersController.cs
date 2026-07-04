using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/suppliers")]
public sealed class SuppliersController(ISupplierApplicationService suppliers) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await suppliers.GetSuppliersAsync(ct));
    [HttpPost("{referenceNumber}/submit")] public async Task<IActionResult> Submit(string referenceNumber, ActorDto dto, CancellationToken ct) => (await suppliers.SubmitAsync(referenceNumber, dto.Actor, ct)) is { } i ? Ok(i) : NotFound();
}
