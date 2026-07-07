using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflows")]
[RequirePermission("Studio.Workflows")]
public sealed class WorkflowsController(IWorkflowApplicationService workflows) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await workflows.GetDefinitionsAsync(ct));
    [HttpGet("dashboard")] public async Task<IActionResult> Dashboard(CancellationToken ct) => Ok(await workflows.GetDashboardAsync(ct));
    [HttpPost] public async Task<IActionResult> Create(CreateWorkflowDto dto, CancellationToken ct) => Created($"/api/workflows/{dto.Code}", await workflows.CreateWorkflowAsync(dto, ct));
    [HttpPost("{code}/nodes")] public async Task<IActionResult> AddNode(string code, WorkflowNodeDto dto, CancellationToken ct) => (await workflows.AddNodeAsync(code, dto, ct)) is { } n ? Created($"/api/workflows/{code}/nodes/{dto.Code}", n) : NotFound();
    [HttpPost("{code}/transitions")] public async Task<IActionResult> AddTransition(string code, WorkflowTransitionDto dto, CancellationToken ct) => (await workflows.AddTransitionAsync(code, dto, ct)) is { } t ? Created($"/api/workflows/{code}/transitions/{dto.ActionCode}", t) : NotFound();
    [HttpPost("designer")] public async Task<IActionResult> SaveDesigner(WorkflowDesignerDto dto, CancellationToken ct) => Ok(await workflows.SaveDesignerAsync(dto, ct));
    [HttpPost("{code}/publish")] public async Task<IActionResult> Publish(string code, PublishDto dto, CancellationToken ct) => (await workflows.PublishAsync(code, dto.Actor, ct)) is { } d ? Ok(d) : NotFound();
    [HttpPost("{code}/versions/{versionNumber:int}/archive")] public async Task<IActionResult> Archive(string code, int versionNumber, PublishDto dto, CancellationToken ct) => (await workflows.ArchiveAsync(code, versionNumber, dto.Actor, ct)) is { } d ? Ok(d) : NotFound();
}
public sealed record PublishDto(string Actor);
