using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations
{
    public partial class IdentitySecurityTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "UserMfaSettings", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                PreferredMethod = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                AuthenticatorSecretEncrypted = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_UserMfaSettings", x => x.Id));

            migrationBuilder.CreateTable(name: "UserMfaChallenges", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Method = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                CodeHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ConsumedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                AttemptCount = table.Column<int>(type: "int", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_UserMfaChallenges", x => x.Id));

            migrationBuilder.CreateTable(name: "TrustedDevices", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DeviceHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                TrustedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_TrustedDevices", x => x.Id));

            migrationBuilder.CreateTable(name: "IdentityProviderConfigurations", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ProviderType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Authority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClientSecretEncrypted = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                MetadataUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CallbackPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                SettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_IdentityProviderConfigurations", x => x.Id));

            migrationBuilder.CreateTable(name: "ExternalIdentityLinks", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProviderCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ExternalSubjectId = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                ExternalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LinkedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                LastLoginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            }, constraints: table => table.PrimaryKey("PK_ExternalIdentityLinks", x => x.Id));

            migrationBuilder.CreateTable(name: "DelegationRules", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DelegatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DelegateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                StartsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                EndsAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
            }, constraints: table => table.PrimaryKey("PK_DelegationRules", x => x.Id));

            migrationBuilder.CreateTable(name: "EscalationRules", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EntityType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                WorkflowCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                NodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                AssignedRole = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                EscalateAfterHours = table.Column<int>(type: "int", nullable: false),
                EscalateToRole = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                EscalateToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_EscalationRules", x => x.Id));

            migrationBuilder.CreateTable(name: "WorkflowTaskEscalations", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                WorkflowTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EscalatedFromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                EscalatedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                EscalatedToRole = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                EscalatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_WorkflowTaskEscalations", x => x.Id));

            migrationBuilder.CreateIndex(name: "IX_UserMfaSettings_UserId", table: "UserMfaSettings", column: "UserId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_TrustedDevices_UserId_DeviceHash", table: "TrustedDevices", columns: new[] { "UserId", "DeviceHash" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_IdentityProviderConfigurations_Code", table: "IdentityProviderConfigurations", column: "Code", unique: true);
            migrationBuilder.CreateIndex(name: "IX_ExternalIdentityLinks_ProviderCode_ExternalSubjectId", table: "ExternalIdentityLinks", columns: new[] { "ProviderCode", "ExternalSubjectId" }, unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WorkflowTaskEscalations");
            migrationBuilder.DropTable(name: "EscalationRules");
            migrationBuilder.DropTable(name: "DelegationRules");
            migrationBuilder.DropTable(name: "ExternalIdentityLinks");
            migrationBuilder.DropTable(name: "IdentityProviderConfigurations");
            migrationBuilder.DropTable(name: "TrustedDevices");
            migrationBuilder.DropTable(name: "UserMfaChallenges");
            migrationBuilder.DropTable(name: "UserMfaSettings");
        }
    }
}
