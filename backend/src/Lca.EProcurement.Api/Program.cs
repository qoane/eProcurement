using Lca.EProcurement.Api.Controllers;
using Lca.EProcurement.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lca.EProcurement.Application;
using Lca.EProcurement.EntityFrameworkCore;
using Lca.EProcurement.Api.Middleware;
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
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    foreach (var p in new[] { "RfpEvidence.View", "RfpEvidence.Manage", "RfpEvidence.Export", "Demo.View", "Demo.Manage", "Uat.View", "Uat.Manage", "Uat.Execute", "Training.View", "Training.Manage", "Implementation.View", "Implementation.Manage", "Handover.View", "Handover.Manage", "Readiness.View", "Readiness.Manage", "SupportMaintenance.View", "SupportMaintenance.Manage", "Studio.View", "Studio.Manage", "Studio.Publish", "Studio.Export", "Studio.Import", "Workflow.Configure", "Workflow.Publish", "Form.Configure", "Form.Publish", "Rule.Configure", "Rule.Publish", "Rule.Test", "ApprovalMatrix.Configure", "ApprovalMatrix.Publish", "DocumentRequirement.Configure", "DocumentRequirement.Publish", "Page.Configure", "Page.Publish", "Navigation.Configure", "Navigation.Publish", "Dashboard.Configure", "Dashboard.Publish", "Report.Configure", "Report.Publish", "Lookup.Configure", "Operations.View", "Operations.Manage", "Operations.Health.View", "Operations.Performance.View", "Operations.Backup.View", "Operations.Backup.Manage", "Operations.Configuration.Validate", "SupportCase.View", "SupportCase.Create", "SupportCase.Manage", "Notifications.View", "Notifications.Manage", "NotificationTemplates.Manage", "NotificationLogs.View", "NotificationPreferences.Manage", "NotificationEventMappings.Manage", "Communications.View", "Communications.Manage", "Communications.Internal", "DeadlineReminders.Manage", "Settings.View", "Settings.Manage", "Document.View", "Document.Upload", "Document.Download", "Document.Publish", "Document.Archive", "Document.Delete", "Document.ManageRetention", "Document.ViewAccessLog", "Document.ManageRequirements", "DataGovernance.View", "DataGovernance.Manage", "DataRetention.View", "DataRetention.Manage", "DataArchive.View", "DataArchive.Manage", "Migration.View", "Migration.Upload", "Migration.Validate", "Migration.Import", "Migration.Cancel", "DataQuality.View", "DataQuality.Manage", "DataQuality.Run", "DataExport.View", "DataExport.Request", "DataExport.Download", "DataExportSensitive", "PrivacyClassification.View", "PrivacyClassification.Manage", "Invoice.View", "Invoice.Create", "Invoice.Submit", "Invoice.Review", "Invoice.ApproveForPayment", "Invoice.Reject", "Invoice.Cancel", "InvoiceMatching.View", "InvoiceMatching.Run", "PurchaseOrderReturn.View", "PurchaseOrderReturn.Create", "PurchaseOrderReturn.Approve", "PurchaseOrderReturn.Reject", "PurchaseOrderReturn.Complete", "PurchaseOrderReturn.Cancel", "Payment.View", "Payment.Manage", "Payment.MarkPaid", "PurchaseOrder.GeneratePdf", "PurchaseOrder.SendToSupplier" })
        options.AddPolicy(p, policy => policy.RequireAssertion(ctx => ctx.User.HasClaim("permission", p) || ctx.User.IsInRole("Administrator")));
});
builder.Services.AddDbContext<EProcurementDbContext>(options => options.UseConfiguredProvider(builder.Configuration["Database:Provider"] ?? "SqlServer", builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IWorkflowApplicationService, WorkflowApplicationService>();
builder.Services.AddScoped<IBusinessRuleApplicationService, BusinessRuleApplicationService>();
builder.Services.AddScoped<ISupplierApplicationService, SupplierApplicationService>();
builder.Services.AddScoped<ISupplierPortalApplicationService, SupplierPortalApplicationService>();
builder.Services.AddScoped<IDynamicFormApplicationService, DynamicFormApplicationService>();
builder.Services.AddScoped<IAuditApplicationService, AuditApplicationService>();
builder.Services.AddScoped<IPlatformConfigurationApplicationService, PlatformConfigurationApplicationService>();
builder.Services.AddScoped<IConfigurationStudioApplicationService, ConfigurationStudioApplicationService>();
builder.Services.AddScoped<IConfigurationRuntimeResolver, ConfigurationRuntimeResolver>();
builder.Services.AddScoped<IMetadataApplicationService, MetadataApplicationService>();
builder.Services.AddScoped<INavigationApplicationService, NavigationApplicationService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISystemSettingsApplicationService, SystemSettingsApplicationService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<ISmsSender, HttpSmsSender>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
builder.Services.AddScoped<INotificationSender, NotificationApplicationService>();
builder.Services.AddScoped<IInAppNotificationService, NotificationApplicationService>();
builder.Services.AddScoped<INotificationTemplateRenderer, NotificationTemplateRenderer>();
builder.Services.AddScoped<INotificationRecipientResolver, NotificationRecipientResolver>();
builder.Services.AddScoped<ICommunicationApplicationService, CommunicationApplicationService>();
builder.Services.AddScoped<IDeadlineReminderService, DeadlineReminderService>();
builder.Services.AddScoped<IPageDefinitionApplicationService, PageDefinitionApplicationService>();
builder.Services.AddScoped<IEntityRecordApplicationService, EntityRecordApplicationService>();
builder.Services.AddScoped<IAnnualProcurementPlanApplicationService, AnnualProcurementPlanApplicationService>();
builder.Services.AddScoped<IBudgetApplicationService, BudgetApplicationService>();
builder.Services.AddScoped<ICostCentreApplicationService, CostCentreApplicationService>();
builder.Services.AddScoped<IProcurementCategoryApplicationService, ProcurementCategoryApplicationService>();
builder.Services.AddScoped<IFinancialYearApplicationService, FinancialYearApplicationService>();
builder.Services.AddScoped<IRequisitionApplicationService, RequisitionApplicationService>();
builder.Services.AddScoped<ITenderApplicationService, TenderApplicationService>();
builder.Services.AddScoped<IPublicTenderApplicationService, PublicTenderApplicationService>();
builder.Services.AddScoped<ISealedBidApplicationService, SealedBidApplicationService>();
builder.Services.AddScoped<ISealedBidAccessService>(sp => sp.GetRequiredService<ISealedBidApplicationService>());
builder.Services.AddScoped<IBidSubmissionApplicationService, BidSubmissionApplicationService>();
builder.Services.AddScoped<IBidOpeningApplicationService, BidOpeningApplicationService>();
builder.Services.AddScoped<IEvaluationApplicationService, EvaluationApplicationService>();
builder.Services.AddScoped<IAwardApplicationService, AwardApplicationService>();
builder.Services.AddScoped<IPurchaseOrderApplicationService, PurchaseOrderApplicationService>();
builder.Services.AddScoped<IContractApplicationService, ContractApplicationService>();
builder.Services.AddScoped<IPurchaseOrderReturnApplicationService, PurchaseOrderReturnApplicationService>();
builder.Services.AddScoped<IInvoiceMatchingApplicationService, InvoiceMatchingApplicationService>();
builder.Services.AddScoped<ISupplierInvoiceApplicationService, SupplierInvoiceApplicationService>();
builder.Services.AddScoped<IIntegrationApplicationService, IntegrationApplicationService>();
builder.Services.AddScoped<IProcurementCaseApplicationService, ProcurementCaseApplicationService>();
builder.Services.AddScoped<IReportingApplicationService, ReportingApplicationService>();
builder.Services.AddScoped<ILocalDocumentStorageProvider, LocalDocumentStorageProvider>();
builder.Services.AddScoped<IDocumentStorageProvider>(sp => sp.GetRequiredService<ILocalDocumentStorageProvider>());
builder.Services.AddScoped<IDocumentAccessService, DocumentAccessService>();
builder.Services.AddScoped<IDocumentVirusScanner, NoOpVirusScanner>();
builder.Services.AddScoped<IDocumentApplicationService, DocumentApplicationService>();
builder.Services.AddScoped<IDocumentRequirementValidationService, DocumentRequirementValidationService>();
builder.Services.AddScoped<IDocumentRetentionService, DocumentRetentionService>();
builder.Services.AddScoped<IDocumentIntegrationService, DocumentIntegrationService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ISecurityHardeningService, SecurityHardeningService>();
builder.Services.AddScoped<IDataGovernanceApplicationService, DataGovernanceApplicationService>();
builder.Services.AddScoped<IMigrationApplicationService, MigrationApplicationService>();
builder.Services.AddScoped<IDataArchivalService, DataArchivalService>();
builder.Services.AddScoped<IDataQualityApplicationService, DataQualityApplicationService>();
builder.Services.AddScoped<IDataExportApplicationService, DataExportApplicationService>();
builder.Services.AddScoped<IOperationsApplicationService, OperationsApplicationService>();
builder.Services.AddScoped<IConfigurationValidationService, ConfigurationValidationService>();
builder.Services.AddScoped<IRfpEvidenceApplicationService, RfpEvidenceApplicationService>();
builder.Services.AddScoped<IDemoDataVerificationService, DemoDataVerificationService>();
builder.Services.AddScoped<IReadinessAssessmentService, ReadinessAssessmentService>();

var app = builder.Build();

async Task EnsureDatabaseSchemaAsync(bool seed)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EProcurementDbContext>();
    await db.RepairMissingRfpEvidenceMigrationAsync();
    await db.Database.MigrateAsync();
    if (await db.RepairMissingRfpEvidenceMigrationAsync())
    {
        await db.Database.MigrateAsync();
    }
    await db.EnsureConfigurablePlatformSchemaAsync();
    await db.EnsureIntegrationSchemaAsync();
    await db.EnsureRfpEvidenceSchemaAsync();
    await db.EnsureSealedBidSecuritySchemaAsync();
    await db.EnsureBidOpeningSchemaAsync();
    await db.EnsureEvaluationManagementSchemaAsync();
    await db.EnsurePurchaseOrderManagementSchemaAsync();
    await db.EnsureContractManagementSchemaAsync();
    await db.EnsureOperationalReadinessSchemaAsync();
    await db.EnsurePublicTenderPublicationSchemaAsync();
    await db.EnsureProcurementCaseTraceSchemaAsync();
    await db.EnsureDataGovernanceSchemaAsync();
    await db.EnsureIdentitySecuritySchemaAsync();
    if (seed) await SeedData.SeedAsync(db);
}

if (args.Contains("--seed"))
{
    await EnsureDatabaseSchemaAsync(seed: true);
    return;
}

if (builder.Configuration.GetValue("Database:ApplySchemaOnStartup", true))
{
    await EnsureDatabaseSchemaAsync(seed: app.Environment.IsDevelopment());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors(FrontendCors);
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EProcurementDbContext>();
        db.AuditEvents.Add(new Lca.EProcurement.Domain.AuditEvent("Access denied", context.Request.Path.Value ?? "HTTP", Guid.Empty, context.Request.Method, context.User.Identity?.Name ?? "anonymous", $"{context.Response.StatusCode} for {context.Request.Path}", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync();
    }
});
app.MapGet("/", () => Results.Ok(new { name = "LCA eProcurement API", status = "running", documentation = "/swagger", health = "/health" })).AllowAnonymous();
app.MapControllers();
app.Run();
