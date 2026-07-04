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
    public DbSet<WorkflowStepDefinition> WorkflowStepDefinitions => Set<WorkflowStepDefinition>();
    public DbSet<WorkflowTransition> WorkflowTransitions => Set<WorkflowTransition>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowTask> WorkflowTasks => Set<WorkflowTask>();
    public DbSet<WorkflowAction> WorkflowActions => Set<WorkflowAction>();
    public DbSet<BusinessRuleDefinition> BusinessRuleDefinitions => Set<BusinessRuleDefinition>();
    public DbSet<BusinessRuleExecutionLog> BusinessRuleExecutionLogs => Set<BusinessRuleExecutionLog>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeedMetadata>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.Kind, x.Code }).IsUnique(); b.Property(x => x.Kind).HasMaxLength(64); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); });
        modelBuilder.Entity<Supplier>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.ReferenceNumber).IsUnique(); b.Property(x => x.ReferenceNumber).HasMaxLength(64); b.Property(x => x.LegalName).HasMaxLength(256); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); b.HasMany(x => x.Documents).WithOne().HasForeignKey(x => x.SupplierId); b.HasMany(x => x.PerformanceRatings).WithOne().HasForeignKey(x => x.SupplierId); b.HasMany(x => x.Categories).WithMany().UsingEntity("SupplierCategoryAssignments"); });
        modelBuilder.Entity<SupplierDocument>(b => { b.HasKey(x => x.Id); b.Property(x => x.DocumentType).HasMaxLength(128); b.Property(x => x.FileName).HasMaxLength(256); b.Property(x => x.UploadedBy).HasMaxLength(256); });
        modelBuilder.Entity<SupplierCategory>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Name).IsUnique(); b.Property(x => x.Name).HasMaxLength(128); });
        modelBuilder.Entity<SupplierPerformanceRating>(b => { b.HasKey(x => x.Id); b.Property(x => x.Notes).HasMaxLength(1000); });
        modelBuilder.Entity<WorkflowDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.WorkflowDefinitionId); b.HasMany(x => x.Transitions).WithOne().HasForeignKey(x => x.WorkflowDefinitionId); });
        modelBuilder.Entity<WorkflowStepDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.WorkflowDefinitionId, x.Code }).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); });
        modelBuilder.Entity<WorkflowTransition>(b => { b.HasKey(x => x.Id); b.Property(x => x.FromStepCode).HasMaxLength(128); b.Property(x => x.Action).HasMaxLength(128); b.Property(x => x.ToStepCode).HasMaxLength(128); b.Property(x => x.RequiredRuleCode).HasMaxLength(128); });
        modelBuilder.Entity<WorkflowInstance>(b => { b.HasKey(x => x.Id); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.CurrentStepCode).HasMaxLength(128); });
        modelBuilder.Entity<WorkflowTask>(b => { b.HasKey(x => x.Id); b.Property(x => x.StepCode).HasMaxLength(128); b.Property(x => x.AssignedRole).HasMaxLength(128); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<WorkflowAction>(b => { b.HasKey(x => x.Id); b.Property(x => x.Action).HasMaxLength(128); b.Property(x => x.FromStepCode).HasMaxLength(128); b.Property(x => x.ToStepCode).HasMaxLength(128); b.Property(x => x.Actor).HasMaxLength(256); });
        modelBuilder.Entity<BusinessRuleDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.AppliesTo).HasMaxLength(128); b.Property(x => x.Expression).HasMaxLength(512); });
        modelBuilder.Entity<BusinessRuleExecutionLog>(b => { b.HasKey(x => x.Id); b.Property(x => x.RuleCode).HasMaxLength(128); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.Outcome).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<AuditEvent>(b => { b.HasKey(x => x.Id); b.Property(x => x.EventType).HasMaxLength(128); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.EntityReference).HasMaxLength(128); b.Property(x => x.Actor).HasMaxLength(256); });
    }
}

public static class DatabaseProviderConfiguration
{
    public const string DemoSqlServerConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LcaEProcurement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    public static DbContextOptionsBuilder UseConfiguredProvider(this DbContextOptionsBuilder builder, string provider, string? connectionString = null)
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
