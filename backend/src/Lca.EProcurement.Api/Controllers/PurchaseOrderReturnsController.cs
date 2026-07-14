using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/purchase-order-returns")]
public sealed class PurchaseOrderReturnsController(IPurchaseOrderReturnApplicationService returns) : ControllerBase
{
    [HttpGet] public Task<List<Lca.EProcurement.Domain.PurchaseOrderReturn>> Get(CancellationToken ct)=>returns.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id,CancellationToken ct)=>(await returns.GetAsync(id,ct)) is { } r?Ok(r):NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreatePurchaseOrderReturnDto dto,CancellationToken ct){var r=await returns.CreateAsync(dto,ct); return Created($"/api/purchase-order-returns/{r.Id}",r);} 
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id,ActorDto dto,CancellationToken ct)=>(await returns.SubmitAsync(id,dto,ct)) is { } r?Ok(r):NotFound();
    [HttpPost("{id:guid}/approve")] public async Task<IActionResult> Approve(Guid id,ActorDto dto,CancellationToken ct)=>(await returns.ApproveAsync(id,dto,ct)) is { } r?Ok(r):NotFound();
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id,ActorDto dto,CancellationToken ct)=>(await returns.RejectAsync(id,dto,ct)) is { } r?Ok(r):NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id,ActorDto dto,CancellationToken ct)=>(await returns.CompleteAsync(id,dto,ct)) is { } r?Ok(r):NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id,ActorDto dto,CancellationToken ct)=>(await returns.CancelAsync(id,dto,ct)) is { } r?Ok(r):NotFound();
}
