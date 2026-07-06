using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class ComponentLibraryMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'Category') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [Category] nvarchar(128) NOT NULL CONSTRAINT [DF_ComponentDefinitions_Category] DEFAULT N'General';
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'RendererKey') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [RendererKey] nvarchar(128) NOT NULL CONSTRAINT [DF_ComponentDefinitions_RendererKey] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'PropertiesJson') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [PropertiesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_ComponentDefinitions_PropertiesJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'EventsJson') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [EventsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_ComponentDefinitions_EventsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'ValidationJson') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [ValidationJson] nvarchar(max) NOT NULL CONSTRAINT [DF_ComponentDefinitions_ValidationJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[ComponentDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ComponentDefinitions]', N'DesignMetadataJson') IS NULL ALTER TABLE [dbo].[ComponentDefinitions] ADD [DesignMetadataJson] nvarchar(max) NOT NULL CONSTRAINT [DF_ComponentDefinitions_DesignMetadataJson] DEFAULT N'{}';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("Category", "ComponentDefinitions");
        migrationBuilder.DropColumn("RendererKey", "ComponentDefinitions");
        migrationBuilder.DropColumn("PropertiesJson", "ComponentDefinitions");
        migrationBuilder.DropColumn("EventsJson", "ComponentDefinitions");
        migrationBuilder.DropColumn("ValidationJson", "ComponentDefinitions");
        migrationBuilder.DropColumn("DesignMetadataJson", "ComponentDefinitions");
    }
}
