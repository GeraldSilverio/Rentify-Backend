using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeVehiclesAndUseCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleImages");

            migrationBuilder.CreateTable(
                name: "Brands",
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
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoPublicId",
                table: "RentCars",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO "Brands" ("Id", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT DISTINCT
                    (substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 1, 8) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 9, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 13, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 17, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 21, 12))::uuid,
                    COALESCE(NULLIF(btrim("Make"), ''), 'Unknown'),
                    'migration',
                    'migration',
                    NOW(),
                    NOW(),
                    false,
                    true
                FROM "Vehicles";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "Models" ("Id", "BrandId", "Name", "CreatedBy", "ModifiedBy", "CreatedDate", "ModifiedDate", "IsDeleted", "IsActive")
                SELECT DISTINCT
                    (substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 1, 8) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 9, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 13, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 17, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 21, 12))::uuid,
                    (substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 1, 8) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 9, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 13, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 17, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown')), 21, 12))::uuid,
                    COALESCE(NULLIF(btrim("Model"), ''), 'Unknown'),
                    'migration',
                    'migration',
                    NOW(),
                    NOW(),
                    false,
                    true
                FROM "Vehicles";
                """);

            migrationBuilder.Sql("""
                UPDATE "Vehicles"
                SET "ModelId" =
                    (substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 1, 8) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 9, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 13, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 17, 4) || '-' ||
                     substr(md5(COALESCE(NULLIF(btrim("Make"), ''), 'Unknown') || ':' || COALESCE(NULLIF(btrim("Model"), ''), 'Unknown')), 21, 12))::uuid;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles",
                column: "ModelId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Models_ModelId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LogoPublicId",
                table: "RentCars");

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "VehicleImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId_IsPrimary",
                table: "VehicleImages",
                columns: new[] { "VehicleId", "IsPrimary" });
        }
    }
}
