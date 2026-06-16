using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVehiclesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Models_ModelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Vin",
                table: "Vehicles");

            migrationBuilder.CreateTable(
                name: "VehicleBrands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleBrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleModels_VehicleBrands_VehicleBrandId",
                        column: x => x.VehicleBrandId,
                        principalTable: "VehicleBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO "VehicleBrands" ("Id", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT "Id", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive"
                FROM "Brands";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "VehicleModels" ("Id", "VehicleBrandId", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT "Id", "BrandId", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive"
                FROM "Models";
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "VehicleUnavailableDates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleModelId",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleTypeId",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Vehicles" AS v
                SET "TenantId" = rc."TenantId",
                    "VehicleModelId" = v."ModelId"
                FROM "RentCars" AS rc
                WHERE rc."Id" = v."RentCarId";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "VehicleTypes" ("Id", "TenantId", "Name", "Description", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT DISTINCT
                    (substr(md5('default-vehicle-type:' || v."TenantId"::text), 1, 8) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 9, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 13, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 17, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 21, 12))::uuid,
                    v."TenantId",
                    'Default',
                    'Migrated default vehicle type',
                    'migration',
                    'migration',
                    NOW(),
                    NOW(),
                    false,
                    true
                FROM "Vehicles" AS v
                WHERE v."TenantId" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE "Vehicles" AS v
                SET "VehicleTypeId" =
                    (substr(md5('default-vehicle-type:' || v."TenantId"::text), 1, 8) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 9, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 13, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 17, 4) || '-' ||
                     substr(md5('default-vehicle-type:' || v."TenantId"::text), 21, 12))::uuid
                WHERE v."TenantId" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE "VehicleUnavailableDates" AS unavailable
                SET "TenantId" = v."TenantId"
                FROM "Vehicles" AS v
                WHERE v."Id" = unavailable."VehicleId";
                """);

            migrationBuilder.CreateTable(
                name: "VehicleImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PublicId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleImages_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO "VehicleImages" ("Id", "TenantId", "VehicleId", "Url", "PublicId", "IsPrimary", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT
                    (substr(md5('vehicle-image:' || v."Id"::text), 1, 8) || '-' ||
                     substr(md5('vehicle-image:' || v."Id"::text), 9, 4) || '-' ||
                     substr(md5('vehicle-image:' || v."Id"::text), 13, 4) || '-' ||
                     substr(md5('vehicle-image:' || v."Id"::text), 17, 4) || '-' ||
                     substr(md5('vehicle-image:' || v."Id"::text), 21, 12))::uuid,
                    v."TenantId",
                    v."Id",
                    v."ImageUrl",
                    COALESCE(NULLIF(v."ImagePublicId", ''), 'legacy/vehicles/' || v."Id"::text),
                    true,
                    v."CreatedBy",
                    v."ModifiedBy",
                    v."CreatedDate",
                    v."ModifiedDate",
                    false,
                    true
                FROM "Vehicles" AS v
                WHERE v."TenantId" IS NOT NULL
                  AND NULLIF(v."ImageUrl", '') IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "VehicleUnavailableDates",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleModelId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleTypeId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleUnavailableDates_TenantId_VehicleId",
                table: "VehicleUnavailableDates",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles",
                columns: new[] { "TenantId", "PlateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_Status",
                table: "Vehicles",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles",
                columns: new[] { "TenantId", "Vin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_PublicId",
                table: "VehicleImages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_TenantId_VehicleId",
                table: "VehicleImages",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId",
                table: "VehicleImages",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels",
                columns: new[] { "VehicleBrandId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_TenantId_Name",
                table: "VehicleTypes",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleImages");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropTable(
                name: "VehicleBrands");

            migrationBuilder.DropIndex(
                name: "IX_VehicleUnavailableDates_TenantId_VehicleId",
                table: "VehicleUnavailableDates");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_PlateNumber",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_Status",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId_Vin",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VehicleUnavailableDates");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Vehicles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Vehicles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Vin",
                table: "Vehicles",
                column: "Vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_BrandId_Name",
                table: "Models",
                columns: new[] { "BrandId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Models_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
