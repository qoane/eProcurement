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
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabasePreparation");
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
    logger.LogInformation("Database ready. Provider: {Provider}; Database: {Database}", db.Database.ProviderName, db.Database.GetDbConnection().Database);
}

app.UseCors(FrontendCors);
app.MapGet("/", () => Results.Ok(new { name = "LCA eProcurement API", status = "running", documentation = "/swagger", health = "/health" }));
app.MapGet("/health", async (EProcurementDbContext db) => Results.Ok(new { status = "healthy", platform = "LCA eProcurement", provider = db.Database.ProviderName, database = db.Database.GetDbConnection().Database }));
app.MapGet("/api/suppliers", async (EProcurementDbContext db) => await db.Suppliers.AsNoTracking().Include(s => s.Categories).OrderBy(s => s.ReferenceNumber).Select(s => new { s.ReferenceNumber, s.LegalName, Status = s.Status.ToString(), Categories = s.Categories.Select(c => c.Name) }).ToListAsync());
app.MapGet("/api/suppliers/{referenceNumber}", async (string referenceNumber, EProcurementDbContext db) => await db.Suppliers.AsNoTracking().Include(s => s.Categories).Include(s => s.Documents).Where(s => s.ReferenceNumber == referenceNumber).Select(s => new { s.ReferenceNumber, s.LegalName, Status = s.Status.ToString(), Categories = s.Categories.Select(c => c.Name), Documents = s.Documents.Select(d => new { d.DocumentType, d.FileName, d.UploadedAt }) }).SingleOrDefaultAsync() is { } supplier ? Results.Ok(supplier) : Results.NotFound());
app.MapPost("/api/suppliers/{referenceNumber}/submit", async (string referenceNumber, EProcurementDbContext db) => await SetSupplierStatus(referenceNumber, SupplierStatus.Submitted, "Supplier submitted", "supplier@demo.co.ls", db));
app.MapPost("/api/suppliers/{referenceNumber}/approve", async (string referenceNumber, EProcurementDbContext db) => await SetSupplierStatus(referenceNumber, SupplierStatus.Approved, "Supplier approved", "approver@lca.org.ls", db));
app.MapGet("/api/audit-events", async (EProcurementDbContext db) => await db.AuditEvents.AsNoTracking().OrderByDescending(e => e.OccurredAt).Take(100).Select(e => new { e.EventType, e.EntityType, e.EntityReference, e.Actor, e.Details, e.OccurredAt }).ToListAsync());
app.MapGet("/api/workflow-tasks", async (EProcurementDbContext db) => await db.WorkflowTasks.AsNoTracking().OrderBy(t => t.StepCode).Select(t => new { t.Id, t.StepCode, t.AssignedRole, Status = t.Status.ToString() }).ToListAsync());

app.Run();

static async Task<IResult> SetSupplierStatus(string referenceNumber, SupplierStatus status, string eventType, string actor, EProcurementDbContext db)
{
    var supplier = await db.Suppliers.SingleOrDefaultAsync(s => s.ReferenceNumber == referenceNumber);
    if (supplier is null) return Results.NotFound();
    db.Entry(supplier).CurrentValues[nameof(Supplier.Status)] = status;
    db.AuditEvents.Add(new AuditEvent(eventType, nameof(Supplier), supplier.Id, supplier.ReferenceNumber, actor, $"Supplier status changed to {status}", DateTimeOffset.UtcNow));
    await db.SaveChangesAsync();
    return Results.Ok(new { supplier.ReferenceNumber, supplier.LegalName, Status = status.ToString() });
}
