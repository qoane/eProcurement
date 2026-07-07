using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/evaluations")]
public sealed class EvaluationsController(IEvaluationApplicationService evaluations) : ControllerBase
{
    [HttpGet] public Task<List<EvaluationSummaryDto>> Get(CancellationToken ct) => evaluations.GetAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await evaluations.GetAsync(id, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(CreateEvaluationSessionDto dto, CancellationToken ct) { var e = await evaluations.CreateAsync(dto, ct); return Created($"/api/evaluations/{e.Session.Id}", e); }
    [HttpPost("{id:guid}/schedule")] public async Task<IActionResult> Schedule(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.ScheduleAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/start")] public async Task<IActionResult> Start(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.StartAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/declare-conflict")] public async Task<IActionResult> Declare(Guid id, EvaluationDeclarationDto dto, CancellationToken ct) => (await evaluations.DeclareConflictAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/score")] public async Task<IActionResult> Score(Guid id, EvaluationScoreDto dto, CancellationToken ct) => (await evaluations.ScoreAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/consensus")] public async Task<IActionResult> Consensus(Guid id, EvaluationConsensusScoreDto dto, CancellationToken ct) => (await evaluations.ConsensusAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/recommend")] public async Task<IActionResult> Recommend(Guid id, EvaluationRecommendationDto dto, CancellationToken ct) => (await evaluations.RecommendAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.CompleteAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/refer-to-award")] public async Task<IActionResult> Refer(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.ReferToAwardAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.CancelAsync(id, dto, ct)) is { } e ? Ok(e) : NotFound();
    [HttpGet("{id:guid}/submissions")] public async Task<IActionResult> Submissions(Guid id, CancellationToken ct) => (await evaluations.GetAsync(id, ct)) is { } e ? Ok(e.Submissions) : NotFound();
    [HttpGet("{id:guid}/committee")] public async Task<IActionResult> Committee(Guid id, CancellationToken ct) => (await evaluations.GetAsync(id, ct)) is { } e ? Ok(e.Committee) : NotFound();
    [HttpGet("{id:guid}/scores")] public async Task<IActionResult> Scores(Guid id, CancellationToken ct) => (await evaluations.GetAsync(id, ct)) is { } e ? Ok(new { e.Scores, e.ConsensusScores }) : NotFound();
    [HttpGet("{id:guid}/report")] public async Task<IActionResult> Report(Guid id, CancellationToken ct) => (await evaluations.GetAsync(id, ct)) is { } e ? Ok(e.Reports.FirstOrDefault()) : NotFound();
}

[ApiController]
[Route("api/evaluation-templates")]
public sealed class EvaluationTemplatesController(IEvaluationApplicationService evaluations) : ControllerBase
{
    [HttpGet] public Task<List<Lca.EProcurement.Domain.EvaluationTemplate>> Get(CancellationToken ct) => evaluations.GetTemplatesAsync(ct);
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, CancellationToken ct) => (await evaluations.GetTemplateAsync(id, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost] public Task<Lca.EProcurement.Domain.EvaluationTemplate> Create(CreateEvaluationTemplateDto dto, CancellationToken ct) => evaluations.CreateTemplateAsync(dto, ct);
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish(Guid id, EvaluationActorDto dto, CancellationToken ct) => (await evaluations.PublishTemplateAsync(id, dto, ct)) is { } t ? Ok(t) : NotFound();
}
