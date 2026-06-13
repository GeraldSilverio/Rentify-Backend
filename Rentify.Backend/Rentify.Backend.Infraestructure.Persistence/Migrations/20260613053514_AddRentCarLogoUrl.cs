using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRentCarLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "RentCars",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "RentCars");
        }
    }
}
