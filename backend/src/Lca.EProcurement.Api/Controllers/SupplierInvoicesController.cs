using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;
[ApiController]
[Route("api/invoices")]
public sealed class SupplierInvoicesController(ISupplierInvoiceApplicationService invoices) : ControllerBase
{
    [HttpGet] public Task<List<Lca.EProcurement.Domain.SupplierInvoice>> Get([FromQuery] Guid? supplierId,CancellationToken ct)=>invoices.GetAsync(supplierId,ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id,CancellationToken ct)=>(await invoices.GetAsync(id,ct)) is { } i?Ok(i):NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateSupplierInvoiceDto dto,CancellationToken ct){var i=await invoices.CreateAsync(dto,ct); return Created($"/api/invoices/{i.Id}",i);} 
    [HttpPost("from-purchase-order/{purchaseOrderId:guid}")] public async Task<IActionResult> FromPo(Guid purchaseOrderId,ActorDto dto,CancellationToken ct){var i=await invoices.CreateFromPurchaseOrderAsync(purchaseOrderId,dto,ct); return Created($"/api/invoices/{i.Id}",i);} 
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id,ActorDto dto,CancellationToken ct)=>(await invoices.SubmitAsync(id,dto,ct)) is { } i?Ok(i):NotFound();
    [HttpPost("{id:guid}/run-match")] public async Task<IActionResult> RunMatch(Guid id,ActorDto dto,CancellationToken ct)=>(await invoices.RunMatchAsync(id,dto,ct)) is { } i?Ok(i):NotFound();
    [HttpPost("{id:guid}/approve-for-payment")] public async Task<IActionResult> Approve(Guid id,ActorDto dto,CancellationToken ct)=>(await invoices.ApproveForPaymentAsync(id,dto,ct)) is { } i?Ok(i):NotFound();
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id,RejectInvoiceDto dto,CancellationToken ct)=>(await invoices.RejectAsync(id,dto,ct)) is { } i?Ok(i):NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id,ActorDto dto,CancellationToken ct)=>(await invoices.CancelAsync(id,dto,ct)) is { } i?Ok(i):NotFound();
    [HttpGet("{id:guid}/history")] public Task<List<Lca.EProcurement.Domain.InvoiceStatusHistory>> History(Guid id,CancellationToken ct)=>invoices.GetHistoryAsync(id,ct);
    [HttpGet("{id:guid}/attachments")] public Task<List<Lca.EProcurement.Domain.InvoiceAttachment>> Attachments(Guid id,CancellationToken ct)=>invoices.GetAttachmentsAsync(id,ct);
    [HttpPost("{id:guid}/attachments")] public Task<Lca.EProcurement.Domain.InvoiceAttachment> AddAttachment(Guid id,InvoiceAttachmentDto dto,CancellationToken ct)=>invoices.AddAttachmentAsync(id,dto,ct);
}
