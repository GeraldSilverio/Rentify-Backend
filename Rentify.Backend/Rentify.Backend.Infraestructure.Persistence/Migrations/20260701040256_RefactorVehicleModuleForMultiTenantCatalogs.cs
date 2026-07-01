using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVehicleModuleForMultiTenantCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels");

            migrationBuilder.DropIndex(
                name: "IX_VehicleImages_VehicleId",
                table: "VehicleImages");

            migrationBuilder.DropIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands");

            migrationBuilder.AlterColumn<string>(
                name: "Vin",
                table: "Vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(17)",
                oldMaxLength: 17);

            migrationBuilder.Sql("""
                ALTER TABLE "Vehicles"
                ALTER COLUMN "Status" TYPE integer
                USING CASE "Status"
                    WHEN 'Available' THEN 1
                    WHEN 'Reserved' THEN 2
                    WHEN 'Rented' THEN 3
                    WHEN 'Maintenance' THEN 4
                    WHEN 'Unavailable' THEN 5
                    WHEN 'OutOfService' THEN 5
                    ELSE 1
                END;
                """);

            migrationBuilder.AddColumn<int>(
                name: "CurrentMileage",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleBrandId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("""
                UPDATE "Vehicles" AS vehicle
                SET "VehicleBrandId" = model."VehicleBrandId"
                FROM "VehicleModels" AS model
                WHERE vehicle."VehicleModelId" = model."Id";
                """);

            migrationBuilder.CreateTable(
                name: "VehicleFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RentalType = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleRates_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""CREATE EXTENSION IF NOT EXISTS pgcrypto;""");

            migrationBuilder.Sql("""
                INSERT INTO "VehicleRates" (
                    "Id",
                    "TenantId",
                    "VehicleId",
                    "RentalType",
                    "Price",
                    "CreatedBy",
                    "ModifiedBy",
                    "CreatedDate",
                    "ModifiedDate",
                    "IsDeleted",
                    "IsActive")
                SELECT
                    gen_random_uuid(),
                    vehicle."TenantId",
                    vehicle."Id",
                    1,
                    vehicle."DailyRate",
                    vehicle."CreatedBy",
                    vehicle."ModifiedBy",
                    vehicle."CreatedDate",
                    vehicle."ModifiedDate",
                    false,
                    true
                FROM "Vehicles" AS vehicle
                WHERE vehicle."DailyRate" > 0;
                """);

            migrationBuilder.DropColumn(
                name: "DailyRate",
                table: "Vehicles");

            migrationBuilder.CreateTable(
                name: "VehicleFeatureAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFeatureAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleFeatureAssignments_VehicleFeatures_VehicleFeatureId",
                        column: x => x.VehicleFeatureId,
                        principalTable: "VehicleFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleFeatureAssignments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_Name",
                table: "VehicleTypes",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles",
                columns: new[] { "TenantId", "PlateNumber" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_VehicleBrandId",
                table: "Vehicles",
                columns: new[] { "TenantId", "VehicleBrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_VehicleModelId",
                table: "Vehicles",
                columns: new[] { "TenantId", "VehicleModelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_VehicleTypeId",
                table: "Vehicles",
                columns: new[] { "TenantId", "VehicleTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles",
                columns: new[] { "TenantId", "Vin" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Vin\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleBrandId",
                table: "Vehicles",
                column: "VehicleBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels",
                columns: new[] { "VehicleBrandId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId_IsPrimary",
                table: "VehicleImages",
                columns: new[] { "VehicleId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatureAssignments_TenantId_VehicleId_VehicleFeature~",
                table: "VehicleFeatureAssignments",
                columns: new[] { "TenantId", "VehicleId", "VehicleFeatureId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatureAssignments_VehicleFeatureId",
                table: "VehicleFeatureAssignments",
                column: "VehicleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatureAssignments_VehicleId",
                table: "VehicleFeatureAssignments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatures_Name",
                table: "VehicleFeatures",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRates_TenantId_VehicleId_RentalType",
                table: "VehicleRates",
                columns: new[] { "TenantId", "VehicleId", "RentalType" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRates_VehicleId",
                table: "VehicleRates",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleBrands_VehicleBrandId",
                table: "Vehicles",
                column: "VehicleBrandId",
                principalTable: "VehicleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleBrands_VehicleBrandId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleFeatureAssignments");

            migrationBuilder.DropTable(
                name: "VehicleRates");

            migrationBuilder.DropTable(
                name: "VehicleFeatures");

            migrationBuilder.DropIndex(
                name: "IX_VehicleTypes_Name",
                table: "VehicleTypes");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_VehicleBrandId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleBrandId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels");

            migrationBuilder.DropIndex(
                name: "IX_VehicleImages_VehicleId_IsPrimary",
                table: "VehicleImages");

            migrationBuilder.DropIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands");

            migrationBuilder.DropColumn(
                name: "CurrentMileage",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleBrandId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<string>(
                name: "Vin",
                table: "Vehicles",
                type: "character varying(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.Sql("""
                ALTER TABLE "Vehicles"
                ALTER COLUMN "Status" TYPE character varying(30)
                USING CASE "Status"
                    WHEN 1 THEN 'Available'
                    WHEN 2 THEN 'Reserved'
                    WHEN 3 THEN 'Rented'
                    WHEN 4 THEN 'Maintenance'
                    WHEN 5 THEN 'OutOfService'
                    ELSE 'Available'
                END;
                """);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyRate",
                table: "Vehicles",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles",
                columns: new[] { "TenantId", "PlateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles",
                columns: new[] { "TenantId", "Vin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels",
                columns: new[] { "VehicleBrandId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId",
                table: "VehicleImages",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands",
                column: "Name",
                unique: true);
        }
    }
}
