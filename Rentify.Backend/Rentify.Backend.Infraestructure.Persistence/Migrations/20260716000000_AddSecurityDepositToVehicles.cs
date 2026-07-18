using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations;

public partial class AddSecurityDepositToVehicles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "SecurityDepositAmount",
            table: "Vehicles",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<bool>(
            name: "SecurityDepositRequired",
            table: "Vehicles",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SecurityDepositAmount",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "SecurityDepositRequired",
            table: "Vehicles");
    }
}
