using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowActionLegacyActionDefault : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'Action') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowActions] SET [Action] = COALESCE(NULLIF([Action], N''''), [ActionCode], N'''') WHERE [Action] IS NULL OR [Action] = N''''');
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowActions]', N'Action') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActions]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowActions]'), N'Action', 'ColumnId')) ALTER TABLE [dbo].[WorkflowActions] ADD CONSTRAINT [DF_WorkflowActions_Action] DEFAULT N'' FOR [Action];
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowActions]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_WorkflowActions_Action]', N'D') IS NOT NULL ALTER TABLE [dbo].[WorkflowActions] DROP CONSTRAINT [DF_WorkflowActions_Action];
");
    }
}
