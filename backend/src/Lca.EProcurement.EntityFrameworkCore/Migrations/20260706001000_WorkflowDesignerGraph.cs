using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowDesignerGraph : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'PositionX') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [PositionX] int NOT NULL CONSTRAINT [DF_WorkflowNodes_PositionX] DEFAULT 0;
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'PositionY') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [PositionY] int NOT NULL CONSTRAINT [DF_WorkflowNodes_PositionY] DEFAULT 0;
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'ActionConfigurationJson') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [ActionConfigurationJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowNodes_ActionConfigurationJson] DEFAULT N'{}';
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'ConditionConfigurationJson') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [ConditionConfigurationJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowNodes_ConditionConfigurationJson] DEFAULT N'{}';
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'BusinessRuleCodesJson') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [BusinessRuleCodesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowNodes_BusinessRuleCodesJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[WorkflowNodes]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowNodes]', N'AssignedRolesJson') IS NULL ALTER TABLE [dbo].[WorkflowNodes] ADD [AssignedRolesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowNodes_AssignedRolesJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ConditionExpression') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ConditionExpression] nvarchar(1000) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ConditionExpression] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ActionConfigurationJson') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ActionConfigurationJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ActionConfigurationJson] DEFAULT N'{}';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'BusinessRuleCodesJson') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [BusinessRuleCodesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowTransitions_BusinessRuleCodesJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'AssignedRolesJson') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [AssignedRolesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_WorkflowTransitions_AssignedRolesJson] DEFAULT N'[]';");
    }

    protected override void Down(MigrationBuilder migrationBuilder) { }
}
