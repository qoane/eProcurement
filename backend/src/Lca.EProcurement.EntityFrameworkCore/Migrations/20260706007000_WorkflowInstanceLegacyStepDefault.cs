using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowInstanceLegacyStepDefault : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'CurrentStepCode') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowInstances]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowInstances]'), N'CurrentStepCode', 'ColumnId'))
    ALTER TABLE [dbo].[WorkflowInstances] ADD CONSTRAINT [DF_WorkflowInstances_CurrentStepCode] DEFAULT N'' FOR [CurrentStepCode];
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL
BEGIN
    DECLARE @DropCurrentStepCodeDefaultSql nvarchar(max) = N'';
    SELECT @DropCurrentStepCodeDefaultSql = @DropCurrentStepCodeDefaultSql + N'ALTER TABLE [dbo].[WorkflowInstances] DROP CONSTRAINT [' + dc.name + N'];'
    FROM sys.default_constraints dc
    WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[WorkflowInstances]')
      AND dc.parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowInstances]'), N'CurrentStepCode', 'ColumnId');
    IF @DropCurrentStepCodeDefaultSql <> N'' EXEC sp_executesql @DropCurrentStepCodeDefaultSql;
END;
");
    }
}
