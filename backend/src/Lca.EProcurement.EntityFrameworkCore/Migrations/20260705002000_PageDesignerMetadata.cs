using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class PageDesignerMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'PageType') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [PageType] nvarchar(64) NOT NULL CONSTRAINT [DF_PageDefinitions_PageType] DEFAULT N'Dashboard';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'DatasourceJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [DatasourceJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_DatasourceJson] DEFAULT N'{}';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'LayoutJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [LayoutJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_LayoutJson] DEFAULT N'{}';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'ToolbarJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [ToolbarJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_ToolbarJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'ActionsJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [ActionsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_ActionsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'FiltersJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [FiltersJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_FiltersJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'ColumnsJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [ColumnsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_ColumnsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'ComponentsJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [ComponentsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_ComponentsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'PermissionsJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [PermissionsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_PermissionsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'NavigationJson') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [NavigationJson] nvarchar(max) NOT NULL CONSTRAINT [DF_PageDefinitions_NavigationJson] DEFAULT N'{}';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("PageType", "PageDefinitions");
        migrationBuilder.DropColumn("DatasourceJson", "PageDefinitions");
        migrationBuilder.DropColumn("LayoutJson", "PageDefinitions");
        migrationBuilder.DropColumn("ToolbarJson", "PageDefinitions");
        migrationBuilder.DropColumn("ActionsJson", "PageDefinitions");
        migrationBuilder.DropColumn("FiltersJson", "PageDefinitions");
        migrationBuilder.DropColumn("ColumnsJson", "PageDefinitions");
        migrationBuilder.DropColumn("ComponentsJson", "PageDefinitions");
        migrationBuilder.DropColumn("PermissionsJson", "PageDefinitions");
        migrationBuilder.DropColumn("NavigationJson", "PageDefinitions");
    }
}
