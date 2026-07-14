using Lca.EProcurement.Application;
using Lca.EProcurement.Api.Security;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Lca.EProcurement.Api.Controllers;
[ApiController, Route("api/workflow-tasks")]
[RequirePermission("Workflow.Tasks")]
public sealed class WorkflowTasksController(IWorkflowApplicationService workflows, ISupplierApplicationService suppliers, EProcurementDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await workflows.GetTasksAsync(ct));
    [HttpGet("{id:guid}")] public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct) => (await suppliers.GetTaskDetailAsync(id, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/assign")] public async Task<IActionResult> Assign(Guid id, AssignTaskDto dto, CancellationToken ct) => (await workflows.AssignTaskAsync(id, dto.AssignedTo, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id, ActorDto dto, CancellationToken ct) => (await workflows.CompleteTaskAsync(id, dto.Actor, ct)) is { } t ? Ok(t) : NotFound();
    [HttpPost("run-escalations")] public async Task<IActionResult> RunEscalations(CancellationToken ct) { var now = DateTimeOffset.UtcNow; var rules = await db.EscalationRules.Where(x => x.IsActive).ToListAsync(ct); var count = 0; foreach (var task in await db.WorkflowTasks.Where(x => x.Status == WorkflowTaskStatus.Open || x.Status == WorkflowTaskStatus.Assigned).ToListAsync(ct)) { var rule = rules.FirstOrDefault(r => r.NodeCode == task.NodeCode && (r.AssignedRole == null || r.AssignedRole == task.AssignedRole)); if (rule is null || task.CreatedAt.AddHours(rule.EscalateAfterHours) > now) continue; db.WorkflowTaskEscalations.Add(new WorkflowTaskEscalation(task.Id, Guid.TryParse(task.AssignedTo, out var from) ? from : null, rule.EscalateToUserId, rule.EscalateToRole, "Task overdue", now)); db.AuditEvents.Add(new AuditEvent("Escalation triggered", nameof(WorkflowTask), task.Id, task.NodeCode, User.Identity?.Name ?? "system", "Workflow task escalation triggered", now)); count++; } await db.SaveChangesAsync(ct); return Ok(new { escalations = count }); }
    [HttpPost("{id:guid}/actions")] public async Task<IActionResult> Execute(Guid id, ExecuteWorkflowTaskActionDto dto, CancellationToken ct) => (await suppliers.ExecuteTaskActionAsync(id, dto, ct)) is { } r ? Ok(r) : NotFound();
}
public sealed record AssignTaskDto(string AssignedTo, string Actor);
public sealed record ActorDto(string Actor);
