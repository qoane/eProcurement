using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/cost-centres")]
public sealed class CostCentresController(IBudgetApplicationService budgets) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await budgets.GetCostCentresAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateCostCentreDto dto, CancellationToken ct) { var c = await budgets.CreateCostCentreAsync(dto, ct); return Created($"/api/cost-centres/{c.Id}", c); }
}
