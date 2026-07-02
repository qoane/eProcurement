using Lca.EProcurement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.EntityFrameworkCore;

public sealed class EProcurementDbContext(DbContextOptions<EProcurementDbContext> options) : DbContext(options)
{
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
}

public static class DatabaseProviderConfiguration
{
    public const string DemoSqlServerConnectionString = "Server=localhost;Database=LcaEProcurement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    public static DbContextOptionsBuilder<EProcurementDbContext> UseConfiguredProvider(this DbContextOptionsBuilder<EProcurementDbContext> builder, string provider, string? connectionString = null)
        => provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase)
            ? builder.UseSqlite(connectionString ?? "DataSource=:memory:")
            : builder.UseSqlServer(connectionString ?? DemoSqlServerConnectionString);
}
