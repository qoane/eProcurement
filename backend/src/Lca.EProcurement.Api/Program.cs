using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCors = "ViteFrontend";
builder.Services.AddCors(options => options.AddPolicy(FrontendCors, policy => policy.WithOrigins("http://localhost:5173", "https://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EProcurementDbContext>(options => options.UseConfiguredProvider(builder.Configuration["Database:Provider"] ?? "SqlServer", builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EProcurementDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
    return;
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EProcurementDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
}

app.UseCors(FrontendCors);
app.MapGet("/", () => Results.Ok(new { name = "LCA eProcurement API", status = "running", documentation = "/swagger", health = "/health" }));
app.MapGet("/health", async (EProcurementDbContext db) => Results.Ok(new { status = "healthy", platform = "LCA eProcurement", provider = db.Database.ProviderName, database = db.Database.GetDbConnection().Database }));

app.MapGet("/api/workflows", async (EProcurementDbContext db) => await db.WorkflowDefinitions.AsNoTracking().Include(w => w.Versions).ThenInclude(v => v.Nodes).Include(w => w.Versions).ThenInclude(v => v.Transitions).OrderBy(w => w.Code).ToListAsync()).WithTags("Workflow Engine");
app.MapPost("/api/workflows", async (CreateWorkflowRequest request, EProcurementDbContext db) =>
{
    if (await db.WorkflowDefinitions.AnyAsync(w => w.Code == request.Code)) return Results.Conflict($"Workflow '{request.Code}' already exists.");
    var definition = new WorkflowDefinition(request.Code, request.Name, request.EntityType);
    var version = new WorkflowVersion(definition.Id, 1);
    version.Nodes.AddRange(request.Nodes.Select(n => new WorkflowNode(version.Id, n.Code, n.Name, n.Kind, n.CreatesTask, n.DefaultAssignedRole, n.IsStart, n.IsTerminal)));
    version.Transitions.AddRange(request.Transitions.Select(t => new WorkflowTransition(version.Id, t.FromNodeCode, t.ActionCode, t.ActionName, t.ToNodeCode, t.RequiredRuleCode)));
    definition.Versions.Add(version);
    db.WorkflowDefinitions.Add(definition);
    await db.SaveChangesAsync();
    return Results.Created($"/api/workflows/{definition.Code}", definition);
}).WithTags("Workflow Engine");
app.MapPost("/api/workflows/{code}/publish", async (string code, PublishWorkflowRequest request, EProcurementDbContext db) =>
{
    var definition = await db.WorkflowDefinitions.Include(w => w.Versions).ThenInclude(v => v.Nodes).SingleOrDefaultAsync(w => w.Code == code && w.IsActive);
    if (definition is null) return Results.NotFound();
    var draft = definition.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault(v => v.Status == WorkflowVersionStatus.Draft);
    if (draft is null) return Results.BadRequest("No draft workflow version is available to publish.");
    db.Entry(draft).CurrentValues[nameof(WorkflowVersion.Status)] = WorkflowVersionStatus.Published;
    db.Entry(draft).CurrentValues[nameof(WorkflowVersion.PublishedAt)] = DateTimeOffset.UtcNow;
    db.Entry(draft).CurrentValues[nameof(WorkflowVersion.PublishedBy)] = request.Actor;
    db.Entry(definition).CurrentValues[nameof(WorkflowDefinition.PublishedVersionId)] = draft.Id;
    await db.SaveChangesAsync();
    return Results.Ok(new { definition.Code, PublishedVersionId = draft.Id });
}).WithTags("Workflow Engine");
app.MapPost("/api/workflows/{code}/clone", async (string code, CloneWorkflowRequest request, EProcurementDbContext db) =>
{
    var source = await db.WorkflowDefinitions.AsNoTracking().Include(w => w.Versions).ThenInclude(v => v.Nodes).Include(w => w.Versions).ThenInclude(v => v.Transitions).SingleOrDefaultAsync(w => w.Code == code && w.IsActive);
    if (source is null) return Results.NotFound();
    if (await db.WorkflowDefinitions.AnyAsync(w => w.Code == request.NewCode)) return Results.Conflict($"Workflow '{request.NewCode}' already exists.");
    var sourceVersion = source.Versions.Single(v => v.Id == source.PublishedVersionId || v.Status == WorkflowVersionStatus.Published);
    var clone = new WorkflowDefinition(request.NewCode, request.NewName, source.EntityType);
    var version = new WorkflowVersion(clone.Id, 1);
    version.Nodes.AddRange(sourceVersion.Nodes.Select(n => new WorkflowNode(version.Id, n.Code, n.Name, n.Kind, n.CreatesTask, n.DefaultAssignedRole, n.IsStart, n.IsTerminal)));
    version.Transitions.AddRange(sourceVersion.Transitions.Select(t => new WorkflowTransition(version.Id, t.FromNodeCode, t.ActionCode, t.ActionName, t.ToNodeCode, t.RequiredRuleCode)));
    clone.Versions.Add(version);
    db.WorkflowDefinitions.Add(clone);
    await db.SaveChangesAsync();
    return Results.Created($"/api/workflows/{clone.Code}", clone);
}).WithTags("Workflow Engine");
app.MapPost("/api/workflow-instances/start", async (StartWorkflowRequest request, EProcurementDbContext db) =>
{
    var definition = await LoadWorkflow(db, request.WorkflowCode);
    if (definition is null) return Results.NotFound();
    var version = definition.Versions.Single(v => v.Id == definition.PublishedVersionId || v.Status == WorkflowVersionStatus.Published);
    var start = version.Nodes.Single(n => n.IsStart);
    var instance = new WorkflowInstance(definition.Id, version.Id, request.EntityType, request.EntityId, start.Code, StartedAt: DateTimeOffset.UtcNow);
    db.WorkflowInstances.Add(instance);
    db.WorkflowHistories.Add(new WorkflowHistory(instance.Id, "WorkflowStarted", start.Code, request.Actor, request.WorkflowCode, DateTimeOffset.UtcNow));
    await CreateTaskIfNeeded(db, version, instance, request.Actor);
    await db.SaveChangesAsync();
    return Results.Created($"/api/workflow-instances/{instance.Id}", instance);
}).WithTags("Workflow Engine");
app.MapPost("/api/workflow-instances/{id:guid}/actions", async (Guid id, ExecuteWorkflowActionRequest request, EProcurementDbContext db) =>
{
    var instance = await db.WorkflowInstances.SingleOrDefaultAsync(i => i.Id == id);
    if (instance is null) return Results.NotFound();
    var version = await db.WorkflowVersions.Include(v => v.Nodes).Include(v => v.Transitions).SingleAsync(v => v.Id == instance.WorkflowVersionId);
    var transition = version.Transitions.Single(t => t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == request.ActionCode);
    var target = version.Nodes.Single(n => n.Code == transition.ToNodeCode);
    await CompleteOpenTasks(db, instance.Id, instance.CurrentNodeCode);
    db.Entry(instance).CurrentValues[nameof(WorkflowInstance.CurrentNodeCode)] = target.Code;
    db.Entry(instance).CurrentValues[nameof(WorkflowInstance.Status)] = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running;
    db.Entry(instance).CurrentValues[nameof(WorkflowInstance.CompletedAt)] = target.IsTerminal ? DateTimeOffset.UtcNow : null;
    db.WorkflowActions.Add(new WorkflowAction(instance.Id, request.ActionCode, transition.ActionName, WorkflowActionKind.Transition, transition.FromNodeCode, transition.ToNodeCode, request.Actor, DateTimeOffset.UtcNow));
    db.WorkflowHistories.Add(new WorkflowHistory(instance.Id, "ActionExecuted", target.Code, request.Actor, $"{transition.FromNodeCode} -> {transition.ToNodeCode}", DateTimeOffset.UtcNow));
    await CreateTaskIfNeeded(db, version, instance with { CurrentNodeCode = target.Code }, request.Actor);
    await ApplySupplierStatus(db, instance, target.Code);
    await db.SaveChangesAsync();
    return Results.Ok(instance with { CurrentNodeCode = target.Code, Status = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running });
}).WithTags("Workflow Engine");
app.MapPost("/api/workflow-tasks/{id:guid}/assign", async (Guid id, AssignTaskRequest request, EProcurementDbContext db) =>
{
    var task = await db.WorkflowTasks.SingleOrDefaultAsync(t => t.Id == id && t.Status == WorkflowTaskStatus.Open);
    if (task is null) return Results.NotFound();
    db.Entry(task).CurrentValues[nameof(WorkflowTask.AssignedTo)] = request.AssignedTo;
    db.Entry(task).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Assigned;
    db.Entry(task).CurrentValues[nameof(WorkflowTask.AssignedAt)] = DateTimeOffset.UtcNow;
    db.WorkflowHistories.Add(new WorkflowHistory(task.WorkflowInstanceId, "TaskAssigned", task.NodeCode, request.Actor, request.AssignedTo, DateTimeOffset.UtcNow, task.Id));
    await db.SaveChangesAsync();
    return Results.Ok(task with { AssignedTo = request.AssignedTo, Status = WorkflowTaskStatus.Assigned });
}).WithTags("Workflow Engine");
app.MapPost("/api/workflow-tasks/{id:guid}/complete", async (Guid id, CompleteTaskRequest request, EProcurementDbContext db) =>
{
    var task = await db.WorkflowTasks.SingleOrDefaultAsync(t => t.Id == id && (t.Status == WorkflowTaskStatus.Open || t.Status == WorkflowTaskStatus.Assigned));
    if (task is null) return Results.NotFound();
    db.Entry(task).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Completed;
    db.Entry(task).CurrentValues[nameof(WorkflowTask.CompletedAt)] = DateTimeOffset.UtcNow;
    db.WorkflowHistories.Add(new WorkflowHistory(task.WorkflowInstanceId, "TaskCompleted", task.NodeCode, request.Actor, "Task completed", DateTimeOffset.UtcNow, task.Id));
    await db.SaveChangesAsync();
    return Results.Ok(task with { Status = WorkflowTaskStatus.Completed });
}).WithTags("Workflow Engine");
app.MapPost("/api/workflow-instances/{id:guid}/cancel", async (Guid id, CancelWorkflowRequest request, EProcurementDbContext db) =>
{
    var instance = await db.WorkflowInstances.SingleOrDefaultAsync(i => i.Id == id && i.Status == WorkflowInstanceStatus.Running);
    if (instance is null) return Results.NotFound();
    db.Entry(instance).CurrentValues[nameof(WorkflowInstance.Status)] = WorkflowInstanceStatus.Cancelled;
    db.Entry(instance).CurrentValues[nameof(WorkflowInstance.CancelledAt)] = DateTimeOffset.UtcNow;
    foreach (var task in await db.WorkflowTasks.Where(t => t.WorkflowInstanceId == id && (t.Status == WorkflowTaskStatus.Open || t.Status == WorkflowTaskStatus.Assigned)).ToListAsync()) db.Entry(task).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Cancelled;
    db.WorkflowHistories.Add(new WorkflowHistory(id, "WorkflowCancelled", instance.CurrentNodeCode, request.Actor, request.Reason, DateTimeOffset.UtcNow));
    await db.SaveChangesAsync();
    return Results.Ok(instance with { Status = WorkflowInstanceStatus.Cancelled });
}).WithTags("Workflow Engine");

app.MapGet("/api/suppliers", async (EProcurementDbContext db) => await db.Suppliers.AsNoTracking().Include(s => s.Categories).OrderBy(s => s.ReferenceNumber).Select(s => new { s.ReferenceNumber, s.LegalName, Status = s.Status.ToString(), Categories = s.Categories.Select(c => c.Name) }).ToListAsync());
app.MapPost("/api/suppliers/{referenceNumber}/submit", async (string referenceNumber, EProcurementDbContext db) => await StartSupplierWorkflow(referenceNumber, db));
app.MapPost("/api/suppliers/{referenceNumber}/approve", async (string referenceNumber, EProcurementDbContext db) => await ExecuteSupplierAction(referenceNumber, "Approve", "approver@lca.org.ls", db));
app.MapGet("/api/audit-events", async (EProcurementDbContext db) => await db.AuditEvents.AsNoTracking().OrderByDescending(e => e.OccurredAt).Take(100).ToListAsync());
app.MapGet("/api/workflow-tasks", async (EProcurementDbContext db) => await db.WorkflowTasks.AsNoTracking().OrderBy(t => t.NodeCode).ToListAsync());
app.Run();

static Task<WorkflowDefinition?> LoadWorkflow(EProcurementDbContext db, string code) => db.WorkflowDefinitions.Include(w => w.Versions).ThenInclude(v => v.Nodes).Include(w => w.Versions).ThenInclude(v => v.Transitions).SingleOrDefaultAsync(w => w.Code == code && w.IsActive);
static async Task CreateTaskIfNeeded(EProcurementDbContext db, WorkflowVersion version, WorkflowInstance instance, string actor) { var node = version.Nodes.Single(n => n.Code == instance.CurrentNodeCode); if (!node.CreatesTask) return; var task = new WorkflowTask(instance.Id, node.Code, node.DefaultAssignedRole, CreatedAt: DateTimeOffset.UtcNow); db.WorkflowTasks.Add(task); db.WorkflowHistories.Add(new WorkflowHistory(instance.Id, "TaskCreated", node.Code, actor, node.DefaultAssignedRole ?? "Unassigned", DateTimeOffset.UtcNow, task.Id)); await Task.CompletedTask; }
static async Task CompleteOpenTasks(EProcurementDbContext db, Guid instanceId, string nodeCode) { foreach (var task in await db.WorkflowTasks.Where(t => t.WorkflowInstanceId == instanceId && t.NodeCode == nodeCode && (t.Status == WorkflowTaskStatus.Open || t.Status == WorkflowTaskStatus.Assigned)).ToListAsync()) { db.Entry(task).CurrentValues[nameof(WorkflowTask.Status)] = WorkflowTaskStatus.Completed; db.Entry(task).CurrentValues[nameof(WorkflowTask.CompletedAt)] = DateTimeOffset.UtcNow; } }
static async Task ApplySupplierStatus(EProcurementDbContext db, WorkflowInstance instance, string nodeCode) { if (instance.EntityType != nameof(Supplier)) return; var supplier = await db.Suppliers.SingleOrDefaultAsync(s => s.Id == instance.EntityId); if (supplier is null) return; var status = nodeCode switch { "DocumentCheck" or "Verification" or "Approval" => SupplierStatus.UnderVerification, "Approved" => SupplierStatus.Approved, "Rejected" => SupplierStatus.Rejected, _ => (SupplierStatus?)null }; if (status is not null) db.Entry(supplier).CurrentValues[nameof(Supplier.Status)] = status.Value; }
static async Task<IResult> StartSupplierWorkflow(string referenceNumber, EProcurementDbContext db) { var supplier = await db.Suppliers.SingleOrDefaultAsync(s => s.ReferenceNumber == referenceNumber); if (supplier is null) return Results.NotFound(); var wf = await LoadWorkflow(db, "SUPPLIER-ONBOARDING"); if (wf is null) return Results.BadRequest("Supplier onboarding workflow is not configured."); var version = wf.Versions.Single(v => v.Id == wf.PublishedVersionId || v.Status == WorkflowVersionStatus.Published); var start = version.Nodes.Single(n => n.IsStart); var instance = new WorkflowInstance(wf.Id, version.Id, nameof(Supplier), supplier.Id, start.Code, StartedAt: DateTimeOffset.UtcNow); db.WorkflowInstances.Add(instance); db.WorkflowHistories.Add(new WorkflowHistory(instance.Id, "WorkflowStarted", start.Code, "supplier@demo.co.ls", wf.Code, DateTimeOffset.UtcNow)); db.Entry(supplier).CurrentValues[nameof(Supplier.Status)] = SupplierStatus.Submitted; await db.SaveChangesAsync(); return Results.Ok(instance); }
static async Task<IResult> ExecuteSupplierAction(string referenceNumber, string actionCode, string actor, EProcurementDbContext db) { var supplier = await db.Suppliers.SingleOrDefaultAsync(s => s.ReferenceNumber == referenceNumber); if (supplier is null) return Results.NotFound(); var instance = await db.WorkflowInstances.OrderByDescending(i => i.StartedAt).FirstOrDefaultAsync(i => i.EntityType == nameof(Supplier) && i.EntityId == supplier.Id && i.Status == WorkflowInstanceStatus.Running); if (instance is null) return Results.BadRequest("Supplier does not have a running workflow instance."); var version = await db.WorkflowVersions.Include(v => v.Nodes).Include(v => v.Transitions).SingleAsync(v => v.Id == instance.WorkflowVersionId); var transition = version.Transitions.Single(t => t.FromNodeCode == instance.CurrentNodeCode && t.ActionCode == actionCode); var target = version.Nodes.Single(n => n.Code == transition.ToNodeCode); await CompleteOpenTasks(db, instance.Id, instance.CurrentNodeCode); db.Entry(instance).CurrentValues[nameof(WorkflowInstance.CurrentNodeCode)] = target.Code; db.Entry(instance).CurrentValues[nameof(WorkflowInstance.Status)] = target.IsTerminal ? WorkflowInstanceStatus.Completed : WorkflowInstanceStatus.Running; db.WorkflowActions.Add(new WorkflowAction(instance.Id, actionCode, transition.ActionName, WorkflowActionKind.Transition, transition.FromNodeCode, transition.ToNodeCode, actor, DateTimeOffset.UtcNow)); await ApplySupplierStatus(db, instance, target.Code); await db.SaveChangesAsync(); return Results.Ok(instance with { CurrentNodeCode = target.Code }); }

public sealed record CreateWorkflowRequest(string Code, string Name, string EntityType, List<WorkflowNodeRequest> Nodes, List<WorkflowTransitionRequest> Transitions);
public sealed record WorkflowNodeRequest(string Code, string Name, WorkflowNodeKind Kind, bool CreatesTask = false, string? DefaultAssignedRole = null, bool IsStart = false, bool IsTerminal = false);
public sealed record WorkflowTransitionRequest(string FromNodeCode, string ActionCode, string ActionName, string ToNodeCode, string? RequiredRuleCode = null);
public sealed record PublishWorkflowRequest(string Actor);
public sealed record CloneWorkflowRequest(string NewCode, string NewName);
public sealed record StartWorkflowRequest(string WorkflowCode, string EntityType, Guid EntityId, string Actor);
public sealed record ExecuteWorkflowActionRequest(string ActionCode, string Actor);
public sealed record AssignTaskRequest(string AssignedTo, string Actor);
public sealed record CompleteTaskRequest(string Actor);
public sealed record CancelWorkflowRequest(string Actor, string Reason);
