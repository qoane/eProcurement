using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/business-rules")]
[RequirePermission("Studio.Rules")]
public sealed class BusinessRulesController(IBusinessRuleApplicationService rules) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await rules.GetRulesAsync(ct));
    [HttpGet("designer-metadata")] public IActionResult Metadata([FromQuery] string appliesTo = "Supplier") => Ok(rules.GetDesignerMetadata(appliesTo));
    [HttpGet("history")] public async Task<IActionResult> History([FromQuery] string? ruleCode, CancellationToken ct) => Ok(await rules.GetHistoryAsync(ruleCode, ct));
    [HttpPost] public async Task<IActionResult> Create(CreateBusinessRuleDto dto, CancellationToken ct) => Created($"/api/business-rules/{dto.Code}", await rules.CreateRuleAsync(dto, ct));
    [HttpPost("validate")] public async Task<IActionResult> Validate(RuleExpressionDto dto, CancellationToken ct) => Ok(await rules.ValidateAsync(dto, ct));
    [HttpPost("simulate")] public async Task<IActionResult> Simulate(RuleSimulationDto dto, CancellationToken ct) => Ok(await rules.SimulateAsync(dto, ct));
    [HttpPost("{code}/publish")] public async Task<IActionResult> Publish(string code, ActorDto dto, CancellationToken ct) => (await rules.PublishAsync(code, dto.Actor, ct)) is { } r ? Ok(r) : NotFound();
    [HttpPost("{code}/evaluate")] public async Task<IActionResult> Evaluate(string code, EvaluateRuleDto dto, CancellationToken ct) => Ok(await rules.EvaluateAsync(code, dto.EntityType, dto.EntityId, dto.Actor, ct));
}
public sealed record EvaluateRuleDto(string EntityType, Guid EntityId, string Actor);
