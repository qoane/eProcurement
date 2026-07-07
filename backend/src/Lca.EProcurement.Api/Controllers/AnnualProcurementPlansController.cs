using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/procurement-plans")]
public sealed class AnnualProcurementPlansController(IAnnualProcurementPlanApplicationService plans) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await plans.GetAsync(ct));
    [HttpGet("dashboard")] public async Task<IActionResult> Dashboard(CancellationToken ct) => Ok(await plans.DashboardAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await plans.GetAsync(id, ct)) is { } p ? Ok(p) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateAnnualProcurementPlanDto dto, CancellationToken ct) { var plan = await plans.CreateAsync(dto, ct); return Created($"/api/procurement-plans/{plan.Id}", plan); }
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id, [FromBody] WorkflowActorDto? dto, CancellationToken ct) => (await plans.SubmitAsync(id, dto?.Actor ?? "procurement@lca.org.ls", ct)) is { } p ? Ok(p) : NotFound();
    [HttpPost("{id:guid}/approve")] public async Task<IActionResult> Approve(Guid id, [FromBody] WorkflowActorDto? dto, CancellationToken ct) => (await plans.ApproveAsync(id, dto?.Actor ?? "approver@lca.org.ls", ct)) is { } p ? Ok(p) : NotFound();
}
public sealed record WorkflowActorDto(string Actor);
