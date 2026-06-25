using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorizandoTenantentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Tenants");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId1",
                table: "TenantSettings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "BusinessModel",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                table: "Tenants",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rnc",
                table: "Tenants",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId1",
                table: "TenantSettings",
                column: "TenantId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Rnc",
                table: "Tenants",
                column: "Rnc",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId1",
                table: "TenantSettings",
                column: "TenantId1",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId1",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_TenantSettings_TenantId1",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Rnc",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "BusinessModel",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LegalName",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Rnc",
                table: "Tenants");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
