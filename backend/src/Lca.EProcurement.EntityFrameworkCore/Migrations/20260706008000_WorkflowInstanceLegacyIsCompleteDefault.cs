using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

public partial class WorkflowInstanceLegacyIsCompleteDefault : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[WorkflowInstances]', N'IsComplete') IS NOT NULL
BEGIN
    EXEC(N'UPDATE [dbo].[WorkflowInstances] SET [IsComplete] = CONVERT(bit, CASE WHEN [Status] = N''Completed'' THEN 1 ELSE 0 END) WHERE [IsComplete] IS NULL');

    IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[WorkflowInstances]') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowInstances]'), N'IsComplete', 'ColumnId'))
        ALTER TABLE [dbo].[WorkflowInstances] ADD CONSTRAINT [DF_WorkflowInstances_IsComplete] DEFAULT CONVERT(bit, 0) FOR [IsComplete];
END;
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', N'U') IS NOT NULL
BEGIN
    DECLARE @DropIsCompleteDefaultSql nvarchar(max) = N'';
    SELECT @DropIsCompleteDefaultSql = @DropIsCompleteDefaultSql + N'ALTER TABLE [dbo].[WorkflowInstances] DROP CONSTRAINT [' + dc.name + N'];'
    FROM sys.default_constraints dc
    WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[WorkflowInstances]')
      AND dc.parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[WorkflowInstances]'), N'IsComplete', 'ColumnId');
    IF @DropIsCompleteDefaultSql <> N'' EXEC sp_executesql @DropIsCompleteDefaultSql;
END;
");
    }
}
