using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/requisitions")]
public sealed class RequisitionsController(IRequisitionApplicationService requisitions) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await requisitions.GetAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await requisitions.GetAsync(id, ct)) is { } r ? Ok(r) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateRequisitionDto dto, CancellationToken ct) { var r = await requisitions.CreateAsync(dto, ct); return Created($"/api/requisitions/{r.Id}", r); }
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id, [FromBody] RequisitionActorDto? dto, CancellationToken ct) => (await requisitions.SubmitAsync(id, dto?.Actor ?? "requester@lca.org.ls", ct)) is { } r ? Ok(r) : NotFound();
    [HttpPost("{id:guid}/approve")] public async Task<IActionResult> Approve(Guid id, [FromBody] RequisitionActorDto? dto, CancellationToken ct) => (await requisitions.ApproveAsync(id, dto?.Actor ?? "approver@lca.org.ls", ct)) is { } r ? Ok(r) : NotFound();
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequisitionDto? dto, CancellationToken ct) => (await requisitions.RejectAsync(id, dto?.Actor ?? "approver@lca.org.ls", dto?.Reason ?? "Rejected", ct)) is { } r ? Ok(r) : NotFound();
    [HttpGet("{id:guid}/budget-validation")] public async Task<IActionResult> BudgetValidation(Guid id, CancellationToken ct) => Ok(await requisitions.ValidateBudgetAsync(id, ct));
}
public sealed record RequisitionActorDto(string Actor);
public sealed record RejectRequisitionDto(string Actor, string Reason);
