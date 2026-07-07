using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/procurement-categories")]
public sealed class ProcurementCategoriesController(IProcurementCategoryApplicationService categories) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await categories.GetAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateProcurementCategoryDto dto, CancellationToken ct) { var c = await categories.CreateAsync(dto, ct); return Created($"/api/procurement-categories/{c.Id}", c); }
}
