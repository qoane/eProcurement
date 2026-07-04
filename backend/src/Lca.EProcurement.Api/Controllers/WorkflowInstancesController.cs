using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflow-instances")]
public sealed class WorkflowInstancesController(IWorkflowApplicationService workflows) : ControllerBase
{
    [HttpPost("start")] public async Task<IActionResult> Start(StartWorkflowDto dto, CancellationToken ct) => Created("", await workflows.StartAsync(dto.WorkflowCode, dto.EntityType, dto.EntityId, dto.Actor, ct));
    [HttpPost("{id:guid}/actions")] public async Task<IActionResult> Action(Guid id, WorkflowActionDto dto, CancellationToken ct) => (await workflows.ExecuteActionAsync(id, dto.ActionCode, dto.Actor, ct)) is { } i ? Ok(i) : NotFound();
}
public sealed record StartWorkflowDto(string WorkflowCode, string EntityType, Guid EntityId, string Actor);
public sealed record WorkflowActionDto(string ActionCode, string Actor);
