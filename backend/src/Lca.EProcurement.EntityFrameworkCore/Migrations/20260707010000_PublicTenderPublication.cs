using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class PublicTenderPublication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenders]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Tenders]', N'Category') IS NULL ALTER TABLE [dbo].[Tenders] ADD [Category] nvarchar(128) NOT NULL CONSTRAINT [DF_Tenders_Category] DEFAULT N'General';
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'IsPublic') IS NULL ALTER TABLE [dbo].[TenderDocuments] ADD [IsPublic] bit NOT NULL CONSTRAINT [DF_TenderDocuments_IsPublic] DEFAULT 0;
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'PublicUrl') IS NULL ALTER TABLE [dbo].[TenderDocuments] ADD [PublicUrl] nvarchar(512) NULL;
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'IsDownloadable') IS NULL ALTER TABLE [dbo].[TenderDocuments] ADD [IsDownloadable] bit NOT NULL CONSTRAINT [DF_TenderDocuments_IsDownloadable] DEFAULT 1;
IF OBJECT_ID(N'[dbo].[PublicTenderPublications]', N'U') IS NULL CREATE TABLE [dbo].[PublicTenderPublications]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_PublicTenderPublications] PRIMARY KEY,[TenderId] uniqueidentifier NOT NULL,[TenderNumber] nvarchar(64) NOT NULL,[Reference] nvarchar(64) NOT NULL,[Title] nvarchar(256) NOT NULL,[Description] nvarchar(4000) NOT NULL,[TenderType] nvarchar(32) NOT NULL,[ProcurementMethod] nvarchar(128) NOT NULL,[Category] nvarchar(128) NOT NULL,[PublishedAt] datetimeoffset NOT NULL,[ClosingDate] datetimeoffset NOT NULL,[Status] nvarchar(64) NOT NULL,[IsVisible] bit NOT NULL,[Slug] nvarchar(256) NOT NULL,[CreatedAt] datetimeoffset NOT NULL,[UpdatedAt] datetimeoffset NOT NULL,CONSTRAINT [UX_PublicTenderPublications_TenderId] UNIQUE([TenderId]),CONSTRAINT [UX_PublicTenderPublications_Reference] UNIQUE([Reference]),CONSTRAINT [UX_PublicTenderPublications_Slug] UNIQUE([Slug]));
IF OBJECT_ID(N'[dbo].[PublicTenderDocuments]', N'U') IS NULL CREATE TABLE [dbo].[PublicTenderDocuments]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_PublicTenderDocuments] PRIMARY KEY,[PublicTenderPublicationId] uniqueidentifier NOT NULL,[DocumentType] nvarchar(128) NOT NULL,[FileName] nvarchar(256) NOT NULL,[PublicUrl] nvarchar(512) NOT NULL,[IsDownloadable] bit NOT NULL,[PublishedAt] datetimeoffset NOT NULL,CONSTRAINT [FK_PublicTenderDocuments_PublicTenderPublications] FOREIGN KEY([PublicTenderPublicationId]) REFERENCES [dbo].[PublicTenderPublications]([Id]) ON DELETE CASCADE);
INSERT INTO [dbo].[PublicTenderPublications]([Id],[TenderId],[TenderNumber],[Reference],[Title],[Description],[TenderType],[ProcurementMethod],[Category],[PublishedAt],[ClosingDate],[Status],[IsVisible],[Slug],[CreatedAt],[UpdatedAt]) SELECT NEWID(),[Id],[TenderNumber],[TenderNumber],[Title],[Description],[TenderType],[ProcurementMethod],[Category],COALESCE([PublishedAt],[PublicationDate],SYSDATETIMEOFFSET()),[ClosingDate],[Status],1,LOWER(REPLACE([TenderNumber] + N'-' + [Title], N' ', N'-')),COALESCE([PublishedAt],[PublicationDate],SYSDATETIMEOFFSET()),SYSDATETIMEOFFSET() FROM [dbo].[Tenders] t WHERE [Status] = N'Published' AND NOT EXISTS (SELECT 1 FROM [dbo].[PublicTenderPublications] p WHERE p.[TenderId] = t.[Id]);
IF OBJECT_ID(N'[dbo].[PublicTenderClarifications]', N'U') IS NULL CREATE TABLE [dbo].[PublicTenderClarifications]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_PublicTenderClarifications] PRIMARY KEY,[PublicTenderPublicationId] uniqueidentifier NOT NULL,[TenderClarificationId] uniqueidentifier NOT NULL,[Question] nvarchar(4000) NOT NULL,[Response] nvarchar(4000) NOT NULL,[PublishedAt] datetimeoffset NOT NULL,CONSTRAINT [FK_PublicTenderClarifications_PublicTenderPublications] FOREIGN KEY([PublicTenderPublicationId]) REFERENCES [dbo].[PublicTenderPublications]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_PublicTenderClarifications_Publication_Clarification] UNIQUE([PublicTenderPublicationId],[TenderClarificationId]));
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PublicTenderClarifications]', N'U') IS NOT NULL DROP TABLE [dbo].[PublicTenderClarifications];
IF OBJECT_ID(N'[dbo].[PublicTenderDocuments]', N'U') IS NOT NULL DROP TABLE [dbo].[PublicTenderDocuments];
IF OBJECT_ID(N'[dbo].[PublicTenderPublications]', N'U') IS NOT NULL DROP TABLE [dbo].[PublicTenderPublications];
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_TenderDocuments_IsDownloadable]', N'D') IS NOT NULL ALTER TABLE [dbo].[TenderDocuments] DROP CONSTRAINT [DF_TenderDocuments_IsDownloadable];
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'IsDownloadable') IS NOT NULL ALTER TABLE [dbo].[TenderDocuments] DROP COLUMN [IsDownloadable];
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'PublicUrl') IS NOT NULL ALTER TABLE [dbo].[TenderDocuments] DROP COLUMN [PublicUrl];
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_TenderDocuments_IsPublic]', N'D') IS NOT NULL ALTER TABLE [dbo].[TenderDocuments] DROP CONSTRAINT [DF_TenderDocuments_IsPublic];
IF OBJECT_ID(N'[dbo].[TenderDocuments]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[TenderDocuments]', N'IsPublic') IS NOT NULL ALTER TABLE [dbo].[TenderDocuments] DROP COLUMN [IsPublic];
IF OBJECT_ID(N'[dbo].[Tenders]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_Tenders_Category]', N'D') IS NOT NULL ALTER TABLE [dbo].[Tenders] DROP CONSTRAINT [DF_Tenders_Category];
IF OBJECT_ID(N'[dbo].[Tenders]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Tenders]', N'Category') IS NOT NULL ALTER TABLE [dbo].[Tenders] DROP COLUMN [Category];
");
    }
}
