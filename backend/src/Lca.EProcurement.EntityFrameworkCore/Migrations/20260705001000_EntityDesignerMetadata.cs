using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class EntityDesignerMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'DisplayName') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [DisplayName] nvarchar(256) NOT NULL CONSTRAINT [DF_EntityDefinitions_DisplayName] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'PluralName') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [PluralName] nvarchar(256) NOT NULL CONSTRAINT [DF_EntityDefinitions_PluralName] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'DefaultSearchField') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [DefaultSearchField] nvarchar(128) NOT NULL CONSTRAINT [DF_EntityDefinitions_DefaultSearchField] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'PropertiesJson') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [PropertiesJson] nvarchar(max) NOT NULL CONSTRAINT [DF_EntityDefinitions_PropertiesJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'RelationshipsJson') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [RelationshipsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_EntityDefinitions_RelationshipsJson] DEFAULT N'[]';
IF OBJECT_ID(N'[dbo].[EntityDefinitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[EntityDefinitions]', N'ValidationsJson') IS NULL ALTER TABLE [dbo].[EntityDefinitions] ADD [ValidationsJson] nvarchar(max) NOT NULL CONSTRAINT [DF_EntityDefinitions_ValidationsJson] DEFAULT N'[]';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("DisplayName", "EntityDefinitions");
        migrationBuilder.DropColumn("PluralName", "EntityDefinitions");
        migrationBuilder.DropColumn("DefaultSearchField", "EntityDefinitions");
        migrationBuilder.DropColumn("PropertiesJson", "EntityDefinitions");
        migrationBuilder.DropColumn("RelationshipsJson", "EntityDefinitions");
        migrationBuilder.DropColumn("ValidationsJson", "EntityDefinitions");
    }
}
