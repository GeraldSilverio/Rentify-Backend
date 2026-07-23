using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservations;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly RentifyContext _context;

    public ReservationRepository(RentifyContext context)
    {
        _context = context;
    }

    public Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations.AddAsync(reservation, cancellationToken).AsTask();
    }

    public Task<Reservation?> GetByIdAsync(
        Guid tenantId,
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations.FirstOrDefaultAsync(
            reservation =>
                reservation.TenantId == tenantId
                && reservation.Id == reservationId
                && !reservation.IsDeleted,
            cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        Guid tenantId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations.AnyAsync(
            reservation =>
                reservation.TenantId == tenantId
                && reservation.Code == code
                && !reservation.IsDeleted,
            cancellationToken);
    }

    public Task<string?> GetLastCodeAsync(
        Guid tenantId,
        int year,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.TenantId == tenantId
                && !reservation.IsDeleted
                && reservation.Code.StartsWith($"RV-{year}-"))
            .OrderByDescending(reservation => reservation.Code)
            .Select(reservation => reservation.Code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> HasApprovedOverlapAsync(
        Guid tenantId,
        Guid vehicleId,
        DateTime deliveryDateTime,
        DateTime expectedReturnDateTime,
        Guid? excludedReservationId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations
            .AsNoTracking()
            .AnyAsync(
                reservation =>
                    reservation.TenantId == tenantId
                    && reservation.VehicleId == vehicleId
                    && reservation.Status == ReservationStatus.Approved
                    && !reservation.IsDeleted
                    && (!excludedReservationId.HasValue || reservation.Id != excludedReservationId.Value)
                    && deliveryDateTime < reservation.ExpectedReturnDateTime
                    && expectedReturnDateTime > reservation.DeliveryDateTime,
                cancellationToken);
    }

    public async Task<PaginatedResponse<ReservationListItemResponse>> GetPagedAsync(
        GetReservationsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Reservation> source = _context.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.TenantId == query.TenantId
                && !reservation.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string pattern = $"%{query.Search.Trim()}%";

            source = source.Where(reservation =>
                EF.Functions.ILike(reservation.Code, pattern)
                || EF.Functions.ILike(reservation.Customer.FirstName, pattern)
                || EF.Functions.ILike(reservation.Customer.LastName, pattern)
                || EF.Functions.ILike(reservation.Customer.IdentificationNumber, pattern)
                || EF.Functions.ILike(reservation.Vehicle.PlateNumber, pattern)
                || EF.Functions.ILike(reservation.Vehicle.VehicleBrand.Name, pattern)
                || EF.Functions.ILike(reservation.Vehicle.VehicleModel.Name, pattern));
        }

        if (query.Status.HasValue)
        {
            source = source.Where(reservation => reservation.Status == query.Status.Value);
        }

        if (query.CustomerId.HasValue)
        {
            source = source.Where(reservation => reservation.CustomerId == query.CustomerId);
        }

        if (query.VehicleId.HasValue)
        {
            source = source.Where(reservation => reservation.VehicleId == query.VehicleId);
        }

        if (query.FromDate.HasValue)
        {
            source = source.Where(reservation => reservation.ExpectedReturnDateTime >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            source = source.Where(reservation => reservation.DeliveryDateTime <= query.ToDate.Value);
        }

        int total = await source.CountAsync(cancellationToken);

        List<ReservationListItemResponse> items =
            await source
                .OrderByDescending(reservation => reservation.CreatedDate)
                .ThenBy(reservation => reservation.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(reservation => new ReservationListItemResponse(
                    reservation.Id,
                    reservation.Code,
                    reservation.CustomerId,
                    reservation.Customer.FirstName + " " + reservation.Customer.LastName,
                    reservation.VehicleId,
                    reservation.Vehicle.VehicleBrand.Name + " " + reservation.Vehicle.VehicleModel.Name,
                    reservation.Vehicle.PlateNumber,
                    reservation.DeliveryDateTime,
                    reservation.ExpectedReturnDateTime,
                    reservation.RentalType,
                    reservation.Quantity,
                    reservation.RentalAmount,
                    reservation.SecurityDepositAmount,
                    reservation.DeliveryFee,
                    reservation.ReturnFee,
                    reservation.DiscountAmount,
                    reservation.TotalAmount,
                    reservation.Status,
                    reservation.Channel,
                    reservation.CreatedDate))
                .ToListAsync(cancellationToken);

        int totalPages = total == 0
            ? 0
            : (int)Math.Ceiling(total / (double)query.PageSize);

        return new PaginatedResponse<ReservationListItemResponse>(
            items,
            query.PageNumber,
            query.PageSize,
            total,
            totalPages);
    }

    public Task<ReservationDetailsResponse?> GetDetailsAsync(
        Guid tenantId,
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.TenantId == tenantId
                && reservation.Id == reservationId
                && !reservation.IsDeleted)
            .Select(reservation => new ReservationDetailsResponse(
                reservation.Id,
                reservation.Code,
                new ReservationCustomerResponse(
                    reservation.Customer.Id,
                    reservation.Customer.FirstName + " " + reservation.Customer.LastName,
                    reservation.Customer.CustomerType,
                    reservation.Customer.IdentificationType,
                    reservation.Customer.IdentificationNumber,
                    reservation.Customer.PhoneNumber,
                    reservation.Customer.Email,
                    reservation.Customer.IsVerified),
                new ReservationVehicleResponse(
                    reservation.Vehicle.Id,
                    reservation.Vehicle.VehicleBrand.Name,
                    reservation.Vehicle.VehicleModel.Name,
                    reservation.Vehicle.VehicleType.Name,
                    reservation.Vehicle.Year,
                    reservation.Vehicle.PlateNumber,
                    reservation.Vehicle.Color,
                    reservation.Vehicle.Status,
                    reservation.Vehicle.Images
                        .Where(image => !image.IsDeleted && image.IsPrimary)
                        .Select(image => image.Url)
                        .FirstOrDefault()),
                reservation.DeliveryDateTime,
                reservation.ExpectedReturnDateTime,
                reservation.RentalType,
                reservation.Quantity,
                reservation.UnitRate,
                reservation.RentalAmount,
                reservation.SecurityDepositRequired,
                reservation.SecurityDepositAmount,
                reservation.DeliveryTenantLocationId,
                reservation.DeliveryLocationName,
                reservation.DeliveryAddressDetails,
                reservation.DeliveryFee,
                reservation.ReturnTenantLocationId,
                reservation.ReturnLocationName,
                reservation.ReturnAddressDetails,
                reservation.ReturnFee,
                reservation.DiscountAmount,
                reservation.TotalAmount,
                reservation.Status,
                reservation.Channel,
                reservation.ApprovedAt,
                reservation.ApprovedBy,
                reservation.RejectedAt,
                reservation.RejectionReason,
                reservation.RejectedBy,
                reservation.CancelledAt,
                reservation.CancellationReason,
                reservation.CancelledBy,
                reservation.ConvertedToRentalAt,
                reservation.ConvertedToRentalBy,
                reservation.Notes,
                reservation.CreatedDate,
                reservation.ModifiedDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ReservationApprovedEmailData?> GetApprovedEmailDataAsync(
        Guid tenantId,
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        return (
            from reservation in _context.Reservations.AsNoTracking()
            join tenant in _context.Tenants.AsNoTracking() on reservation.TenantId equals tenant.Id
            where reservation.TenantId == tenantId
                && reservation.Id == reservationId
                && reservation.Status == ReservationStatus.Approved
                && !reservation.IsDeleted
                && !reservation.Customer.IsDeleted
                && !reservation.Vehicle.IsDeleted
                && !tenant.IsDeleted
            select new ReservationApprovedEmailData(
                reservation.Customer.FirstName,
                reservation.Customer.Email,
                tenant.Name,
                tenant.PhoneNumber,
                tenant.WhatsApp,
                tenant.Email,
                reservation.Code,
                reservation.ApprovedAt ?? reservation.ModifiedDate,
                reservation.Vehicle.VehicleBrand.Name,
                reservation.Vehicle.VehicleModel.Name,
                reservation.Vehicle.Year,
                reservation.RentalType,
                reservation.Quantity,
                reservation.DeliveryDateTime,
                reservation.ExpectedReturnDateTime,
                reservation.DeliveryLocationName,
                reservation.ReturnLocationName,
                reservation.UnitRate,
                reservation.RentalAmount,
                reservation.DeliveryFee,
                reservation.ReturnFee,
                reservation.SecurityDepositRequired,
                reservation.SecurityDepositAmount,
                reservation.DiscountAmount,
                reservation.TotalAmount))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
