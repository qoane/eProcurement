using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class SealedBidSecurity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[SealedBidEnvelopes]', N'U') IS NULL CREATE TABLE [dbo].[SealedBidEnvelopes]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_SealedBidEnvelopes] PRIMARY KEY,[BidSubmissionId] uniqueidentifier NOT NULL,[TenderId] uniqueidentifier NOT NULL,[SupplierId] uniqueidentifier NOT NULL,[EnvelopeNumber] nvarchar(64) NOT NULL,[Status] nvarchar(64) NOT NULL,[SealedAt] datetimeoffset NULL,[SealedBy] nvarchar(256) NULL,[OpenedAt] datetimeoffset NULL,[OpenedBy] nvarchar(256) NULL,[OpeningSessionId] uniqueidentifier NULL,[SubmissionHash] nvarchar(128) NOT NULL,[DocumentManifestHash] nvarchar(128) NOT NULL,[TimestampReference] nvarchar(128) NOT NULL,[DigitalSignatureReference] nvarchar(512) NULL,[SecureVaultReference] nvarchar(512) NULL,[OpeningKeyReference] nvarchar(512) NULL,[CreatedAt] datetimeoffset NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_SealedBidEnvelopes_BidSubmissionId') CREATE UNIQUE INDEX [IX_SealedBidEnvelopes_BidSubmissionId] ON [dbo].[SealedBidEnvelopes]([BidSubmissionId]);
IF OBJECT_ID(N'[dbo].[SealedBidDocumentEvidence]', N'U') IS NULL CREATE TABLE [dbo].[SealedBidDocumentEvidence]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_SealedBidDocumentEvidence] PRIMARY KEY,[BidSubmissionDocumentId] uniqueidentifier NOT NULL,[BidSubmissionId] uniqueidentifier NOT NULL,[FileName] nvarchar(256) NOT NULL,[DocumentType] nvarchar(128) NOT NULL,[StorageReference] nvarchar(512) NOT NULL,[FileSize] bigint NOT NULL,[ContentHash] nvarchar(128) NOT NULL,[HashAlgorithm] nvarchar(32) NOT NULL,[UploadedAt] datetimeoffset NOT NULL,[SealedAt] datetimeoffset NULL,[IsPublicBeforeOpening] bit NOT NULL,[CreatedAt] datetimeoffset NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_SealedBidDocumentEvidence_BidSubmissionDocumentId') CREATE UNIQUE INDEX [IX_SealedBidDocumentEvidence_BidSubmissionDocumentId] ON [dbo].[SealedBidDocumentEvidence]([BidSubmissionDocumentId]);
IF OBJECT_ID(N'[dbo].[BidAccessLogs]', N'U') IS NULL CREATE TABLE [dbo].[BidAccessLogs]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidAccessLogs] PRIMARY KEY,[BidSubmissionId] uniqueidentifier NOT NULL,[UserId] nvarchar(256) NOT NULL,[UserEmail] nvarchar(256) NOT NULL,[AccessType] nvarchar(64) NOT NULL,[AccessAllowed] bit NOT NULL,[DeniedReason] nvarchar(1000) NULL,[AccessedAt] datetimeoffset NOT NULL,[IpAddress] nvarchar(64) NULL,[UserAgent] nvarchar(512) NULL);
IF OBJECT_ID(N'[dbo].[BidOpeningEvidence]', N'U') IS NULL CREATE TABLE [dbo].[BidOpeningEvidence]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_BidOpeningEvidence] PRIMARY KEY,[BidOpeningSessionId] uniqueidentifier NOT NULL,[BidSubmissionId] uniqueidentifier NOT NULL,[OpenedBy] nvarchar(256) NOT NULL,[OpenedAt] datetimeoffset NOT NULL,[OpeningReason] nvarchar(1000) NOT NULL,[SubmissionHashAtOpening] nvarchar(128) NOT NULL,[DocumentManifestHashAtOpening] nvarchar(128) NOT NULL,[IntegrityCheckPassed] bit NOT NULL,[IntegrityCheckResultJson] nvarchar(max) NOT NULL,[CreatedAt] datetimeoffset NOT NULL);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_BidOpeningEvidence_Session_Bid') CREATE INDEX [IX_BidOpeningEvidence_Session_Bid] ON [dbo].[BidOpeningEvidence]([BidOpeningSessionId],[BidSubmissionId]);
");
    }
    protected override void Down(MigrationBuilder migrationBuilder) { }
}
