using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class CommunicationNotificationsPhase1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[NotificationRetryQueues]', N'U') IS NULL CREATE TABLE [dbo].[NotificationRetryQueues]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_NotificationRetryQueues] PRIMARY KEY,[DeliveryLogId] uniqueidentifier NOT NULL,[NextAttemptAt] datetimeoffset NOT NULL,[AttemptCount] int NOT NULL,[MaxAttempts] int NOT NULL,[Status] nvarchar(32) NOT NULL,[LastError] nvarchar(2000) NULL,CONSTRAINT [FK_NotificationRetryQueues_NotificationDeliveryLogs_DeliveryLogId] FOREIGN KEY([DeliveryLogId]) REFERENCES [dbo].[NotificationDeliveryLogs]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[CommunicationThreads]', N'U') IS NULL CREATE TABLE [dbo].[CommunicationThreads]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_CommunicationThreads] PRIMARY KEY,[ThreadNumber] nvarchar(64) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[EntityId] uniqueidentifier NOT NULL,[EntityReference] nvarchar(128) NOT NULL,[Subject] nvarchar(512) NOT NULL,[Visibility] nvarchar(64) NOT NULL,[CreatedBy] nvarchar(256) NOT NULL,[CreatedAt] datetimeoffset NOT NULL,[ClosedAt] datetimeoffset NULL,[Status] nvarchar(64) NOT NULL,[SupplierId] uniqueidentifier NULL,CONSTRAINT [UX_CommunicationThreads_ThreadNumber] UNIQUE([ThreadNumber]));
IF OBJECT_ID(N'[dbo].[CommunicationMessages]', N'U') IS NULL CREATE TABLE [dbo].[CommunicationMessages]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_CommunicationMessages] PRIMARY KEY,[ThreadId] uniqueidentifier NOT NULL,[SenderUserId] nvarchar(256) NOT NULL,[SenderName] nvarchar(256) NOT NULL,[SenderType] nvarchar(64) NOT NULL,[Body] nvarchar(max) NOT NULL,[IsInternal] bit NOT NULL,[IsPublic] bit NOT NULL,[CreatedAt] datetimeoffset NOT NULL,CONSTRAINT [FK_CommunicationMessages_CommunicationThreads_ThreadId] FOREIGN KEY([ThreadId]) REFERENCES [dbo].[CommunicationThreads]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[DeadlineReminderRules]', N'U') IS NULL CREATE TABLE [dbo].[DeadlineReminderRules]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DeadlineReminderRules] PRIMARY KEY,[Code] nvarchar(128) NOT NULL,[Name] nvarchar(256) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[DateField] nvarchar(128) NOT NULL,[ReminderOffsetHours] int NOT NULL,[TemplateCode] nvarchar(128) NOT NULL,[RecipientRule] nvarchar(128) NOT NULL,[IsEnabled] bit NOT NULL,[CreatedAt] datetimeoffset NOT NULL,CONSTRAINT [UX_DeadlineReminderRules_Code] UNIQUE([Code]));
IF OBJECT_ID(N'[dbo].[DeadlineReminderRuns]', N'U') IS NULL CREATE TABLE [dbo].[DeadlineReminderRuns]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DeadlineReminderRuns] PRIMARY KEY,[RuleId] uniqueidentifier NOT NULL,[EntityType] nvarchar(128) NOT NULL,[EntityId] uniqueidentifier NOT NULL,[EntityReference] nvarchar(128) NOT NULL,[ScheduledFor] datetimeoffset NOT NULL,[ExecutedAt] datetimeoffset NULL,[Status] nvarchar(32) NOT NULL,[ErrorMessage] nvarchar(2000) NULL,CONSTRAINT [FK_DeadlineReminderRuns_DeadlineReminderRules_RuleId] FOREIGN KEY([RuleId]) REFERENCES [dbo].[DeadlineReminderRules]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_DeadlineReminderRuns_Rule_Entity_Scheduled] UNIQUE([RuleId],[EntityId],[ScheduledFor]));");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS [dbo].[DeadlineReminderRuns];DROP TABLE IF EXISTS [dbo].[DeadlineReminderRules];DROP TABLE IF EXISTS [dbo].[CommunicationMessages];DROP TABLE IF EXISTS [dbo].[CommunicationThreads];DROP TABLE IF EXISTS [dbo].[NotificationRetryQueues];");
    }
}
