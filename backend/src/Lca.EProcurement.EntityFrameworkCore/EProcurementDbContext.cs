using Lca.EProcurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Lca.EProcurement.EntityFrameworkCore;

public sealed class EProcurementDbContext(DbContextOptions<EProcurementDbContext> options) : DbContext(options)
{
    public DbSet<SeedMetadata> SeedMetadata => Set<SeedMetadata>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierDocument> SupplierDocuments => Set<SupplierDocument>();
    public DbSet<SupplierCategory> SupplierCategories => Set<SupplierCategory>();
    public DbSet<SupplierPerformanceRating> SupplierPerformanceRatings => Set<SupplierPerformanceRating>();
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowVersion> WorkflowVersions => Set<WorkflowVersion>();
    public DbSet<WorkflowNode> WorkflowNodes => Set<WorkflowNode>();
    public DbSet<WorkflowTransition> WorkflowTransitions => Set<WorkflowTransition>();
    public DbSet<WorkflowTransitionEffect> WorkflowTransitionEffects => Set<WorkflowTransitionEffect>();
    public DbSet<WorkflowMapping> WorkflowMappings => Set<WorkflowMapping>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowTask> WorkflowTasks => Set<WorkflowTask>();
    public DbSet<WorkflowAction> WorkflowActions => Set<WorkflowAction>();
    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();
    public DbSet<BusinessRuleDefinition> BusinessRuleDefinitions => Set<BusinessRuleDefinition>();
    public DbSet<BusinessRuleExecutionLog> BusinessRuleExecutionLogs => Set<BusinessRuleExecutionLog>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<FormDefinition> FormDefinitions => Set<FormDefinition>();
    public DbSet<FormVersion> FormVersions => Set<FormVersion>();
    public DbSet<FormSection> FormSections => Set<FormSection>();
    public DbSet<FormField> FormFields => Set<FormField>();
    public DbSet<FormFieldValidation> FormFieldValidations => Set<FormFieldValidation>();
    public DbSet<FormFieldVisibilityRule> FormFieldVisibilityRules => Set<FormFieldVisibilityRule>();
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
    public DbSet<FormSubmissionValue> FormSubmissionValues => Set<FormSubmissionValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeedMetadata>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.Kind, x.Code }).IsUnique(); b.Property(x => x.Kind).HasMaxLength(64); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); });
        modelBuilder.Entity<Supplier>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.ReferenceNumber).IsUnique(); b.Property(x => x.ReferenceNumber).HasMaxLength(64); b.Property(x => x.LegalName).HasMaxLength(256); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); b.HasMany(x => x.Documents).WithOne().HasForeignKey(x => x.SupplierId); b.HasMany(x => x.PerformanceRatings).WithOne().HasForeignKey(x => x.SupplierId); b.HasMany(x => x.Categories).WithMany().UsingEntity("SupplierCategoryAssignments"); });
        modelBuilder.Entity<SupplierDocument>(b => { b.HasKey(x => x.Id); b.Property(x => x.DocumentType).HasMaxLength(128); b.Property(x => x.FileName).HasMaxLength(256); b.Property(x => x.UploadedBy).HasMaxLength(256); });
        modelBuilder.Entity<SupplierCategory>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Name).IsUnique(); b.Property(x => x.Name).HasMaxLength(128); });
        modelBuilder.Entity<SupplierPerformanceRating>(b => { b.HasKey(x => x.Id); b.Property(x => x.Notes).HasMaxLength(1000); });
        modelBuilder.Entity<WorkflowDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.EntityType).HasMaxLength(128); b.HasMany(x => x.Versions).WithOne().HasForeignKey(x => x.WorkflowDefinitionId); });
        modelBuilder.Entity<WorkflowVersion>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.WorkflowDefinitionId, x.VersionNumber }).IsUnique(); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); b.HasMany(x => x.Nodes).WithOne().HasForeignKey(x => x.WorkflowVersionId); b.HasMany(x => x.Transitions).WithOne().HasForeignKey(x => x.WorkflowVersionId); });
        modelBuilder.Entity<WorkflowNode>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.WorkflowVersionId, x.Code }).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.Kind).HasConversion<string>().HasMaxLength(64); b.Property(x => x.DefaultAssignedRole).HasMaxLength(128); });
        modelBuilder.Entity<WorkflowTransition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.WorkflowVersionId, x.FromNodeCode, x.ActionCode }).IsUnique(); b.Property(x => x.FromNodeCode).HasMaxLength(128); b.Property(x => x.ActionCode).HasMaxLength(128); b.Property(x => x.ActionName).HasMaxLength(256); b.Property(x => x.ToNodeCode).HasMaxLength(128); b.Property(x => x.RequiredRuleCode).HasMaxLength(128); });
        modelBuilder.Entity<WorkflowTransitionEffect>(b => { b.HasKey(x => x.Id); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.PropertyName).HasMaxLength(128); b.Property(x => x.ValueExpression).HasMaxLength(512); b.HasIndex(x => x.TriggerTransitionId); });
        modelBuilder.Entity<WorkflowMapping>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.EntityType, x.ActionCode }).IsUnique(); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.ActionCode).HasMaxLength(128); b.Property(x => x.WorkflowCode).HasMaxLength(128); });
        modelBuilder.Entity<WorkflowInstance>(b => { b.HasKey(x => x.Id); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.CurrentNodeCode).HasMaxLength(128); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<WorkflowTask>(b => { b.HasKey(x => x.Id); b.Property(x => x.NodeCode).HasMaxLength(128); b.Property(x => x.AssignedRole).HasMaxLength(128); b.Property(x => x.AssignedTo).HasMaxLength(256); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<WorkflowAction>(b => { b.HasKey(x => x.Id); b.Property(x => x.ActionCode).HasMaxLength(128); b.Property(x => x.ActionName).HasMaxLength(256); b.Property(x => x.Kind).HasConversion<string>().HasMaxLength(64); b.Property(x => x.FromNodeCode).HasMaxLength(128); b.Property(x => x.ToNodeCode).HasMaxLength(128); b.Property(x => x.Actor).HasMaxLength(256); });
        modelBuilder.Entity<WorkflowHistory>(b => { b.HasKey(x => x.Id); b.Property(x => x.EventType).HasMaxLength(128); b.Property(x => x.NodeCode).HasMaxLength(128); b.Property(x => x.Actor).HasMaxLength(256); b.Property(x => x.Details).HasMaxLength(2000); });
        modelBuilder.Entity<BusinessRuleDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.AppliesTo).HasMaxLength(128); b.Property(x => x.Expression).HasMaxLength(512); });
        modelBuilder.Entity<BusinessRuleExecutionLog>(b => { b.HasKey(x => x.Id); b.Property(x => x.RuleCode).HasMaxLength(128); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.Outcome).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<AuditEvent>(b => { b.HasKey(x => x.Id); b.Property(x => x.EventType).HasMaxLength(128); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.EntityReference).HasMaxLength(128); b.Property(x => x.Actor).HasMaxLength(256); });
        modelBuilder.Entity<FormDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.EntityType).HasMaxLength(128); b.HasMany(x => x.Versions).WithOne().HasForeignKey(x => x.FormDefinitionId); });
        modelBuilder.Entity<FormVersion>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.FormDefinitionId, x.VersionNumber }).IsUnique(); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); b.HasMany(x => x.Sections).WithOne().HasForeignKey(x => x.FormVersionId); });
        modelBuilder.Entity<FormSection>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Title).HasMaxLength(256); b.HasMany(x => x.Fields).WithOne().HasForeignKey(x => x.FormSectionId); });
        modelBuilder.Entity<FormField>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Label).HasMaxLength(256); b.Property(x => x.FieldType).HasMaxLength(64); b.HasMany(x => x.Validations).WithOne().HasForeignKey(x => x.FormFieldId); b.HasMany(x => x.VisibilityRules).WithOne().HasForeignKey(x => x.FormFieldId); });
        modelBuilder.Entity<FormFieldValidation>(b => { b.HasKey(x => x.Id); b.Property(x => x.ValidationType).HasMaxLength(128); b.Property(x => x.Message).HasMaxLength(512); });
        modelBuilder.Entity<FormFieldVisibilityRule>(b => { b.HasKey(x => x.Id); b.Property(x => x.Expression).HasMaxLength(512); });
        modelBuilder.Entity<FormSubmission>(b => { b.HasKey(x => x.Id); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.SubmittedBy).HasMaxLength(256); b.HasMany(x => x.Values).WithOne().HasForeignKey(x => x.FormSubmissionId); });
        modelBuilder.Entity<FormSubmissionValue>(b => { b.HasKey(x => x.Id); b.Property(x => x.FieldCode).HasMaxLength(128); });
    }
}

public static class DatabaseProviderConfiguration
{
    public const string DemoSqlServerConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LcaEProcurement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    public static DbContextOptionsBuilder UseConfiguredProvider(this DbContextOptionsBuilder builder, string provider, string? connectionString = null)
        => provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
            ? builder.UseSqlServer(connectionString ?? DemoSqlServerConnectionString)
            : throw new NotSupportedException($"Database provider '{provider}' is not configured. SQL Server is the application database provider.");

    public static DbContextOptionsBuilder<TContext> UseConfiguredProvider<TContext>(this DbContextOptionsBuilder<TContext> builder, string provider, string? connectionString = null)
        where TContext : DbContext
        => provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
            ? builder.UseSqlServer(connectionString ?? DemoSqlServerConnectionString)
            : throw new NotSupportedException($"Database provider '{provider}' is not configured. SQL Server is the application database provider.");
}

public sealed class EProcurementDbContextFactory : IDesignTimeDbContextFactory<EProcurementDbContext>
{
    public EProcurementDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "src", "Lca.EProcurement.Api"));
        if (!Directory.Exists(basePath)) basePath = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("appsettings.json", optional: true).AddJsonFile("appsettings.Development.json", optional: true).AddEnvironmentVariables().Build();
        var options = new DbContextOptionsBuilder<EProcurementDbContext>().UseConfiguredProvider(configuration["Database:Provider"] ?? "SqlServer", configuration.GetConnectionString("Default"));
        return new EProcurementDbContext(options.Options);
    }
}
