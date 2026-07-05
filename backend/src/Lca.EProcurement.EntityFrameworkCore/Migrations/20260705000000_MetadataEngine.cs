using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class MetadataEngine : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var table in Tables)
        {
            migrationBuilder.Sql($@"
IF OBJECT_ID(N'[dbo].[{table}]', N'U') IS NULL CREATE TABLE [dbo].[{table}](
    [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_{table}] PRIMARY KEY,
    [Code] nvarchar(128) NOT NULL CONSTRAINT [UX_{table}_Code] UNIQUE,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Version] int NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    [Created] datetimeoffset NOT NULL,
    [Modified] datetimeoffset NULL,
    [CreatedBy] nvarchar(256) NOT NULL,
    [ModifiedBy] nvarchar(256) NULL); ");
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in Tables.Reverse()) migrationBuilder.DropTable(table);
    }

    private static readonly string[] Tables =
    [
        "Applications", "BusinessProcesses", "EntityDefinitions", "PageDefinitions", "LayoutDefinitions", "ComponentDefinitions", "NavigationDefinitions", "MenuDefinitions", "DashboardDefinitions", "ReportDefinitions", "ThemeDefinitions", "LookupDefinitions", "DocumentTypeDefinitions", "SystemSettings"
    ];
}
