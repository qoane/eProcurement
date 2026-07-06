using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class BusinessRuleDesigner : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'Category') IS NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] ADD [Category] nvarchar(128) NOT NULL CONSTRAINT [DF_BusinessRuleDefinitions_Category] DEFAULT N'General';
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'Status') IS NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] ADD [Status] nvarchar(64) NOT NULL CONSTRAINT [DF_BusinessRuleDefinitions_Status] DEFAULT N'Draft';
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'FailureMessage') IS NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] ADD [FailureMessage] nvarchar(512) NOT NULL CONSTRAINT [DF_BusinessRuleDefinitions_FailureMessage] DEFAULT N'Rule failed';
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'PublishedAt') IS NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] ADD [PublishedAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'PublishedBy') IS NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] ADD [PublishedBy] nvarchar(256) NULL;
""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'PublishedBy') IS NOT NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] DROP COLUMN [PublishedBy];
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'PublishedAt') IS NOT NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] DROP COLUMN [PublishedAt];
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'FailureMessage') IS NOT NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] DROP COLUMN [FailureMessage];
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'Status') IS NOT NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] DROP COLUMN [Status];
IF OBJECT_ID(N'[dbo].[BusinessRuleDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[BusinessRuleDefinitions]', N'Category') IS NOT NULL ALTER TABLE [dbo].[BusinessRuleDefinitions] DROP COLUMN [Category];
""");
    }
}
