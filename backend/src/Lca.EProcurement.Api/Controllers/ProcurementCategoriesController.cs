using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/procurement-categories")]
public sealed class ProcurementCategoriesController(IBudgetApplicationService budgets) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await budgets.GetProcurementCategoriesAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateProcurementCategoryDto dto, CancellationToken ct) { var c = await budgets.CreateProcurementCategoryAsync(dto, ct); return Created($"/api/procurement-categories/{c.Id}", c); }
}
