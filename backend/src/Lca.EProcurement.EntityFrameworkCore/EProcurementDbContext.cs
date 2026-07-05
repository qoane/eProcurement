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
    public DbSet<DocumentTypeRequirement> DocumentTypeRequirements => Set<DocumentTypeRequirement>();
    public DbSet<LookupValue> LookupValues => Set<LookupValue>();
    public DbSet<BusinessProcessDefinition> BusinessProcessDefinitions => Set<BusinessProcessDefinition>();
    public DbSet<DocumentRequirementSet> DocumentRequirementSets => Set<DocumentRequirementSet>();
    public DbSet<DocumentRequirement> DocumentRequirements => Set<DocumentRequirement>();
    public DbSet<ApprovalMatrix> ApprovalMatrices => Set<ApprovalMatrix>();
    public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
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
        modelBuilder.Entity<DocumentTypeRequirement>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.EntityType, x.DocumentType }).IsUnique(); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.DocumentType).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); });
        modelBuilder.Entity<LookupValue>(b => { b.HasKey(x => x.Id); b.HasIndex(x => new { x.LookupType, x.Code }).IsUnique(); b.Property(x => x.LookupType).HasMaxLength(128); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); });

        modelBuilder.Entity<BusinessProcessDefinition>(b => { b.HasKey(x => x.Id); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(128); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.EntityType).HasMaxLength(128); b.Property(x => x.Status).HasConversion<string>().HasMaxLength(64); });
        modelBuilder.Entity<DocumentRequirementSet>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.EntityType).HasMaxLength(128); b.HasMany(x => x.Requirements).WithOne().HasForeignKey(x => x.DocumentRequirementSetId); });
        modelBuilder.Entity<DocumentRequirement>(b => { b.HasKey(x => x.Id); b.Property(x => x.DocumentType).HasMaxLength(128); b.Property(x => x.AllowedExtensions).HasMaxLength(256); b.Property(x => x.RuleCode).HasMaxLength(128); });
        modelBuilder.Entity<ApprovalMatrix>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(256); b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.EntityType).HasMaxLength(128); b.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.ApprovalMatrixId); });
        modelBuilder.Entity<ApprovalStep>(b => { b.HasKey(x => x.Id); b.Property(x => x.Role).HasMaxLength(128); b.Property(x => x.MinimumAmount).HasPrecision(18,2); b.Property(x => x.MaximumAmount).HasPrecision(18,2); b.Property(x => x.RuleCode).HasMaxLength(128); });
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
    public static Task EnsureConfigurablePlatformSchemaAsync(this EProcurementDbContext db, CancellationToken cancellationToken = default)
        => db.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID(N'[dbo].[BusinessProcessDefinitions]', N'U') IS NULL CREATE TABLE [dbo].[BusinessProcessDefinitions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BusinessProcessDefinitions] PRIMARY KEY,[Code] nvarchar(128) NOT NULL CONSTRAINT [UX_BusinessProcessDefinitions_Code] UNIQUE,[Name] nvarchar(256) NOT NULL,[Description] nvarchar(1000) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[ActiveWorkflowDefinitionId] uniqueidentifier NULL,[ActiveFormDefinitionId] uniqueidentifier NULL,[ActiveDocumentRequirementSetId] uniqueidentifier NULL,[ActiveApprovalMatrixId] uniqueidentifier NULL,[Status] nvarchar(64) NOT NULL);
IF OBJECT_ID(N'[dbo].[DocumentRequirementSets]', N'U') IS NULL CREATE TABLE [dbo].[DocumentRequirementSets]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DocumentRequirementSets] PRIMARY KEY,[Name] nvarchar(256) NOT NULL,[Description] nvarchar(1000) NOT NULL,[EntityType] nvarchar(128) NOT NULL);
IF OBJECT_ID(N'[dbo].[DocumentRequirements]', N'U') IS NULL CREATE TABLE [dbo].[DocumentRequirements]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DocumentRequirements] PRIMARY KEY,[DocumentRequirementSetId] uniqueidentifier NOT NULL,[DocumentType] nvarchar(128) NOT NULL,[Required] bit NOT NULL,[MinimumFiles] int NOT NULL,[MaximumFiles] int NOT NULL,[AllowedExtensions] nvarchar(256) NOT NULL,[MaximumFileSize] bigint NOT NULL,[RuleCode] nvarchar(128) NULL,CONSTRAINT [FK_DocumentRequirements_Sets] FOREIGN KEY([DocumentRequirementSetId]) REFERENCES [dbo].[DocumentRequirementSets]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[ApprovalMatrices]', N'U') IS NULL CREATE TABLE [dbo].[ApprovalMatrices]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_ApprovalMatrices] PRIMARY KEY,[Name] nvarchar(256) NOT NULL,[Description] nvarchar(1000) NOT NULL,[EntityType] nvarchar(128) NOT NULL);
IF OBJECT_ID(N'[dbo].[ApprovalSteps]', N'U') IS NULL CREATE TABLE [dbo].[ApprovalSteps]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_ApprovalSteps] PRIMARY KEY,[ApprovalMatrixId] uniqueidentifier NOT NULL,[Role] nvarchar(128) NOT NULL,[Sequence] int NOT NULL,[MinimumAmount] decimal(18,2) NULL,[MaximumAmount] decimal(18,2) NULL,[RuleCode] nvarchar(128) NULL,CONSTRAINT [FK_ApprovalSteps_Matrices] FOREIGN KEY([ApprovalMatrixId]) REFERENCES [dbo].[ApprovalMatrices]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[WorkflowDefinitions]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowDefinitions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowDefinitions] PRIMARY KEY,[Code] nvarchar(128) NOT NULL CONSTRAINT [UX_WorkflowDefinitions_Code] UNIQUE,[Name] nvarchar(256) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,[PublishedVersionId] uniqueidentifier NULL);
IF OBJECT_ID(N'[dbo].[WorkflowVersions]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowVersions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowVersions] PRIMARY KEY,[WorkflowDefinitionId] uniqueidentifier NOT NULL,[VersionNumber] int NOT NULL,[Status] nvarchar(64) NOT NULL,[PublishedAt] datetimeoffset NULL,[PublishedBy] nvarchar(max) NULL,CONSTRAINT [FK_WorkflowVersions_WorkflowDefinitions] FOREIGN KEY([WorkflowDefinitionId]) REFERENCES [dbo].[WorkflowDefinitions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_WorkflowVersions_Definition_Version] UNIQUE([WorkflowDefinitionId],[VersionNumber]));
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowNodes]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowNodes] PRIMARY KEY,[WorkflowVersionId] uniqueidentifier NOT NULL,[Code] nvarchar(128) NOT NULL,[Name] nvarchar(256) NOT NULL,[Kind] nvarchar(64) NOT NULL,[CreatesTask] bit NOT NULL,[DefaultAssignedRole] nvarchar(128) NULL,[IsStart] bit NOT NULL,[IsTerminal] bit NOT NULL,CONSTRAINT [FK_WorkflowNodes_WorkflowVersions] FOREIGN KEY([WorkflowVersionId]) REFERENCES [dbo].[WorkflowVersions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_WorkflowNodes_Version_Code] UNIQUE([WorkflowVersionId],[Code]));
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowTransitions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowTransitions] PRIMARY KEY,[WorkflowVersionId] uniqueidentifier NOT NULL,[FromNodeCode] nvarchar(128) NOT NULL,[ActionCode] nvarchar(128) NOT NULL,[ActionName] nvarchar(256) NOT NULL,[ToNodeCode] nvarchar(128) NOT NULL,[RequiredRuleCode] nvarchar(128) NULL,CONSTRAINT [FK_WorkflowTransitions_WorkflowVersions] FOREIGN KEY([WorkflowVersionId]) REFERENCES [dbo].[WorkflowVersions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_WorkflowTransitions_Version_From_Action] UNIQUE([WorkflowVersionId],[FromNodeCode],[ActionCode]));
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'WorkflowVersionId') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [WorkflowVersionId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowTransitions_WorkflowVersionId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'FromNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [FromNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_FromNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ActionCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ActionCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ActionCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ActionName') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ActionName] nvarchar(256) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ActionName] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ToNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ToNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ToNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'RequiredRuleCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [RequiredRuleCode] nvarchar(128) NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'FromStepCode') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'FromStepCode', 'ColumnId')) ALTER TABLE [dbo].[WorkflowTransitions] ADD CONSTRAINT [DF_WorkflowTransitions_FromStepCode] DEFAULT N'' FOR [FromStepCode];
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ToStepCode') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'ToStepCode', 'ColumnId')) ALTER TABLE [dbo].[WorkflowTransitions] ADD CONSTRAINT [DF_WorkflowTransitions_ToStepCode] DEFAULT N'' FOR [ToStepCode];
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'Action') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'Action', 'ColumnId')) ALTER TABLE [dbo].[WorkflowTransitions] ADD CONSTRAINT [DF_WorkflowTransitions_Action] DEFAULT N'' FOR [Action];
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'WorkflowDefinitionId') IS NOT NULL
BEGIN
    DECLARE @DropWorkflowTransitionDefinitionSql nvarchar(max) = N'';
    SELECT @DropWorkflowTransitionDefinitionSql = @DropWorkflowTransitionDefinitionSql + N'ALTER TABLE [dbo].[WorkflowTransitions] DROP CONSTRAINT [' + fk.name + N'];'
    FROM sys.foreign_keys fk
    WHERE fk.parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]')
      AND EXISTS (
          SELECT 1
          FROM sys.foreign_key_columns fkc
          WHERE fkc.constraint_object_id = fk.object_id
            AND fkc.parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'WorkflowDefinitionId', 'ColumnId'));
    SELECT @DropWorkflowTransitionDefinitionSql = @DropWorkflowTransitionDefinitionSql + N'ALTER TABLE [dbo].[WorkflowTransitions] DROP CONSTRAINT [' + dc.name + N'];'
    FROM sys.default_constraints dc
    WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]')
      AND dc.parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'WorkflowDefinitionId', 'ColumnId');
    SELECT @DropWorkflowTransitionDefinitionSql = @DropWorkflowTransitionDefinitionSql + N'DROP INDEX [' + i.name + N'] ON [dbo].[WorkflowTransitions];'
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic
        ON ic.object_id = i.object_id
       AND ic.index_id = i.index_id
    WHERE i.object_id = OBJECT_ID(N'[dbo].[WorkflowTransitions]')
      AND ic.column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTransitions]'), N'WorkflowDefinitionId', 'ColumnId')
      AND i.is_primary_key = 0
      AND i.is_unique_constraint = 0;
    IF @DropWorkflowTransitionDefinitionSql <> N'' EXEC sp_executesql @DropWorkflowTransitionDefinitionSql;
    ALTER TABLE [dbo].[WorkflowTransitions] DROP COLUMN [WorkflowDefinitionId];
END;
IF OBJECT_ID(N'[dbo].[WorkflowDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowDefinitions]', N'EntityType') IS NULL ALTER TABLE [dbo].[WorkflowDefinitions] ADD [EntityType] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowDefinitions_EntityType] DEFAULT N'Supplier';
IF OBJECT_ID(N'[dbo].[WorkflowDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowDefinitions]', N'IsActive') IS NULL ALTER TABLE [dbo].[WorkflowDefinitions] ADD [IsActive] bit NOT NULL CONSTRAINT [DF_WorkflowDefinitions_IsActive] DEFAULT CONVERT(bit, 1);
IF OBJECT_ID(N'[dbo].[WorkflowDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowDefinitions]', N'PublishedVersionId') IS NULL ALTER TABLE [dbo].[WorkflowDefinitions] ADD [PublishedVersionId] uniqueidentifier NULL;
IF OBJECT_ID(N'[dbo].[WorkflowMappings]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowMappings]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowMappings] PRIMARY KEY,[EntityType] nvarchar(128) NOT NULL,[ActionCode] nvarchar(128) NOT NULL,[WorkflowCode] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,CONSTRAINT [UX_WorkflowMappings_Entity_Action] UNIQUE([EntityType],[ActionCode]));
IF OBJECT_ID(N'[dbo].[DocumentTypeRequirements]', N'U') IS NULL CREATE TABLE [dbo].[DocumentTypeRequirements]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DocumentTypeRequirements] PRIMARY KEY,[EntityType] nvarchar(128) NOT NULL,[DocumentType] nvarchar(128) NOT NULL,[Name] nvarchar(256) NOT NULL,[IsRequired] bit NOT NULL,CONSTRAINT [UX_DocumentTypeRequirements_Entity_DocumentType] UNIQUE([EntityType],[DocumentType]));
IF OBJECT_ID(N'[dbo].[LookupValues]', N'U') IS NULL CREATE TABLE [dbo].[LookupValues]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_LookupValues] PRIMARY KEY,[LookupType] nvarchar(128) NOT NULL,[Code] nvarchar(128) NOT NULL,[Name] nvarchar(256) NOT NULL,[DisplayOrder] int NOT NULL,[IsActive] bit NOT NULL,CONSTRAINT [UX_LookupValues_Type_Code] UNIQUE([LookupType],[Code]));
IF OBJECT_ID(N'[dbo].[WorkflowTransitionEffects]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowTransitionEffects]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowTransitionEffects] PRIMARY KEY,[EntityType] nvarchar(128) NOT NULL,[PropertyName] nvarchar(128) NOT NULL,[ValueExpression] nvarchar(512) NOT NULL,[TriggerTransitionId] uniqueidentifier NOT NULL);
IF OBJECT_ID(N'[dbo].[FormDefinitions]', N'U') IS NULL CREATE TABLE [dbo].[FormDefinitions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormDefinitions] PRIMARY KEY,[Code] nvarchar(128) NOT NULL CONSTRAINT [UX_FormDefinitions_Code] UNIQUE,[Name] nvarchar(256) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,[ActiveVersionId] uniqueidentifier NULL);
IF OBJECT_ID(N'[dbo].[FormVersions]', N'U') IS NULL CREATE TABLE [dbo].[FormVersions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormVersions] PRIMARY KEY,[FormDefinitionId] uniqueidentifier NOT NULL,[VersionNumber] int NOT NULL,[Status] nvarchar(64) NOT NULL,[PublishedAt] datetimeoffset NULL,[PublishedBy] nvarchar(max) NULL,CONSTRAINT [FK_FormVersions_FormDefinitions] FOREIGN KEY([FormDefinitionId]) REFERENCES [dbo].[FormDefinitions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_FormVersions_Definition_Version] UNIQUE([FormDefinitionId],[VersionNumber]));
IF OBJECT_ID(N'[dbo].[FormSections]', N'U') IS NULL CREATE TABLE [dbo].[FormSections]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSections] PRIMARY KEY,[FormVersionId] uniqueidentifier NOT NULL,[Code] nvarchar(128) NOT NULL,[Title] nvarchar(256) NOT NULL,[DisplayOrder] int NOT NULL,CONSTRAINT [FK_FormSections_FormVersions] FOREIGN KEY([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFields]', N'U') IS NULL CREATE TABLE [dbo].[FormFields]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFields] PRIMARY KEY,[FormSectionId] uniqueidentifier NOT NULL,[Code] nvarchar(128) NOT NULL,[Label] nvarchar(256) NOT NULL,[FieldType] nvarchar(64) NOT NULL,[DisplayOrder] int NOT NULL,[IsRequired] bit NOT NULL,CONSTRAINT [FK_FormFields_FormSections] FOREIGN KEY([FormSectionId]) REFERENCES [dbo].[FormSections]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFieldValidations]', N'U') IS NULL CREATE TABLE [dbo].[FormFieldValidations]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFieldValidations] PRIMARY KEY,[FormFieldId] uniqueidentifier NOT NULL,[ValidationType] nvarchar(128) NOT NULL,[ConfigurationJson] nvarchar(max) NULL,[Message] nvarchar(512) NOT NULL,CONSTRAINT [FK_FormFieldValidations_FormFields] FOREIGN KEY([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFieldVisibilityRules]', N'U') IS NULL CREATE TABLE [dbo].[FormFieldVisibilityRules]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFieldVisibilityRules] PRIMARY KEY,[FormFieldId] uniqueidentifier NOT NULL,[Expression] nvarchar(512) NOT NULL,CONSTRAINT [FK_FormFieldVisibilityRules_FormFields] FOREIGN KEY([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormSubmissions]', N'U') IS NULL CREATE TABLE [dbo].[FormSubmissions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSubmissions] PRIMARY KEY,[FormDefinitionId] uniqueidentifier NOT NULL,[FormVersionId] uniqueidentifier NOT NULL,[EntityType] nvarchar(128) NOT NULL,[EntityId] uniqueidentifier NOT NULL,[SubmittedBy] nvarchar(256) NOT NULL,[SubmittedAt] datetimeoffset NOT NULL);
IF OBJECT_ID(N'[dbo].[FormSubmissionValues]', N'U') IS NULL CREATE TABLE [dbo].[FormSubmissionValues]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSubmissionValues] PRIMARY KEY,[FormSubmissionId] uniqueidentifier NOT NULL,[FieldCode] nvarchar(128) NOT NULL,[Value] nvarchar(max) NULL,CONSTRAINT [FK_FormSubmissionValues_FormSubmissions] FOREIGN KEY([FormSubmissionId]) REFERENCES [dbo].[FormSubmissions]([Id]) ON DELETE CASCADE);
", cancellationToken);

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
