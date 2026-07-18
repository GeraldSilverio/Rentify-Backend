using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncCustomerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                table: "Customers",
                type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "Regular");

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "Customers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumberNormalized",
                table: "Customers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdentificationType",
                table: "Customers",
                type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "Cedula");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentSide",
                table: "CustomerDocuments",
                type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "NotApplicable");

        migrationBuilder.Sql(
            "UPDATE \"Customers\" " +
            "SET \"IdentificationNumber\" = 'LEGACY-' || substring(\"Id\"::text, 1, 22), " +
            "\"IdentificationNumberNormalized\" = 'LEGACY-' || substring(\"Id\"::text, 1, 22) " +
            "WHERE \"IdentificationNumber\" = '';" );

        migrationBuilder.AlterColumn<string>(
            name: "IdentificationNumber",
            table: "Customers",
            type: "character varying(30)",
            maxLength: 30,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(30)",
            oldMaxLength: 30,
            oldDefaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "IdentificationNumberNormalized",
            table: "Customers",
            type: "character varying(30)",
            maxLength: 30,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(30)",
            oldMaxLength: 30,
            oldDefaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_IdentificationType_IdentificationNumberN~",
                table: "Customers",
                columns: new[] { "TenantId", "IdentificationType", "IdentificationNumberNormalized" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_IdentificationType_IdentificationNumberN~",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IdentificationNumberNormalized",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IdentificationType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DocumentSide",
                table: "CustomerDocuments");
        }
    }
}
