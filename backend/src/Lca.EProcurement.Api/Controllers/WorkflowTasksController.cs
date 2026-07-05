using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflow-tasks")]
public sealed class WorkflowTasksController(IWorkflowApplicationService workflows, ISupplierApplicationService suppliers) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await workflows.GetTasksAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct) => (await suppliers.GetTaskDetailAsync(id, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/assign")] public async Task<IActionResult> Assign(Guid id, AssignTaskDto dto, CancellationToken ct) => (await workflows.AssignTaskAsync(id, dto.AssignedTo, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, ActorDto dto, CancellationToken ct) => (await workflows.CompleteTaskAsync(id, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/actions")] public async Task<IActionResult> Execute(Guid id, ExecuteWorkflowTaskActionDto dto, CancellationToken ct) => (await suppliers.ExecuteTaskActionAsync(id, dto, ct)) is { } r ? Ok(r) : NotFound();
}
public sealed record AssignTaskDto(string AssignedTo, string Actor);
public sealed record ActorDto(string Actor);
