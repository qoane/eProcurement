using Lca.EProcurement.Api.Controllers;
using Lca.EProcurement.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lca.EProcurement.Application;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCors = "ViteFrontend";
builder.Services.AddCors(options => options.AddPolicy(FrontendCors, policy => policy.WithOrigins("http://localhost:5173", "https://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true, ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)) }; });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<EProcurementDbContext>(options => options.UseConfiguredProvider(builder.Configuration["Database:Provider"] ?? "SqlServer", builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IWorkflowApplicationService, WorkflowApplicationService>();
builder.Services.AddScoped<IBusinessRuleApplicationService, BusinessRuleApplicationService>();
builder.Services.AddScoped<ISupplierApplicationService, SupplierApplicationService>();
builder.Services.AddScoped<IDynamicFormApplicationService, DynamicFormApplicationService>();
builder.Services.AddScoped<IAuditApplicationService, AuditApplicationService>();
builder.Services.AddScoped<IPlatformConfigurationApplicationService, PlatformConfigurationApplicationService>();
builder.Services.AddScoped<IConfigurationStudioApplicationService, ConfigurationStudioApplicationService>();
builder.Services.AddScoped<IMetadataApplicationService, MetadataApplicationService>();
builder.Services.AddScoped<INavigationApplicationService, NavigationApplicationService>();
builder.Services.AddScoped<IPageDefinitionApplicationService, PageDefinitionApplicationService>();
builder.Services.AddScoped<IEntityRecordApplicationService, EntityRecordApplicationService>();
builder.Services.AddScoped<IAnnualProcurementPlanApplicationService, AnnualProcurementPlanApplicationService>();
builder.Services.AddScoped<IBudgetApplicationService, BudgetApplicationService>();
builder.Services.AddScoped<ICostCentreApplicationService, CostCentreApplicationService>();
builder.Services.AddScoped<IProcurementCategoryApplicationService, ProcurementCategoryApplicationService>();
builder.Services.AddScoped<IFinancialYearApplicationService, FinancialYearApplicationService>();
builder.Services.AddScoped<IRequisitionApplicationService, RequisitionApplicationService>();
builder.Services.AddScoped<ITenderApplicationService, TenderApplicationService>();
builder.Services.AddScoped<IBidSubmissionApplicationService, BidSubmissionApplicationService>();
builder.Services.AddScoped<IBidOpeningApplicationService, BidOpeningApplicationService>();
builder.Services.AddScoped<IEvaluationApplicationService, EvaluationApplicationService>();
builder.Services.AddScoped<IAwardApplicationService, AwardApplicationService>();
builder.Services.AddScoped<IPurchaseOrderApplicationService, PurchaseOrderApplicationService>();
builder.Services.AddScoped<IContractApplicationService, ContractApplicationService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

var app = builder.Build();

async Task EnsureDatabaseSchemaAsync(bool seed)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EProcurementDbContext>();
    await db.Database.MigrateAsync();
    await db.EnsureConfigurablePlatformSchemaAsync();
    if (seed) await SeedData.SeedAsync(db);
}

if (args.Contains("--seed"))
{
    await EnsureDatabaseSchemaAsync(seed: true);
    return;
}

await EnsureDatabaseSchemaAsync(seed: app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(FrontendCors);
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Ok(new { name = "LCA eProcurement API", status = "running", documentation = "/swagger", health = "/health" }));
app.MapGet("/health", async (EProcurementDbContext db) => Results.Ok(new { status = "healthy", platform = "LCA eProcurement", provider = db.Database.ProviderName, database = db.Database.GetDbConnection().Database }));
app.MapControllers();
app.Run();
