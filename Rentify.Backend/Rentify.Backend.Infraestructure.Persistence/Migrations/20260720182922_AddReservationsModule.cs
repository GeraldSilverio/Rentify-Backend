using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Vehicles_VehicleId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReturnLocation",
                table: "Reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitRate",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<bool>(
                name: "SecurityDepositRequired",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<decimal>(
                name: "SecurityDepositAmount",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "RentalAmount",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Reservations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Reservations",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Reservations",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                table: "Reservations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConvertedToRentalAt",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConvertedToRentalBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddressDetails",
                table: "Reservations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocationName",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryTenantLocationId",
                table: "Reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddressDetails",
                table: "Reservations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnFee",
                table: "Reservations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReturnLocationName",
                table: "Reservations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ReturnTenantLocationId",
                table: "Reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DeliveryTenantLocationId",
                table: "Reservations",
                column: "DeliveryTenantLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReturnTenantLocationId",
                table: "Reservations",
                column: "ReturnTenantLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_Code",
                table: "Reservations",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_CustomerId",
                table: "Reservations",
                columns: new[] { "TenantId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_DeliveryDateTime",
                table: "Reservations",
                columns: new[] { "TenantId", "DeliveryDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_ExpectedReturnDateTime",
                table: "Reservations",
                columns: new[] { "TenantId", "ExpectedReturnDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_Status",
                table: "Reservations",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_VehicleId",
                table: "Reservations",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TenantId_VehicleId_DeliveryDateTime_ExpectedRe~",
                table: "Reservations",
                columns: new[] { "TenantId", "VehicleId", "DeliveryDateTime", "ExpectedReturnDateTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Vehicles_VehicleId",
                table: "Reservations",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Vehicles_VehicleId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_DeliveryTenantLocationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReturnTenantLocationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_Code",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_CustomerId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_DeliveryDateTime",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_ExpectedReturnDateTime",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_Status",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_VehicleId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TenantId_VehicleId_DeliveryDateTime_ExpectedRe~",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConvertedToRentalAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConvertedToRentalBy",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressDetails",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryLocationName",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryTenantLocationId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReturnAddressDetails",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReturnFee",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReturnLocationName",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReturnTenantLocationId",
                table: "Reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitRate",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "SecurityDepositRequired",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "SecurityDepositAmount",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "RentalAmount",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Reservations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Reservations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Reservations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReturnLocation",
                table: "Reservations",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Vehicles_VehicleId",
                table: "Reservations",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
