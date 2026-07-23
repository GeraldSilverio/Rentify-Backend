using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.Backend.Infrastructure.Persistence.Migrations;

public partial class RebuildReservationsModule : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DO $$ DECLARE item record; BEGIN FOR item IN SELECT conrelid::regclass AS table_name, conname FROM pg_constraint WHERE contype = 'f' AND confrelid = to_regclass('public."Reservations"') LOOP EXECUTE format('ALTER TABLE %s DROP CONSTRAINT IF EXISTS %I', item.table_name, item.conname); END LOOP; END $$;""");
        migrationBuilder.Sql("DROP TABLE IF EXISTS public.\"ReservationPayments\" CASCADE;");
        migrationBuilder.Sql("DROP TABLE IF EXISTS public.\"ReservationVehicles\" CASCADE;");
        migrationBuilder.Sql("DROP TABLE IF EXISTS public.\"Reservations\" CASCADE;");
        migrationBuilder.CreateTable(
            name: "Reservations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false), TenantId = table.Column<Guid>(type: "uuid", nullable: false), Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false), CustomerId = table.Column<Guid>(type: "uuid", nullable: false), VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                DeliveryDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), ExpectedReturnDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), RentalType = table.Column<int>(type: "integer", nullable: false), Quantity = table.Column<int>(type: "integer", nullable: false), UnitRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false), RentalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                SecurityDepositRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false), SecurityDepositAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m), DeliveryTenantLocationId = table.Column<Guid>(type: "uuid", nullable: true), DeliveryLocationName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false), DeliveryAddressDetails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true), DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                ReturnTenantLocationId = table.Column<Guid>(type: "uuid", nullable: true), ReturnLocationName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false), ReturnAddressDetails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true), ReturnFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m), DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m), TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false), Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false), Channel = table.Column<int>(type: "integer", nullable: false), ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), ApprovedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true), RejectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true), RejectedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true), CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true), CancelledBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true), ConvertedToRentalAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), ConvertedToRentalBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false), ModifiedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false), CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), IsDeleted = table.Column<bool>(type: "boolean", nullable: false), IsActive = table.Column<bool>(type: "boolean", nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Reservations", x => x.Id);
                table.ForeignKey("FK_Reservations_Customers_CustomerId", x => x.CustomerId, "Customers", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Reservations_Vehicles_VehicleId", x => x.VehicleId, "Vehicles", "Id", onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex("IX_Reservations_DeliveryTenantLocationId", "Reservations", "DeliveryTenantLocationId");
        migrationBuilder.CreateIndex("IX_Reservations_ReturnTenantLocationId", "Reservations", "ReturnTenantLocationId");
        migrationBuilder.CreateIndex("IX_Reservations_TenantId_Code", "Reservations", new[] { "TenantId", "Code" }, unique: true, filter: "\"IsDeleted\" = false");
        migrationBuilder.CreateIndex("IX_Reservations_TenantId_CustomerId", "Reservations", new[] { "TenantId", "CustomerId" }); migrationBuilder.CreateIndex("IX_Reservations_TenantId_VehicleId", "Reservations", new[] { "TenantId", "VehicleId" }); migrationBuilder.CreateIndex("IX_Reservations_TenantId_Status", "Reservations", new[] { "TenantId", "Status" }); migrationBuilder.CreateIndex("IX_Reservations_TenantId_DeliveryDateTime", "Reservations", new[] { "TenantId", "DeliveryDateTime" }); migrationBuilder.CreateIndex("IX_Reservations_TenantId_ExpectedReturnDateTime", "Reservations", new[] { "TenantId", "ExpectedReturnDateTime" }); migrationBuilder.CreateIndex("IX_Reservations_TenantId_VehicleId_DeliveryDateTime_ExpectedReturnDateTime", "Reservations", new[] { "TenantId", "VehicleId", "DeliveryDateTime", "ExpectedReturnDateTime" });
        migrationBuilder.Sql("""DO $$ BEGIN IF to_regclass('public."Payments"') IS NOT NULL AND EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND table_name = 'Payments' AND column_name = 'ReservationId') THEN ALTER TABLE public."Payments" ADD CONSTRAINT "FK_Payments_Reservations_ReservationId" FOREIGN KEY ("ReservationId") REFERENCES public."Reservations" ("Id") ON DELETE RESTRICT; END IF; IF to_regclass('public."Invoices"') IS NOT NULL AND EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND table_name = 'Invoices' AND column_name = 'ReservationId') THEN ALTER TABLE public."Invoices" ADD CONSTRAINT "FK_Invoices_Reservations_ReservationId" FOREIGN KEY ("ReservationId") REFERENCES public."Reservations" ("Id") ON DELETE RESTRICT; END IF; END $$;""");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DO $$ DECLARE item record; BEGIN FOR item IN SELECT conrelid::regclass AS table_name, conname FROM pg_constraint WHERE contype = 'f' AND confrelid = to_regclass('public."Reservations"') LOOP EXECUTE format('ALTER TABLE %s DROP CONSTRAINT IF EXISTS %I', item.table_name, item.conname); END LOOP; END $$;""");
        migrationBuilder.Sql("DROP TABLE IF EXISTS public.\"Reservations\" CASCADE;");
    }
}
