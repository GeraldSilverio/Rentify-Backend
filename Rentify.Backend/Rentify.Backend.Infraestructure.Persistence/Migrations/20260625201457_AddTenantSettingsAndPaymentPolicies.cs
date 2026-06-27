using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantSettingsAndPaymentPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId1",
                table: "TenantSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantSettings",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_TenantSettings_TenantId1",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "TenantSettings");

            migrationBuilder.RenameColumn(
                name: "TenantId1",
                table: "TenantSettings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "TenantSettings",
                newName: "CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "EnableSmsNotifications",
                table: "TenantSettings",
                newName: "EnableReservations");

            migrationBuilder.RenameColumn(
                name: "EnableNotifications",
                table: "TenantSettings",
                newName: "EnablePublicCatalog");

            migrationBuilder.RenameColumn(
                name: "EnableEmailNotifications",
                table: "TenantSettings",
                newName: "EnableMaintenance");

            migrationBuilder.RenameColumn(
                name: "AllowOnlineReservations",
                table: "TenantSettings",
                newName: "EnableLateFees");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "TenantSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "TenantSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "EnableDriverFleet",
                table: "TenantSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantSettings",
                table: "TenantSettings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PaymentPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PaymentFrequency = table.Column<int>(type: "integer", nullable: false),
                    CutoffDayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    GraceDays = table.Column<int>(type: "integer", nullable: false),
                    ReminderStartDayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    LateFeeEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentPolicies_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentPolicies_TenantId",
                table: "PaymentPolicies",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings");

            migrationBuilder.DropTable(
                name: "PaymentPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantSettings",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "EnableDriverFleet",
                table: "TenantSettings");

            migrationBuilder.RenameColumn(
                name: "EnableReservations",
                table: "TenantSettings",
                newName: "EnableSmsNotifications");

            migrationBuilder.RenameColumn(
                name: "EnablePublicCatalog",
                table: "TenantSettings",
                newName: "EnableNotifications");

            migrationBuilder.RenameColumn(
                name: "EnableMaintenance",
                table: "TenantSettings",
                newName: "EnableEmailNotifications");

            migrationBuilder.RenameColumn(
                name: "EnableLateFees",
                table: "TenantSettings",
                newName: "AllowOnlineReservations");

            migrationBuilder.RenameColumn(
                name: "CurrencyCode",
                table: "TenantSettings",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TenantSettings",
                newName: "TenantId1");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "TenantSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "TenantSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "TenantSettings",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "TenantSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "TenantSettings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "TenantSettings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantSettings",
                table: "TenantSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId1",
                table: "TenantSettings",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId1",
                table: "TenantSettings",
                column: "TenantId1",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
