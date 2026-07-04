using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflows")]
public sealed class WorkflowsController(IWorkflowApplicationService workflows) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await workflows.GetDefinitionsAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateWorkflowDto dto, CancellationToken ct) => Created($"/api/workflows/{dto.Code}", await workflows.CreateWorkflowAsync(dto, ct));
    [HttpPost("{code}/publish")] public async Task<IActionResult> Publish(string code, PublishDto dto, CancellationToken ct) => (await workflows.PublishAsync(code, dto.Actor, ct)) is { } d ? Ok(d) : NotFound();
}
public sealed record PublishDto(string Actor);
