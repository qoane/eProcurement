using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowActionLegacyStepDefaults : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'FromStepCode') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'FromNodeCode') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowActions] SET [FromStepCode] = COALESCE(NULLIF([FromStepCode], N''''), [FromNodeCode], N'''') WHERE [FromStepCode] IS NULL OR [FromStepCode] = N''''');
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ToStepCode') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ToNodeCode') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowActions] SET [ToStepCode] = COALESCE(NULLIF([ToStepCode], N''''), [ToNodeCode], N'''') WHERE [ToStepCode] IS NULL OR [ToStepCode] = N''''');
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'FromStepCode') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowActions]'), N'FromStepCode', 'ColumnId')) ALTER TABLE [dbo].[WorkflowActions] ADD CONSTRAINT [DF_WorkflowActions_FromStepCode] DEFAULT N'' FOR [FromStepCode];
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'ToStepCode') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowActions]'), N'ToStepCode', 'ColumnId')) ALTER TABLE [dbo].[WorkflowActions] ADD CONSTRAINT [DF_WorkflowActions_ToStepCode] DEFAULT N'' FOR [ToStepCode];
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_WorkflowActions_FromStepCode]', N'D') IS NOT NULL ALTER TABLE [dbo].[WorkflowActions] DROP CONSTRAINT [DF_WorkflowActions_FromStepCode];
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_WorkflowActions_ToStepCode]', N'D') IS NOT NULL ALTER TABLE [dbo].[WorkflowActions] DROP CONSTRAINT [DF_WorkflowActions_ToStepCode];
");
    }
}
