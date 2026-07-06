using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowHistorySchemaCompatibility : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowActions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowActions] PRIMARY KEY,[WorkflowInstanceId] uniqueidentifier NOT NULL,[ActionCode] nvarchar(128) NOT NULL,[ActionName] nvarchar(256) NOT NULL,[Kind] nvarchar(64) NOT NULL,[FromNodeCode] nvarchar(128) NOT NULL,[ToNodeCode] nvarchar(128) NOT NULL,[Actor] nvarchar(256) NOT NULL,[ActionedAt] datetimeoffset NOT NULL,[WorkflowTaskId] uniqueidentifier NULL,[Details] nvarchar(max) NULL);
IF OBJECT_ID(N'[dbo].[WorkflowHistories]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowHistories]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowHistories] PRIMARY KEY,[WorkflowInstanceId] uniqueidentifier NOT NULL,[EventType] nvarchar(128) NOT NULL,[NodeCode] nvarchar(128) NOT NULL,[Actor] nvarchar(256) NOT NULL,[Details] nvarchar(2000) NOT NULL,[OccurredAt] datetimeoffset NOT NULL,[WorkflowTaskId] uniqueidentifier NULL);
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
