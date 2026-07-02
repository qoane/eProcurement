using Lca.EProcurement.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EProcurementDbContext>(options => options.UseConfiguredProvider(builder.Configuration["Database:Provider"] ?? "SqlServer", builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", platform = "LCA eProcurement" }));
app.MapGet("/api/suppliers", () => new[] { new { referenceNumber = "SUP-LCA-2026-0001", legalName = "Maseru ICT Supplies Pty Ltd", status = "UnderVerification" } });
app.Run();
