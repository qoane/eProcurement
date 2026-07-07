using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[RequirePermission("BidOpening.View")]
[Route("api/bid-opening")]
public sealed class BidOpeningController(IBidOpeningApplicationService bidOpening) : ControllerBase
{
    [HttpGet] public Task<List<BidOpeningSummaryDto>> Get(CancellationToken ct) => bidOpening.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await bidOpening.GetAsync(id, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateBidOpeningSessionDto dto, CancellationToken ct) { var session = await bidOpening.CreateAsync(dto, ct); return Created($"/api/bid-opening/{session.Session.Id}", session); }
    [HttpPost("{id:guid}/schedule")] public async Task<IActionResult> Schedule(Guid id, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.ScheduleAsync(id, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost("{id:guid}/start")] public async Task<IActionResult> Start(Guid id, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.StartAsync(id, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost("{id:guid}/open-submission/{bidSubmissionId:guid}")] public async Task<IActionResult> OpenSubmission(Guid id, Guid bidSubmissionId, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.OpenSubmissionAsync(id, bidSubmissionId, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.CompleteAsync(id, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost("{id:guid}/refer-to-evaluation")] public async Task<IActionResult> ReferToEvaluation(Guid id, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.ReferToEvaluationAsync(id, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, BidOpeningActorDto dto, CancellationToken ct) => (await bidOpening.CancelAsync(id, dto, ct)) is { } session ? Ok(session) : NotFound();
    [HttpGet("{id:guid}/submissions")] public Task<List<Lca.EProcurement.Domain.BidOpeningSubmission>> Submissions(Guid id, CancellationToken ct) => bidOpening.GetSubmissionsAsync(id, ct);
    [HttpGet("{id:guid}/minutes")] public Task<List<Lca.EProcurement.Domain.BidOpeningMinute>> Minutes(Guid id, CancellationToken ct) => bidOpening.GetMinutesAsync(id, ct);
    [HttpPost("{id:guid}/minutes")] public async Task<IActionResult> AddMinute(Guid id, AddBidOpeningMinuteDto dto, CancellationToken ct) => (await bidOpening.AddMinuteAsync(id, dto, ct)) is { } minute ? Ok(minute) : NotFound();
    [HttpGet("{id:guid}/report")] public async Task<IActionResult> Report(Guid id, CancellationToken ct) => (await bidOpening.GetReportAsync(id, ct)) is { } report ? Ok(report) : NotFound();
}
