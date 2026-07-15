using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations
{
    public partial class SupplierAccountLifecycleFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Suppliers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Suppliers]', N'AccountState') IS NULL ALTER TABLE [dbo].[Suppliers] ADD [AccountState] nvarchar(64) NOT NULL CONSTRAINT [DF_Suppliers_AccountState] DEFAULT N'PendingVerification';
IF OBJECT_ID(N'[dbo].[Suppliers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Suppliers]', N'EmailVerified') IS NULL ALTER TABLE [dbo].[Suppliers] ADD [EmailVerified] bit NOT NULL CONSTRAINT [DF_Suppliers_EmailVerified] DEFAULT CONVERT(bit, 0);
IF OBJECT_ID(N'[dbo].[Suppliers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Suppliers]', N'ProfileComplete') IS NULL ALTER TABLE [dbo].[Suppliers] ADD [ProfileComplete] bit NOT NULL CONSTRAINT [DF_Suppliers_ProfileComplete] DEFAULT CONVERT(bit, 0);
IF OBJECT_ID(N'[dbo].[Suppliers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Suppliers]', N'AccountState') IS NOT NULL EXEC(N'UPDATE [dbo].[Suppliers] SET [AccountState] = CASE WHEN [Status] = N''Approved'' THEN N''Approved'' ELSE [AccountState] END');
IF OBJECT_ID(N'[dbo].[Suppliers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Suppliers]', N'EmailVerified') IS NOT NULL EXEC(N'UPDATE [dbo].[Suppliers] SET [EmailVerified] = CONVERT(bit, 1), [ProfileComplete] = CONVERT(bit, 1) WHERE [Status] = N''Approved''');
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ProfileComplete", table: "Suppliers");
            migrationBuilder.DropColumn(name: "EmailVerified", table: "Suppliers");
            migrationBuilder.DropColumn(name: "AccountState", table: "Suppliers");
        }
    }
}
