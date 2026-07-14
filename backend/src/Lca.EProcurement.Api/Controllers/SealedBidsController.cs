using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[RequirePermission("Bid.View")]
public sealed class SealedBidsController(ISealedBidApplicationService sealedBids) : ControllerBase
{
    [HttpGet("api/bids/{id:guid}/sealed-envelope")]
    public async Task<IActionResult> Envelope(Guid id, CancellationToken ct) => (await sealedBids.GetEnvelopeAsync(id, ct)) is { } envelope ? Ok(envelope) : NotFound();

    [HttpGet("api/bids/{id:guid}/receipt")]
    public async Task<IActionResult> Receipt(Guid id, CancellationToken ct) => (await sealedBids.GetReceiptAsync(id, ct)) is { } receipt ? Ok(receipt) : NotFound();

    [HttpGet("api/bids/{id:guid}/access-log")]
    public Task<List<Lca.EProcurement.Domain.BidAccessLog>> AccessLog(Guid id, CancellationToken ct) => sealedBids.GetAccessLogAsync(id, ct);

    [HttpPost("api/bids/{id:guid}/integrity-check")]
    public async Task<IActionResult> IntegrityCheck(Guid id, [FromBody] BidActorDto? dto, CancellationToken ct) => (await sealedBids.CheckIntegrityAsync(id, dto?.Actor ?? "auditor@lca.org.ls", null, ct)) is { } result ? Ok(result) : NotFound();

    [HttpGet("api/bid-opening/{sessionId:guid}/evidence")]
    public Task<List<Lca.EProcurement.Domain.BidOpeningEvidence>> OpeningEvidence(Guid sessionId, CancellationToken ct) => sealedBids.GetOpeningEvidenceAsync(sessionId, ct);

    [HttpGet("api/bid-opening/{sessionId:guid}/evidence/{bidSubmissionId:guid}")]
    public async Task<IActionResult> OpeningEvidence(Guid sessionId, Guid bidSubmissionId, CancellationToken ct) => (await sealedBids.GetOpeningEvidenceAsync(sessionId, bidSubmissionId, ct)) is { } evidence ? Ok(evidence) : NotFound();
}
