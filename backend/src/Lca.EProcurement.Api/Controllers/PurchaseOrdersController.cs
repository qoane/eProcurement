using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public sealed class PurchaseOrdersController(IPurchaseOrderApplicationService purchaseOrders) : ControllerBase
{
    [HttpGet] public Task<List<PurchaseOrderSummaryDto>> Get(CancellationToken ct) => purchaseOrders.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await purchaseOrders.GetAsync(id, ct)) is { } po ? Ok(po) : NotFound();
    [HttpPost("from-award/{awardId:guid}")] public async Task<IActionResult> FromAward(Guid awardId, PurchaseOrderActorDto dto, CancellationToken ct) { var po = await purchaseOrders.CreateFromAwardAsync(awardId, dto, ct); return Created($"/api/purchase-orders/{po.PurchaseOrder.Id}", po); }
    [HttpPost("{id:guid}/issue")] public async Task<IActionResult> Issue(Guid id, PurchaseOrderActorDto dto, CancellationToken ct) => (await purchaseOrders.IssueAsync(id, dto, ct)) is { } po ? Ok(po) : NotFound();
    [HttpPost("{id:guid}/acknowledge")] public async Task<IActionResult> Acknowledge(Guid id, PurchaseOrderActorDto dto, CancellationToken ct) => (await purchaseOrders.AcknowledgeAsync(id, dto, ct)) is { } po ? Ok(po) : NotFound();
    [HttpPost("{id:guid}/record-delivery")] public async Task<IActionResult> RecordDelivery(Guid id, RecordPurchaseOrderDeliveryDto dto, CancellationToken ct) => (await purchaseOrders.RecordDeliveryAsync(id, dto, ct)) is { } po ? Ok(po) : NotFound();
    [HttpPost("{id:guid}/close")] public async Task<IActionResult> Close(Guid id, PurchaseOrderActorDto dto, CancellationToken ct) => (await purchaseOrders.CloseAsync(id, dto, ct)) is { } po ? Ok(po) : NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, PurchaseOrderActorDto dto, CancellationToken ct) => (await purchaseOrders.CancelAsync(id, dto, ct)) is { } po ? Ok(po) : NotFound();
    [HttpGet("{id:guid}/history")] public Task<List<Lca.EProcurement.Domain.PurchaseOrderHistory>> History(Guid id, CancellationToken ct) => purchaseOrders.GetHistoryAsync(id, ct);
    [HttpGet("{id:guid}/goods-receipts")] public Task<List<Lca.EProcurement.Domain.GoodsReceipt>> GoodsReceipts(Guid id, CancellationToken ct) => purchaseOrders.GetGoodsReceiptsAsync(id, ct);
}
