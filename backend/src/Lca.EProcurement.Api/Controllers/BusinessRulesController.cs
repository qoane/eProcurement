using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/business-rules")]
public sealed class BusinessRulesController(IBusinessRuleApplicationService rules) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await rules.GetRulesAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateBusinessRuleDto dto, CancellationToken ct) => Created($"/api/business-rules/{dto.Code}", await rules.CreateRuleAsync(dto, ct));
    [HttpPost("{code}/evaluate")] public async Task<IActionResult> Evaluate(string code, EvaluateRuleDto dto, CancellationToken ct) => Ok(await rules.EvaluateAsync(code, dto.EntityType, dto.EntityId, dto.Actor, ct));
}
public sealed record EvaluateRuleDto(string EntityType, Guid EntityId, string Actor);
