using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/cost-centres")]
public sealed class CostCentresController(ICostCentreApplicationService costCentres) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await costCentres.GetAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateCostCentreDto dto, CancellationToken ct) { var c = await costCentres.CreateAsync(dto, ct); return Created($"/api/cost-centres/{c.Id}", c); }
}
