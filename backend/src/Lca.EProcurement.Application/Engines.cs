using System.Text.Json;
using Lca.EProcurement.Domain;

namespace Lca.EProcurement.Application;

public interface IAuditSink { void Record(string type, string entityType, Guid entityId, string reference, string actor, string details); }
public sealed class InMemoryAuditSink : IAuditSink { public List<AuditEvent> Events { get; } = []; public void Record(string t, string et, Guid id, string r, string a, string d) => Events.Add(new AuditEvent(t, et, id, r, a, d, DateTimeOffset.UtcNow)); }

public sealed record RuleResult(string RuleCode, bool Passed, string Message);
public sealed class BusinessRulesEngine(List<BusinessRuleDefinition> definitions, List<BusinessRuleExecutionLog> logs, IAuditSink audit)
{
    public RuleResult Evaluate(string ruleCode, Supplier supplier, string actor)
    {
        var rule = definitions.Single(r => r.Code == ruleCode && r.IsActive);
        var passed = rule.Expression switch
        {
            "HasDocument:CompanyRegistration" => supplier.Documents.Any(d => d.DocumentType == "CompanyRegistration"),
            "HasDocument:TaxClearance" => supplier.Documents.Any(d => d.DocumentType == "TaxClearance"),
            "HasAtLeastOneCategory" => supplier.Categories.Any(),
            _ => false
        };
        var result = new RuleResult(rule.Code, passed, passed ? "Rule passed" : $"Rule failed: {rule.Name}");
        logs.Add(new BusinessRuleExecutionLog(rule.Code, nameof(Supplier), supplier.Id, JsonSerializer.Serialize(supplier), passed ? RuleOutcome.Passed : RuleOutcome.Failed, JsonSerializer.Serialize(result), DateTimeOffset.UtcNow));
        audit.Record("Rule evaluated", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, result.Message);
        return result;
    }
}

public sealed class WorkflowEngine(List<WorkflowDefinition> definitions, List<WorkflowInstance> instances, List<WorkflowTask> tasks, List<WorkflowAction> actions, BusinessRulesEngine rules, IAuditSink audit)
{
    public WorkflowInstance Start(string workflowCode, Supplier supplier, string actor)
    {
        var definition = definitions.Single(d => d.Code == workflowCode && d.IsActive);
        var start = definition.Steps.First(s => s.Code == "Submitted");
        var instance = new WorkflowInstance(definition.Id, nameof(Supplier), supplier.Id, start.Code);
        instances.Add(instance);
        audit.Record("Workflow started", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, workflowCode);
        CreateTaskIfNeeded(definition, instance);
        return instance;
    }

    public WorkflowInstance Move(WorkflowInstance instance, Supplier supplier, string action, string actor)
    {
        var definition = definitions.Single(d => d.Id == instance.WorkflowDefinitionId);
        var transition = definition.Transitions.Single(t => t.FromStepCode == instance.CurrentStepCode && t.Action == action);
        if (transition.RequiredRuleCode is not null && !rules.Evaluate(transition.RequiredRuleCode, supplier, actor).Passed)
            throw new InvalidOperationException($"Workflow action '{action}' blocked by rule '{transition.RequiredRuleCode}'.");
        tasks.Where(t => t.WorkflowInstanceId == instance.Id && t.StepCode == instance.CurrentStepCode && t.Status == WorkflowTaskStatus.Open).ToList().ForEach(t => tasks[tasks.IndexOf(t)] = t with { Status = WorkflowTaskStatus.Completed });
        var next = instance with { CurrentStepCode = transition.ToStepCode, IsComplete = transition.ToStepCode is "Approved" or "Rejected" };
        instances[instances.IndexOf(instance)] = next;
        actions.Add(new WorkflowAction(instance.Id, action, transition.FromStepCode, transition.ToStepCode, actor, DateTimeOffset.UtcNow));
        audit.Record("Workflow moved step", nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, $"{transition.FromStepCode} -> {transition.ToStepCode}");
        CreateTaskIfNeeded(definition, next);
        return next;
    }
    private void CreateTaskIfNeeded(WorkflowDefinition d, WorkflowInstance i) { if (d.Steps.Single(s => s.Code == i.CurrentStepCode).CreatesTask) tasks.Add(new WorkflowTask(i.Id, i.CurrentStepCode, i.CurrentStepCode == "Approval" ? "Approver" : "ProcurementOfficer")); }
}
