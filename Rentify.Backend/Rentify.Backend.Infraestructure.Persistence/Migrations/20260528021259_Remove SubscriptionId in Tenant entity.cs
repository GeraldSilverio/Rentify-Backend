using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubscriptionIdinTenantentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Subscriptions_CurrentSubscriptionId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CurrentSubscriptionId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CurrentSubscriptionId",
                table: "Tenants");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "RentCars",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RentCars");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentSubscriptionId",
                table: "Tenants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CurrentSubscriptionId",
                table: "Tenants",
                column: "CurrentSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Subscriptions_CurrentSubscriptionId",
                table: "Tenants",
                column: "CurrentSubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
