using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class BidOpeningManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[BidOpeningSessions]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningSessions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningSessions] PRIMARY KEY,[SessionNumber] nvarchar(64) NOT NULL,[TenderId] uniqueidentifier NOT NULL,[Title] nvarchar(256) NOT NULL,[ScheduledAt] datetimeoffset NOT NULL,[Status] nvarchar(64) NOT NULL,[CreatedBy] nvarchar(256) NOT NULL,[CreatedAt] datetimeoffset NOT NULL,[Chairperson] nvarchar(256) NOT NULL,[Notes] nvarchar(2000) NULL,[StartedAt] datetimeoffset NULL,[CompletedAt] datetimeoffset NULL);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_BidOpeningSessions_SessionNumber') CREATE UNIQUE INDEX [IX_BidOpeningSessions_SessionNumber] ON [dbo].[BidOpeningSessions]([SessionNumber]);
IF OBJECT_ID(N'[dbo].[BidOpeningCommitteeMembers]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningCommitteeMembers]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningCommitteeMembers] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[Name] nvarchar(256) NOT NULL,[Email] nvarchar(256) NOT NULL,[Role] nvarchar(128) NOT NULL,[AttendanceConfirmed] bit NOT NULL,[ConfirmedAt] datetimeoffset NULL,CONSTRAINT [FK_BidOpeningCommitteeMembers_BidOpeningSessions] FOREIGN KEY([BidOpeningSessionId]) REFERENCES [dbo].[BidOpeningSessions]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[BidOpeningSubmissions]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningSubmissions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningSubmissions] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[BidSubmissionId] uniqueidentifier NOT NULL,[SupplierId] uniqueidentifier NOT NULL,[SupplierName] nvarchar(256) NOT NULL,[SubmissionNumber] nvarchar(64) NOT NULL,[SubmittedAt] datetimeoffset NULL,[Status] nvarchar(64) NOT NULL,[OpenedAt] datetimeoffset NULL,[OpenedBy] nvarchar(256) NULL,[Notes] nvarchar(2000) NULL,[SealedDocumentHash] nvarchar(512) NULL,[OpeningKeyReference] nvarchar(512) NULL,[DigitalSignatureReference] nvarchar(512) NULL,[TimestampAuthorityReference] nvarchar(512) NULL,[SecureVaultReference] nvarchar(512) NULL,CONSTRAINT [FK_BidOpeningSubmissions_BidOpeningSessions] FOREIGN KEY([BidOpeningSessionId]) REFERENCES [dbo].[BidOpeningSessions]([Id]) ON DELETE CASCADE);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_BidOpeningSubmissions_Session_Bid') CREATE UNIQUE INDEX [IX_BidOpeningSubmissions_Session_Bid] ON [dbo].[BidOpeningSubmissions]([BidOpeningSessionId],[BidSubmissionId]);
IF OBJECT_ID(N'[dbo].[BidOpeningMinutes]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningMinutes]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningMinutes] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[MinuteText] nvarchar(4000) NOT NULL,[RecordedBy] nvarchar(256) NOT NULL,[RecordedAt] datetimeoffset NOT NULL,CONSTRAINT [FK_BidOpeningMinutes_BidOpeningSessions] FOREIGN KEY([BidOpeningSessionId]) REFERENCES [dbo].[BidOpeningSessions]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[BidOpeningChecklistItems]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningChecklistItems]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningChecklistItems] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[Description] nvarchar(512) NOT NULL,[Completed] bit NOT NULL,[CompletedBy] nvarchar(256) NULL,[CompletedAt] datetimeoffset NULL,CONSTRAINT [FK_BidOpeningChecklistItems_BidOpeningSessions] FOREIGN KEY([BidOpeningSessionId]) REFERENCES [dbo].[BidOpeningSessions]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[BidOpeningReports]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningReports]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningReports] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[ReportNumber] nvarchar(64) NOT NULL,[GeneratedAt] datetimeoffset NOT NULL,[GeneratedBy] nvarchar(256) NOT NULL,[SummaryJson] nvarchar(max) NOT NULL,CONSTRAINT [FK_BidOpeningReports_BidOpeningSessions] FOREIGN KEY([BidOpeningSessionId]) REFERENCES [dbo].[BidOpeningSessions]([Id]) ON DELETE CASCADE);
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS [dbo].[BidOpeningReports];DROP TABLE IF EXISTS [dbo].[BidOpeningChecklistItems];DROP TABLE IF EXISTS [dbo].[BidOpeningMinutes];DROP TABLE IF EXISTS [dbo].[BidOpeningSubmissions];DROP TABLE IF EXISTS [dbo].[BidOpeningCommitteeMembers];DROP TABLE IF EXISTS [dbo].[BidOpeningSessions];");
    }
}
