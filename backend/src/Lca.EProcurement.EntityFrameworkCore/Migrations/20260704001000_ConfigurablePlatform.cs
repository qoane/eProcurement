using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class ConfigurablePlatform : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowMappings]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowMappings]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowMappings] PRIMARY KEY,[EntityType] nvarchar(128) NOT NULL,[ActionCode] nvarchar(128) NOT NULL,[WorkflowCode] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,CONSTRAINT [UX_WorkflowMappings_Entity_Action] UNIQUE([EntityType],[ActionCode]));
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'WorkflowVersionId') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [WorkflowVersionId] uniqueidentifier NOT NULL CONSTRAINT [DF_WorkflowTransitions_WorkflowVersionId] DEFAULT CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000');
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'FromNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [FromNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_FromNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ActionCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ActionCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ActionCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ActionName') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ActionName] nvarchar(256) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ActionName] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'ToNodeCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [ToNodeCode] nvarchar(128) NOT NULL CONSTRAINT [DF_WorkflowTransitions_ToNodeCode] DEFAULT N'';
IF OBJECT_ID(N'[dbo].[WorkflowTransitions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTransitions]', N'RequiredRuleCode') IS NULL ALTER TABLE [dbo].[WorkflowTransitions] ADD [RequiredRuleCode] nvarchar(128) NULL;
IF OBJECT_ID(N'[dbo].[WorkflowTransitionEffects]', N'U') IS NULL CREATE TABLE [dbo].[WorkflowTransitionEffects]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_WorkflowTransitionEffects] PRIMARY KEY,[EntityType] nvarchar(128) NOT NULL,[PropertyName] nvarchar(128) NOT NULL,[ValueExpression] nvarchar(512) NOT NULL,[TriggerTransitionId] uniqueidentifier NOT NULL);
IF OBJECT_ID(N'[dbo].[FormDefinitions]', N'U') IS NULL CREATE TABLE [dbo].[FormDefinitions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormDefinitions] PRIMARY KEY,[Code] nvarchar(128) NOT NULL CONSTRAINT [UX_FormDefinitions_Code] UNIQUE,[Name] nvarchar(256) NOT NULL,[EntityType] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,[ActiveVersionId] uniqueidentifier NULL);
IF OBJECT_ID(N'[dbo].[FormVersions]', N'U') IS NULL CREATE TABLE [dbo].[FormVersions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormVersions] PRIMARY KEY,[FormDefinitionId] uniqueidentifier NOT NULL,[VersionNumber] int NOT NULL,[Status] nvarchar(64) NOT NULL,[PublishedAt] datetimeoffset NULL,[PublishedBy] nvarchar(max) NULL,CONSTRAINT [FK_FormVersions_FormDefinitions] FOREIGN KEY([FormDefinitionId]) REFERENCES [dbo].[FormDefinitions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_FormVersions_Definition_Version] UNIQUE([FormDefinitionId],[VersionNumber]));
IF OBJECT_ID(N'[dbo].[FormSections]', N'U') IS NULL CREATE TABLE [dbo].[FormSections]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSections] PRIMARY KEY,[FormVersionId] uniqueidentifier NOT NULL,[Code] nvarchar(128) NOT NULL,[Title] nvarchar(256) NOT NULL,[DisplayOrder] int NOT NULL,CONSTRAINT [FK_FormSections_FormVersions] FOREIGN KEY([FormVersionId]) REFERENCES [dbo].[FormVersions]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFields]', N'U') IS NULL CREATE TABLE [dbo].[FormFields]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFields] PRIMARY KEY,[FormSectionId] uniqueidentifier NOT NULL,[Code] nvarchar(128) NOT NULL,[Label] nvarchar(256) NOT NULL,[FieldType] nvarchar(64) NOT NULL,[DisplayOrder] int NOT NULL,[IsRequired] bit NOT NULL,CONSTRAINT [FK_FormFields_FormSections] FOREIGN KEY([FormSectionId]) REFERENCES [dbo].[FormSections]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFieldValidations]', N'U') IS NULL CREATE TABLE [dbo].[FormFieldValidations]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFieldValidations] PRIMARY KEY,[FormFieldId] uniqueidentifier NOT NULL,[ValidationType] nvarchar(128) NOT NULL,[ConfigurationJson] nvarchar(max) NULL,[Message] nvarchar(512) NOT NULL,CONSTRAINT [FK_FormFieldValidations_FormFields] FOREIGN KEY([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormFieldVisibilityRules]', N'U') IS NULL CREATE TABLE [dbo].[FormFieldVisibilityRules]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormFieldVisibilityRules] PRIMARY KEY,[FormFieldId] uniqueidentifier NOT NULL,[Expression] nvarchar(512) NOT NULL,CONSTRAINT [FK_FormFieldVisibilityRules_FormFields] FOREIGN KEY([FormFieldId]) REFERENCES [dbo].[FormFields]([Id]) ON DELETE CASCADE);
IF OBJECT_ID(N'[dbo].[FormSubmissions]', N'U') IS NULL CREATE TABLE [dbo].[FormSubmissions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSubmissions] PRIMARY KEY,[FormDefinitionId] uniqueidentifier NOT NULL,[FormVersionId] uniqueidentifier NOT NULL,[EntityType] nvarchar(128) NOT NULL,[EntityId] uniqueidentifier NOT NULL,[SubmittedBy] nvarchar(256) NOT NULL,[SubmittedAt] datetimeoffset NOT NULL);
IF OBJECT_ID(N'[dbo].[FormSubmissionValues]', N'U') IS NULL CREATE TABLE [dbo].[FormSubmissionValues]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_FormSubmissionValues] PRIMARY KEY,[FormSubmissionId] uniqueidentifier NOT NULL,[FieldCode] nvarchar(128) NOT NULL,[Value] nvarchar(max) NULL,CONSTRAINT [FK_FormSubmissionValues_FormSubmissions] FOREIGN KEY([FormSubmissionId]) REFERENCES [dbo].[FormSubmissions]([Id]) ON DELETE CASCADE);
");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in new[] { "FormSubmissionValues", "FormSubmissions", "FormFieldVisibilityRules", "FormFieldValidations", "FormFields", "FormSections", "FormVersions", "FormDefinitions", "WorkflowTransitionEffects", "WorkflowMappings" }) migrationBuilder.DropTable(table);
    }
}
