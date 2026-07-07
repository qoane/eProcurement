using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/awards")]
public sealed class AwardsController(IAwardApplicationService awards) : ControllerBase
{
    [HttpGet] public Task<List<AwardSummaryDto>> Get(CancellationToken ct) => awards.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await awards.GetAsync(id, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("from-evaluation/{evaluationSessionId:guid}")] public async Task<IActionResult> FromEvaluation(Guid evaluationSessionId, AwardActorDto dto, CancellationToken ct) { var a = await awards.CreateFromEvaluationAsync(evaluationSessionId, dto, ct); return Created($"/api/awards/{a.Award.Id}", a); }
    [HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id, AwardActorDto dto, CancellationToken ct) => (await awards.SubmitAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/approve")] public async Task<IActionResult> Approve(Guid id, AwardApprovalDto dto, CancellationToken ct) => (await awards.ApproveAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id, AwardDecisionDto dto, CancellationToken ct) => (await awards.RejectAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish(Guid id, AwardActorDto dto, CancellationToken ct) => (await awards.PublishAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, AwardActorDto dto, CancellationToken ct) => (await awards.CancelAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/convert-to-purchase-order")] public async Task<IActionResult> ConvertToPurchaseOrder(Guid id, AwardActorDto dto, CancellationToken ct) => (await awards.ConvertToPurchaseOrderAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpPost("{id:guid}/convert-to-contract")] public async Task<IActionResult> ConvertToContract(Guid id, AwardActorDto dto, CancellationToken ct) => (await awards.ConvertToContractAsync(id, dto, ct)) is { } a ? Ok(a) : NotFound();
    [HttpGet("{id:guid}/history")] public Task<List<Lca.EProcurement.Domain.AwardHistory>> History(Guid id, CancellationToken ct) => awards.GetHistoryAsync(id, ct);
    [HttpGet("{id:guid}/notifications")] public Task<List<Lca.EProcurement.Domain.AwardNotification>> Notifications(Guid id, CancellationToken ct) => awards.GetNotificationsAsync(id, ct);
    [HttpGet("{id:guid}/report")] public async Task<IActionResult> Report(Guid id, CancellationToken ct) => (await awards.GetReportAsync(id, ct)) is { } r ? Ok(r) : NotFound();
}
