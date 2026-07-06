using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class PageDesignerFirstClassFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'ApplicationId') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [ApplicationId] uniqueidentifier NULL;
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'Route') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [Route] nvarchar(256) NOT NULL CONSTRAINT [DF_PageDefinitions_Route] DEFAULT N'/app/studio/pages';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'Icon') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [Icon] nvarchar(128) NOT NULL CONSTRAINT [DF_PageDefinitions_Icon] DEFAULT N'FileText';
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'LayoutId') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [LayoutId] uniqueidentifier NULL;
IF OBJECT_ID(N'[dbo].[PageDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[PageDefinitions]', N'PublishedVersionId') IS NULL ALTER TABLE [dbo].[PageDefinitions] ADD [PublishedVersionId] uniqueidentifier NULL;");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("ApplicationId", "PageDefinitions");
        migrationBuilder.DropColumn("Route", "PageDefinitions");
        migrationBuilder.DropColumn("Icon", "PageDefinitions");
        migrationBuilder.DropColumn("LayoutId", "PageDefinitions");
        migrationBuilder.DropColumn("PublishedVersionId", "PageDefinitions");
    }
}
