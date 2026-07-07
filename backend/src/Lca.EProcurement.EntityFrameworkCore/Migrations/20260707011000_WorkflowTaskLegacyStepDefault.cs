using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowTaskLegacyStepDefault : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'StepCode') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'NodeCode') IS NOT NULL EXEC(N'UPDATE [dbo].[WorkflowTasks] SET [StepCode] = COALESCE(NULLIF([StepCode], N''''), [NodeCode], N'''') WHERE [StepCode] IS NULL OR [StepCode] = N'''');
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[WorkflowTasks]', N'StepCode') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowTasks]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowTasks]'), N'StepCode', 'ColumnId')) ALTER TABLE [dbo].[WorkflowTasks] ADD CONSTRAINT [DF_WorkflowTasks_StepCode] DEFAULT N'' FOR [StepCode];
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowTasks]', N'U') IS NOT NULL AND OBJECT_ID(N'[dbo].[DF_WorkflowTasks_StepCode]', N'D') IS NOT NULL ALTER TABLE [dbo].[WorkflowTasks] DROP CONSTRAINT [DF_WorkflowTasks_StepCode];
");
    }
}
