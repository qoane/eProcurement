using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Lca.EProcurement.EntityFrameworkCore.Migrations;
public partial class IdentitySecurity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[ApplicationUsers]', N'U') IS NULL CREATE TABLE [dbo].[ApplicationUsers]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_ApplicationUsers] PRIMARY KEY,[Email] nvarchar(256) NOT NULL,[FullName] nvarchar(256) NOT NULL,[PhoneNumber] nvarchar(64) NULL,[UserType] nvarchar(64) NOT NULL,[IsActive] bit NOT NULL,[IsExternalUser] bit NOT NULL,[SupplierId] uniqueidentifier NULL,[CreatedAt] datetimeoffset NOT NULL,[LastLoginAt] datetimeoffset NULL,[PasswordHash] nvarchar(1024) NOT NULL,CONSTRAINT [UX_ApplicationUsers_Email] UNIQUE([Email]));
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NULL CREATE TABLE [dbo].[Roles]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_Roles] PRIMARY KEY,[Name] nvarchar(128) NOT NULL,[Description] nvarchar(512) NOT NULL,[IsActive] bit NOT NULL,CONSTRAINT [UX_Roles_Name] UNIQUE([Name]));
IF OBJECT_ID(N'[dbo].[Permissions]', N'U') IS NULL CREATE TABLE [dbo].[Permissions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_Permissions] PRIMARY KEY,[Code] nvarchar(128) NOT NULL,[Name] nvarchar(256) NOT NULL,[Description] nvarchar(512) NOT NULL,[Category] nvarchar(128) NOT NULL,[IsActive] bit NOT NULL,CONSTRAINT [UX_Permissions_Code] UNIQUE([Code]));
IF OBJECT_ID(N'[dbo].[RolePermissions]', N'U') IS NULL CREATE TABLE [dbo].[RolePermissions]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_RolePermissions] PRIMARY KEY,[RoleId] uniqueidentifier NOT NULL,[PermissionId] uniqueidentifier NOT NULL,CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_RolePermissions_RoleId_PermissionId] UNIQUE([RoleId],[PermissionId]));
IF OBJECT_ID(N'[dbo].[UserRoles]', N'U') IS NULL CREATE TABLE [dbo].[UserRoles]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_UserRoles] PRIMARY KEY,[UserId] uniqueidentifier NOT NULL,[RoleId] uniqueidentifier NOT NULL,CONSTRAINT [FK_UserRoles_ApplicationUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[ApplicationUsers]([Id]) ON DELETE CASCADE,CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_UserRoles_UserId_RoleId] UNIQUE([UserId],[RoleId]));
IF OBJECT_ID(N'[dbo].[UserProfiles]', N'U') IS NULL CREATE TABLE [dbo].[UserProfiles]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_UserProfiles] PRIMARY KEY,[UserId] uniqueidentifier NOT NULL,[Department] nvarchar(128) NOT NULL,[JobTitle] nvarchar(128) NOT NULL,[PreferencesJson] nvarchar(max) NOT NULL,CONSTRAINT [FK_UserProfiles_ApplicationUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[ApplicationUsers]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_UserProfiles_UserId] UNIQUE([UserId]));
IF OBJECT_ID(N'[dbo].[SupplierUserLinks]', N'U') IS NULL CREATE TABLE [dbo].[SupplierUserLinks]([Id] uniqueidentifier NOT NULL CONSTRAINT [PK_SupplierUserLinks] PRIMARY KEY,[UserId] uniqueidentifier NOT NULL,[SupplierId] uniqueidentifier NOT NULL,[IsPrimaryContact] bit NOT NULL,[LinkedAt] datetimeoffset NOT NULL,CONSTRAINT [FK_SupplierUserLinks_ApplicationUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[ApplicationUsers]([Id]) ON DELETE CASCADE,CONSTRAINT [UX_SupplierUserLinks_UserId_SupplierId] UNIQUE([UserId],[SupplierId]));
");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS [dbo].[SupplierUserLinks];DROP TABLE IF EXISTS [dbo].[UserProfiles];DROP TABLE IF EXISTS [dbo].[UserRoles];DROP TABLE IF EXISTS [dbo].[RolePermissions];DROP TABLE IF EXISTS [dbo].[Permissions];DROP TABLE IF EXISTS [dbo].[Roles];DROP TABLE IF EXISTS [dbo].[ApplicationUsers];");
    }
}
