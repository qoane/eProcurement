using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowInstanceSchemaCompatibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowInstances]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowInstances] PRIMARY KEY,[WorkflowDefinitionId] uniqueidentifier NOT NULL,[WorkflowVersionId] uniqueidentifier NOT NULL,[EntityType] nvarchar(128) NOT NULL,[EntityId] uniqueidentifier NOT NULL,[CurrentNodeCode] nvarchar(128) NOT NULL,[Status] nvarchar(64) NOT NULL,[StartedAt] datetimeoffset NOT NULL,[CompletedAt] datetimeoffset NULL,[CancelledAt] datetimeoffset NULL);
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'WorkflowDefinitionId') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [WorkflowDefinitionId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowInstances_WorkflowDefinitionId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'WorkflowVersionId') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [WorkflowVersionId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowInstances_WorkflowVersionId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'EntityType') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [EntityType] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowInstances_EntityType] DEFAULT N'Supplier';
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'EntityId') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [EntityId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowInstances_EntityId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CurrentNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [CurrentNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowInstances_CurrentNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'Status') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [Status] nvarchar(64) NOT NULL CONSTRAINT [DF_WorkflowInstances_Status] DEFAULT N'Running';
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'StartedAt') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [StartedAt] datetimeoffset NOT NULL CONSTRAINT [DF_WorkflowInstances_StartedAt] DEFAULT SYSUTCDATETIME();
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CompletedAt') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [CompletedAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CancelledAt') IS NULL ALTER TABLE [dbo].[WorkflowInstances] ADD [CancelledAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CurrentStepCode') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CurrentNodeCode') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowInstances] SET [CurrentNodeCode] = [CurrentStepCode] WHERE ([CurrentNodeCode] = N'''' OR [CurrentNodeCode] IS NULL) AND [CurrentStepCode] IS NOT NULL');
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'WorkflowDefinitionId') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'WorkflowVersionId') IS NOT NULL AND OBJECT_ID(N'[dbo].[WorkflowDefinitions]', N'U') IS NOT NULL EXEC(N'UPDATE i SET [WorkflowVersionId] = d.[PublishedVersionId] FROM [dbo].[WorkflowInstances] i INNER JOIN [dbo].[WorkflowDefinitions] d ON d.[Id] = i.[WorkflowDefinitionId] WHERE i.[WorkflowVersionId] = CONVERT(uniqueidentifier, ''00000000-0000-0000-0000-000000000000'') AND d.[PublishedVersionId] IS NOT NULL');
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
