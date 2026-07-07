using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[RequirePermission("Budget.View")]
[Route("api/budgets")]
public sealed class BudgetsController(IBudgetApplicationService budgets) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await budgets.GetAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await budgets.GetAsync(id, ct)) is { } b ? Ok(b) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateBudgetDto dto, CancellationToken ct) { var b = await budgets.CreateAsync(dto, ct); return Created($"/api/budgets/{b.Id}", b); }
}
