using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations
{
    public partial class ApplicationUserSecurityFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'FailedLoginCount') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [FailedLoginCount] int NOT NULL CONSTRAINT [DF_ApplicationUsers_FailedLoginCount] DEFAULT 0;
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'LockoutEnd') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [LockoutEnd] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'EmailConfirmed') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [EmailConfirmed] bit NOT NULL CONSTRAINT [DF_ApplicationUsers_EmailConfirmed] DEFAULT CONVERT(bit, 0);
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'PhoneNumberConfirmed') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [PhoneNumberConfirmed] bit NOT NULL CONSTRAINT [DF_ApplicationUsers_PhoneNumberConfirmed] DEFAULT CONVERT(bit, 0);
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'LastPasswordChangedAt') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [LastPasswordChangedAt] datetimeoffset NULL;
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[ApplicationUsers]', N'MustChangePassword') IS NULL ALTER TABLE [dbo].[ApplicationUsers] ADD [MustChangePassword] bit NOT NULL CONSTRAINT [DF_ApplicationUsers_MustChangePassword] DEFAULT CONVERT(bit, 0);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "MustChangePassword", table: "ApplicationUsers");
            migrationBuilder.DropColumn(name: "LastPasswordChangedAt", table: "ApplicationUsers");
            migrationBuilder.DropColumn(name: "PhoneNumberConfirmed", table: "ApplicationUsers");
            migrationBuilder.DropColumn(name: "EmailConfirmed", table: "ApplicationUsers");
            migrationBuilder.DropColumn(name: "LockoutEnd", table: "ApplicationUsers");
            migrationBuilder.DropColumn(name: "FailedLoginCount", table: "ApplicationUsers");
        }
    }
}
