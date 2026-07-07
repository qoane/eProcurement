using System.Text.Json;
using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Application;

public interface IAuditSink { void Record(string type, string entityType, Guid entityId, string reference, string actor, string details); }
public sealed class InMemoryAuditSink : IAuditSink { public List<AuditEvent> Events { get; } = []; public void Record(string t, string et, Guid id, string r, string a, string d) => Events.Add(new AuditEvent(t, et, id, r, a, d, DateTimeOffset.UtcNow)); }

public sealed record RuleResult(string RuleCode, bool Passed, string Message, string? RuleName = null, string? Expression = null, string? FailureMessage = null);
public sealed class BusinessRulesEngine(List<BusinessRuleDefinition> definitions, List<BusinessRuleExecutionLog> logs, IAuditSink audit)
{
    public RuleResult Evaluate(string ruleCode, Supplier supplier, string actor)
    {
        var rule = definitions.Single(r => r.Code == ruleCode && r.IsActive);
        var passed = SimpleExpressionEvaluator.Evaluate(rule.Expression, supplier);
        var result = new RuleResult(rule.Code, passed, passed ? $"Business rule {rule.Code} ({rule.Name}) passed." : $"Business rule {rule.Code} ({rule.Name}) failed: {rule.FailureMessage}", rule.Name, rule.Expression, rule.FailureMessage);
        logs.Add(new BusinessRuleExecutionLog(rule.Code, nameof(Supplier), supplier.Id, JsonSerializer.Serialize(supplier), passed ? RuleOutcome.Passed : RuleOutcome.Failed, JsonSerializer.Serialize(result), DateTimeOffset.UtcNow));
        audit.Record("Rule evaluated", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, result.Message);
        return result;
    }
}

public sealed class WorkflowEngine(List<WorkflowDefinition> definitions, List<WorkflowInstance> instances, List<WorkflowTask> tasks, List<WorkflowAction> actions, List<WorkflowHistory> history, BusinessRulesEngine rules, IAuditSink audit)
{
    public WorkflowDefinition CreateWorkflow(string code, string name, string entityType, IEnumerable<WorkflowNode> nodes, IEnumerable<WorkflowTransition> transitions)
    {
        var definition = new WorkflowDefinition(code, name, entityType);
        var version = new WorkflowVersion(definition.Id, 1);
        version.Nodes.AddRange(nodes.Select(n => n with { WorkflowVersionId = version.Id }));
        version.Transitions.AddRange(transitions.Select(t => t with { WorkflowVersionId = version.Id }));
        definition.Versions.Add(version);
        definitions.Add(definition);
        return definition;
    }

    public WorkflowDefinition PublishWorkflow(string code, string actor)
    {
        var definition = definitions.Single(d => d.Code == code && d.IsActive);
        var draft = definition.Versions.OrderByDescending(v => v.VersionNumber).First(v => v.Status == WorkflowVersionStatus.Draft);
        definition.Versions[definition.Versions.IndexOf(draft)] = draft with { Status = WorkflowVersionStatus.Published, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = actor };
        return definition with { PublishedVersionId = draft.Id };
    }

    public WorkflowDefinition CloneWorkflow(string sourceCode, string newCode, string newName)
    {
        var source = definitions.Single(d => d.Code == sourceCode && d.IsActive);
        var sourceVersion = PublishedVersion(source);
        return CreateWorkflow(newCode, newName, source.EntityType, sourceVersion.Nodes, sourceVersion.Transitions);
    }

    public WorkflowInstance Start(string workflowCode, Supplier supplier, string actor)
    {
        var definition = definitions.Single(d => d.Code == workflowCode && d.IsActive);
        var version = PublishedVersion(definition);
        var start = version.Nodes.Single(n => n.IsStart);
        var instance = new WorkflowInstance(definition.Id, version.Id, definition.EntityType, supplier.Id, start.Code, StartedAt: DateTimeOffset.UtcNow);
        instances.Add(instance);
        history.Add(new WorkflowHistory(instance.Id, "WorkflowStarted", start.Code, actor, workflowCode, DateTimeOffset.UtcNow));
        audit.Record("Workflow started", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, workflowCode);
        CreateTaskIfNeeded(version, instance, actor);
        return instance;
    }

    public WorkflowInstance ExecuteAction(WorkflowInstance instance, Supplier supplier, string actionCode, string actor)
    {
        var version = definitions.SelectMany(d => d.Versions).Single(v => v.Id == instance.WorkflowVersionId);
        var transition = version.Transitions.Single(t => t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == actionCode);
        if (transition.RequiredRuleCode is not null && !rules.Evaluate(transition.RequiredRuleCode, supplier, actor).Passed)
            throw new InvalidOperationException($"Workflow action '{actionCode}' cannot be completed because business rule '{transition.RequiredRuleCode}' was not satisfied.");
        tasks.Where(t => t.WorkflowInstanceId == instance.Id && t.NodeCode == instance.CurrentNodeCode && t.Status is WorkflowTaskStatus.Open or WorkflowTaskStatus.Assigned).ToList().ForEach(t => tasks[tasks.IndexOf(t)] = t with { Status = WorkflowTaskStatus.Completed, CompletedAt = DateTimeOffset.UtcNow });
        var target = version.Nodes.Single(n => n.Code == transition.ToNodeCode);
        var next = instance with { CurrentNodeCode = target.Code, Status = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running, CompletedAt = target.IsTerminal ? DateTimeOffset.UtcNow : null };
        instances[instances.IndexOf(instance)] = next;
        actions.Add(new WorkflowAction(instance.Id, actionCode, transition.ActionName, WorkflowActionKind.Transition, transition.FromNodeCode, transition.ToNodeCode, actor, DateTimeOffset.UtcNow));
        history.Add(new WorkflowHistory(instance.Id, "ActionExecuted", target.Code, actor, $"{transition.FromNodeCode} -> {transition.ToNodeCode} via {actionCode}", DateTimeOffset.UtcNow));
        audit.Record("Workflow moved step", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, $"{transition.FromNodeCode} -> {transition.ToNodeCode}");
        CreateTaskIfNeeded(version, next, actor);
        return next;
    }

    public WorkflowTask AssignTask(Guid taskId, string assignee, string actor)
    {
        var task = tasks.Single(t => t.Id == taskId && t.Status == WorkflowTaskStatus.Open);
        var assigned = task with { AssignedTo = assignee, Status = WorkflowTaskStatus.Assigned, AssignedAt = DateTimeOffset.UtcNow };
        tasks[tasks.IndexOf(task)] = assigned;
        history.Add(new WorkflowHistory(task.WorkflowInstanceId, "TaskAssigned", task.NodeCode, actor, assignee, DateTimeOffset.UtcNow, task.Id));
        return assigned;
    }

    public WorkflowTask CompleteTask(Guid taskId, string actor)
    {
        var task = tasks.Single(t => t.Id == taskId && t.Status is WorkflowTaskStatus.Open or WorkflowTaskStatus.Assigned);
        var completed = task with { Status = WorkflowTaskStatus.Completed, CompletedAt = DateTimeOffset.UtcNow };
        tasks[tasks.IndexOf(task)] = completed;
        history.Add(new WorkflowHistory(task.WorkflowInstanceId, "TaskCompleted", task.NodeCode, actor, "Task completed", DateTimeOffset.UtcNow, task.Id));
        return completed;
    }

    public WorkflowInstance CancelWorkflow(WorkflowInstance instance, string actor, string reason)
    {
        var cancelled = instance with { Status = WorkflowInstanceStatus.Cancelled, CancelledAt = DateTimeOffset.UtcNow };
        instances[instances.IndexOf(instance)] = cancelled;
        history.Add(new WorkflowHistory(instance.Id, "WorkflowCancelled", instance.CurrentNodeCode, actor, reason, DateTimeOffset.UtcNow));
        return cancelled;
    }

    private static WorkflowVersion PublishedVersion(WorkflowDefinition definition) => definition.Versions.Single(v => v.Id == definition.PublishedVersionId || v.Status == WorkflowVersionStatus.Published);
    private void CreateTaskIfNeeded(WorkflowVersion v, WorkflowInstance i, string actor)
    {
        var node = v.Nodes.Single(n => n.Code == i.CurrentNodeCode);
        if (!node.CreatesTask) return;
        var task = new WorkflowTask(i.Id, i.CurrentNodeCode, node.DefaultAssignedRole, CreatedAt: DateTimeOffset.UtcNow);
        tasks.Add(task);
        history.Add(new WorkflowHistory(i.Id, "TaskCreated", i.CurrentNodeCode, actor, node.DefaultAssignedRole ?? "Unassigned", DateTimeOffset.UtcNow, task.Id));
    }
}
