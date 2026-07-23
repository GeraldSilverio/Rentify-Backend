using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceVehicleFeatureAssignmentUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleFeatureAssignments_TenantId_VehicleId_VehicleFeature~",
                table: "VehicleFeatureAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatureAssignments_TenantId_VehicleId_VehicleFeature~",
                table: "VehicleFeatureAssignments",
                columns: new[] { "TenantId", "VehicleId", "VehicleFeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleFeatureAssignments_TenantId_VehicleId_VehicleFeature~",
                table: "VehicleFeatureAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFeatureAssignments_TenantId_VehicleId_VehicleFeature~",
                table: "VehicleFeatureAssignments",
                columns: new[] { "TenantId", "VehicleId", "VehicleFeatureId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
