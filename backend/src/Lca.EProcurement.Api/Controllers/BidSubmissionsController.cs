using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[RequirePermission("Bid.View")]
[Route("api/bids")]
public sealed class BidSubmissionsController(IBidSubmissionApplicationService bids, ISealedBidApplicationService sealedBids) : ControllerBase
{
    [HttpGet] public Task<List<Lca.EProcurement.Domain.BidSubmission>> Get(CancellationToken ct) => bids.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await bids.GetAsync(id, ct)) is { } bid ? Ok(bid) : NotFound();
    [RequirePermission("Bid.Create")]
    [HttpPost] public async Task<IActionResult> Create(CreateBidSubmissionDto dto, CancellationToken ct) { var bid = await bids.CreateAsync(dto, ct); return Created($"/api/bids/{bid.Id}", bid); }
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateBidSubmissionDto dto, CancellationToken ct) => (await bids.UpdateAsync(id, dto, ct)) is { } bid ? Ok(bid) : NotFound();
    [RequirePermission("Bid.Submit")]
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id, [FromBody] BidActorDto? dto, CancellationToken ct) => (await bids.SubmitAsync(id, dto?.Actor ?? "supplier@demo.ls", ct)) is { } bid ? Ok(bid) : NotFound();
    [HttpPost("{id:guid}/withdraw")] public async Task<IActionResult> Withdraw(Guid id, [FromBody] BidActorDto? dto, CancellationToken ct) => (await bids.WithdrawAsync(id, dto?.Actor ?? "supplier@demo.ls", ct)) is { } bid ? Ok(bid) : NotFound();
    [HttpGet("{id:guid}/documents")] public async Task<IActionResult> Documents(Guid id, [FromQuery] string actor = "procurement@lca.org.ls", CancellationToken ct = default) => await sealedBids.CanViewBidDocument(actor, id, Guid.Empty, ct) ? Ok(await bids.GetDocumentsAsync(id, ct)) : Forbid();
    [HttpPost("{id:guid}/documents")] public async Task<IActionResult> Upload(Guid id, UploadBidDocumentDto dto, CancellationToken ct) => (await bids.AddDocumentAsync(id, dto, ct)) is { } doc ? Ok(doc) : NotFound();
    [HttpGet("{id:guid}/history")] public Task<List<Lca.EProcurement.Domain.BidSubmissionHistory>> History(Guid id, CancellationToken ct) => bids.GetHistoryAsync(id, ct);
}
public sealed record BidActorDto(string Actor);
