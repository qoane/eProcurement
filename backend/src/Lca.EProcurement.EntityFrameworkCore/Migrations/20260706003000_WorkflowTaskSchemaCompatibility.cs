using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowTaskSchemaCompatibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowTasks]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowTasks] PRIMARY KEY,[WorkflowInstanceId] uniqueidentifier NOT NULL,[NodeCode] nvarchar(128) NOT NULL,[AssignedRole] nvarchar(128) NULL,[AssignedTo] nvarchar(256) NULL,[Status] nvarchar(64) NOT NULL,[CreatedAt] datetimeoffset NOT NULL,[AssignedAt] datetimeoffset NULL,[CompletedAt] datetimeoffset NULL);
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'WorkflowInstanceId') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [WorkflowInstanceId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowTasks_WorkflowInstanceId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'NodeCode') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [NodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTasks_NodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'AssignedRole') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [AssignedRole] nvarchar(128) NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'AssignedTo') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [AssignedTo] nvarchar(256) NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'Status') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [Status] nvarchar(64) NOT NULL CONSTRAINT [DF_WorkflowTasks_Status] DEFAULT N'Open';
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'CreatedAt') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [CreatedAt] datetimeoffset NOT NULL CONSTRAINT [DF_WorkflowTasks_CreatedAt] DEFAULT SYSUTCDATETIME();
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'AssignedAt') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [AssignedAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'CompletedAt') IS NULL ALTER TABLE [dbo].[WorkflowTasks] ADD [CompletedAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'StepCode') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'NodeCode') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowTasks] SET [NodeCode] = [StepCode] WHERE ([NodeCode] = N'''' OR [NodeCode] IS NULL) AND [StepCode] IS NOT NULL');
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
