using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class TenderClarificationManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'Status') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [Status] nvarchar(32) NOT NULL CONSTRAINT [DF_TenderClarifications_Status] DEFAULT N'Submitted';
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'Visibility') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [Visibility] nvarchar(32) NOT NULL CONSTRAINT [DF_TenderClarifications_Visibility] DEFAULT N'Private';
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'SupplierId') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [SupplierId] uniqueidentifier NULL;
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'SupplierName') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [SupplierName] nvarchar(256) NOT NULL CONSTRAINT [DF_TenderClarifications_SupplierName] DEFAULT N'';
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'QuestionReference') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [QuestionReference] nvarchar(64) NOT NULL CONSTRAINT [DF_TenderClarifications_QuestionReference] DEFAULT N'';
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'AssignedOfficer') IS NULL ALTER TABLE [dbo].[TenderClarifications] ADD [AssignedOfficer] nvarchar(256) NULL;
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'IsPublished') IS NULL ALTER TABLE [dbo].[TenderClarificationResponses] ADD [IsPublished] bit NOT NULL CONSTRAINT [DF_TenderClarificationResponses_IsPublished] DEFAULT CAST(0 AS bit);
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'PublishedAt') IS NULL ALTER TABLE [dbo].[TenderClarificationResponses] ADD [PublishedAt] datetimeoffset NULL;
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'PublishedBy') IS NULL ALTER TABLE [dbo].[TenderClarificationResponses] ADD [PublishedBy] nvarchar(256) NULL;
UPDATE [dbo].[TenderClarifications] SET [Visibility] = CASE WHEN [IsPublic] = 1 THEN N'Public' ELSE N'Private' END WHERE [Visibility] IS NULL OR [Visibility] = N'Private';
UPDATE [dbo].[TenderClarifications] SET [SupplierName] = [AskedBy] WHERE [SupplierName] = N'';
UPDATE [dbo].[TenderClarifications] SET [QuestionReference] = CONCAT(N'CLR-', FORMAT([AskedAt], 'yyyyMMddHHmmss')) WHERE [QuestionReference] = N'';
""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'PublishedBy') IS NOT NULL ALTER TABLE [dbo].[TenderClarificationResponses] DROP COLUMN [PublishedBy];
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'PublishedAt') IS NOT NULL ALTER TABLE [dbo].[TenderClarificationResponses] DROP COLUMN [PublishedAt];
IF COL_LENGTH(N'[dbo].[TenderClarificationResponses]', N'IsPublished') IS NOT NULL BEGIN ALTER TABLE [dbo].[TenderClarificationResponses] DROP CONSTRAINT [DF_TenderClarificationResponses_IsPublished]; ALTER TABLE [dbo].[TenderClarificationResponses] DROP COLUMN [IsPublished]; END
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'AssignedOfficer') IS NOT NULL ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [AssignedOfficer];
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'QuestionReference') IS NOT NULL BEGIN ALTER TABLE [dbo].[TenderClarifications] DROP CONSTRAINT [DF_TenderClarifications_QuestionReference]; ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [QuestionReference]; END
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'SupplierName') IS NOT NULL BEGIN ALTER TABLE [dbo].[TenderClarifications] DROP CONSTRAINT [DF_TenderClarifications_SupplierName]; ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [SupplierName]; END
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'SupplierId') IS NOT NULL ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [SupplierId];
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'Visibility') IS NOT NULL BEGIN ALTER TABLE [dbo].[TenderClarifications] DROP CONSTRAINT [DF_TenderClarifications_Visibility]; ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [Visibility]; END
IF COL_LENGTH(N'[dbo].[TenderClarifications]', N'Status') IS NOT NULL BEGIN ALTER TABLE [dbo].[TenderClarifications] DROP CONSTRAINT [DF_TenderClarifications_Status]; ALTER TABLE [dbo].[TenderClarifications] DROP COLUMN [Status]; END
""");
    }
}
