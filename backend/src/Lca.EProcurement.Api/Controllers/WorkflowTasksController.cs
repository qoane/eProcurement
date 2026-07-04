using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflow-tasks")]
public sealed class WorkflowTasksController(IWorkflowApplicationService workflows) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await workflows.GetTasksAsync(ct));
    [HttpPost("{id:guid}/assign")] public async Task<IActionResult> Assign(Guid id, AssignTaskDto dto, CancellationToken ct) => (await workflows.AssignTaskAsync(id, dto.AssignedTo, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, ActorDto dto, CancellationToken ct) => (await workflows.CompleteTaskAsync(id, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
}
public sealed record AssignTaskDto(string AssignedTo, string Actor);
public sealed record ActorDto(string Actor);
