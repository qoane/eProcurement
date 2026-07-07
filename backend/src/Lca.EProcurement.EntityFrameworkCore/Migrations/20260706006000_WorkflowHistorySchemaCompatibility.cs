using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowHistorySchemaCompatibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowActions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowActions] PRIMARY KEY,[WorkflowInstanceId] uniqueidentifier NOT NULL,[ActionCode] nvarchar(128) NOT NULL,[ActionName] nvarchar(256) NOT NULL,[Kind] nvarchar(64) NOT NULL,[FromNodeCode] nvarchar(128) NOT NULL,[ToNodeCode] nvarchar(128) NOT NULL,[Actor] nvarchar(256) NOT NULL,[ActionedAt] datetimeoffset NOT NULL,[WorkflowTaskId] uniqueidentifier NULL,[Details] nvarchar(max) NULL);
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'WorkflowInstanceId') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [WorkflowInstanceId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowActions_WorkflowInstanceId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ActionCode') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [ActionCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowActions_ActionCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ActionName') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [ActionName] nvarchar(256) NOT NULL CONSTRAINT [DF_WorkflowActions_ActionName] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'Kind') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [Kind] nvarchar(64) NOT NULL CONSTRAINT [DF_WorkflowActions_Kind] DEFAULT N'Transition';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'FromNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [FromNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowActions_FromNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ToNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [ToNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowActions_ToNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'Actor') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [Actor] nvarchar(256) NOT NULL CONSTRAINT [DF_WorkflowActions_Actor] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ActionedAt') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [ActionedAt] datetimeoffset NOT NULL CONSTRAINT [DF_WorkflowActions_ActionedAt] DEFAULT SYSUTCDATETIME();
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'WorkflowTaskId') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [WorkflowTaskId] uniqueidentifier NULL;
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'Details') IS NULL ALTER TABLE [dbo].[WorkflowActions] ADD [Details] nvarchar(max) NULL;
IF OBJECT_ID(N'[dbo].[WorkflowHistories]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowHistories]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowHistories] PRIMARY KEY,[WorkflowInstanceId] uniqueidentifier NOT NULL,[EventType] nvarchar(128) NOT NULL,[NodeCode] nvarchar(128) NOT NULL,[Actor] nvarchar(256) NOT NULL,[Details] nvarchar(2000) NOT NULL,[OccurredAt] datetimeoffset NOT NULL,[WorkflowTaskId] uniqueidentifier NULL);
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
