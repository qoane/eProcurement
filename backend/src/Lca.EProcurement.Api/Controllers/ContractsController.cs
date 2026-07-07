using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController, Route("api/contracts")]
public sealed class ContractsController(IContractApplicationService contracts) : ControllerBase
{
    [HttpGet] public Task<List<ContractSummaryDto>> Get(CancellationToken ct) => contracts.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await contracts.GetAsync(id, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("from-award/{awardId:guid}")] public async Task<IActionResult> FromAward(Guid awardId, ContractCreateDto dto, CancellationToken ct) { var c = await contracts.CreateFromAwardAsync(awardId, dto, ct); return Created($"/api/contracts/{c.Contract.Id}", c); }
    [HttpPost("from-purchase-order/{purchaseOrderId:guid}")] public async Task<IActionResult> FromPurchaseOrder(Guid purchaseOrderId, ContractCreateDto dto, CancellationToken ct) { var c = await contracts.CreateFromPurchaseOrderAsync(purchaseOrderId, dto, ct); return Created($"/api/contracts/{c.Contract.Id}", c); }
    [HttpPost("{id:guid}/activate")] public async Task<IActionResult> Activate(Guid id, ContractActorDto dto, CancellationToken ct) => (await contracts.ActivateAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, ContractActorDto dto, CancellationToken ct) => (await contracts.CompleteAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("{id:guid}/terminate")] public async Task<IActionResult> Terminate(Guid id, ContractActorDto dto, CancellationToken ct) => (await contracts.TerminateAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("{id:guid}/renew")] public async Task<IActionResult> Renew(Guid id, ContractRenewDto dto, CancellationToken ct) => (await contracts.RenewAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpPost("{id:guid}/variation")] public async Task<IActionResult> Variation(Guid id, ContractVariationDto dto, CancellationToken ct) => (await contracts.AddVariationAsync(id, dto, ct)) is { } c ? Ok(c) : NotFound();
    [HttpGet("{id:guid}/milestones")] public Task<List<Lca.EProcurement.Domain.ContractMilestone>> Milestones(Guid id, CancellationToken ct) => contracts.GetMilestonesAsync(id, ct);
    [HttpPost("{id:guid}/milestones")] public Task<Lca.EProcurement.Domain.ContractMilestone> AddMilestone(Guid id, ContractMilestoneDto dto, CancellationToken ct) => contracts.AddMilestoneAsync(id, dto, ct);
    [HttpGet("{id:guid}/performance")] public Task<List<Lca.EProcurement.Domain.ContractPerformanceReview>> Performance(Guid id, CancellationToken ct) => contracts.GetPerformanceAsync(id, ct);
    [HttpPost("{id:guid}/performance")] public Task<Lca.EProcurement.Domain.ContractPerformanceReview> AddPerformance(Guid id, ContractPerformanceDto dto, CancellationToken ct) => contracts.AddPerformanceAsync(id, dto, ct);
}
