using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsApptoTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_PhoneNumber",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "WhatsApp",
                table: "Tenants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_WhatsApp",
                table: "Tenants",
                column: "WhatsApp",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_WhatsApp",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "WhatsApp",
                table: "Tenants");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_PhoneNumber",
                table: "Tenants",
                column: "PhoneNumber",
                unique: true);
        }
    }
}
