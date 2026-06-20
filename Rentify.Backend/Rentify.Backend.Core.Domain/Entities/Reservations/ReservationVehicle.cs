using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Domain.Entities.Reservations;

public sealed class ReservationVehicle : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ReservationId { get; private set; }
    public Guid VehicleId { get; private set; }
    public decimal DailyRate { get; private set; }
    public int RentalDays { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Reservation Reservation { get; private set; } = null!;
    public Vehicle Vehicle { get; private set; } = null!;

    private ReservationVehicle()
    {
    }

    private ReservationVehicle(
        Guid tenantId,
        Guid reservationId,
        Guid vehicleId,
        decimal dailyRate,
        int rentalDays,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ReservationId = reservationId;
        VehicleId = vehicleId;
        DailyRate = dailyRate;
        RentalDays = rentalDays;
        TotalAmount = dailyRate * rentalDays;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static ReservationVehicle Create(
        Guid tenantId,
        Guid reservationId,
        Guid vehicleId,
        decimal dailyRate,
        int rentalDays,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation Id is required.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero.");

        if (rentalDays <= 0)
            throw new ArgumentException("Rental days must be greater than zero.");

        return new ReservationVehicle(tenantId, reservationId, vehicleId, dailyRate, rentalDays, createdBy);
    }
}
