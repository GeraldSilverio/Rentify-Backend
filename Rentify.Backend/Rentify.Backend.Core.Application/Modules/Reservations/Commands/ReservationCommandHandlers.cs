using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Locations;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed class ReservationCommandHandlers : IRequestHandler<CreateReservationCommand, ResultReponse<ReservationResponse>>, IRequestHandler<UpdateReservationCommand, ResultReponse<ReservationResponse>>, IRequestHandler<ApproveReservationCommand, ResultReponse<ReservationResponse>>, IRequestHandler<RejectReservationCommand, ResultReponse<ReservationResponse>>, IRequestHandler<CancelReservationCommand, ResultReponse<ReservationResponse>>, IRequestHandler<DeleteReservationCommand, ResultReponse<Guid>>
{
    private readonly IReservationRepository _reservations; private readonly ICustomerRepository _customers; private readonly IVehicleRepository _vehicles; private readonly ITenantLocationRepository _locations; private readonly IReservationCodeGenerator _codes; private readonly IUnitOfWork _unitOfWork;
    public ReservationCommandHandlers(IReservationRepository reservations, ICustomerRepository customers, IVehicleRepository vehicles, ITenantLocationRepository locations, IReservationCodeGenerator codes, IUnitOfWork unitOfWork) { _reservations = reservations; _customers = customers; _vehicles = vehicles; _locations = locations; _codes = codes; _unitOfWork = unitOfWork; }

    public async Task<ResultReponse<ReservationResponse>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        await ValidateCustomerAsync(request.TenantId, request.CustomerId, cancellationToken); Vehicle vehicle = await GetReservableVehicleAsync(request.TenantId, request.VehicleId, cancellationToken);
        (string deliveryName, decimal deliveryFee) = await ResolveDeliveryAsync(request.TenantId, request.DeliveryTenantLocationId, request.DeliveryLocationName, request.DeliveryFee, cancellationToken);
        (string returnName, decimal returnFee) = await ResolveReturnAsync(request.TenantId, request.ReturnTenantLocationId, request.ReturnLocationName, request.ReturnFee, cancellationToken);
        int quantity = Reservation.CalculateQuantity(request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType); VehicleRate rate = GetRate(vehicle, request.RentalType);
        string code = await _codes.GenerateAsync(request.TenantId, cancellationToken);
        Reservation reservation = Reservation.Create(request.TenantId, code, request.CustomerId, request.VehicleId, request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType, quantity, rate.Price, vehicle.SecurityDepositRequired, vehicle.SecurityDepositAmount, request.DeliveryTenantLocationId, deliveryName, request.DeliveryAddressDetails, deliveryFee, request.ReturnTenantLocationId, returnName, request.ReturnAddressDetails, returnFee, request.DiscountAmount, request.Channel, request.Notes, request.CreatedBy);
        await _reservations.AddAsync(reservation, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<ReservationResponse>.Success(ToResponse(reservation));
    }
    public async Task<ResultReponse<ReservationResponse>> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation reservation = await GetReservationAsync(request.TenantId, request.ReservationId, cancellationToken); Vehicle vehicle = await GetReservableVehicleAsync(request.TenantId, reservation.VehicleId, cancellationToken);
        (string deliveryName, decimal deliveryFee) = await ResolveDeliveryAsync(request.TenantId, request.DeliveryTenantLocationId, request.DeliveryLocationName, request.DeliveryFee, cancellationToken);
        (string returnName, decimal returnFee) = await ResolveReturnAsync(request.TenantId, request.ReturnTenantLocationId, request.ReturnLocationName, request.ReturnFee, cancellationToken);
        int quantity = Reservation.CalculateQuantity(request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType); VehicleRate rate = GetRate(vehicle, request.RentalType);
        reservation.UpdatePendingReservation(request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType, quantity, rate.Price, vehicle.SecurityDepositRequired, vehicle.SecurityDepositAmount, request.DeliveryTenantLocationId, deliveryName, request.DeliveryAddressDetails, deliveryFee, request.ReturnTenantLocationId, returnName, request.ReturnAddressDetails, returnFee, request.DiscountAmount, reservation.Channel, request.Notes, request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<ReservationResponse>.Success(ToResponse(reservation));
    }
    public async Task<ResultReponse<ReservationResponse>> Handle(ApproveReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation reservation = await GetReservationAsync(request.TenantId, request.ReservationId, cancellationToken); await ValidateCustomerAsync(request.TenantId, reservation.CustomerId, cancellationToken); await GetReservableVehicleAsync(request.TenantId, reservation.VehicleId, cancellationToken);
        if (await _reservations.HasApprovedOverlapAsync(request.TenantId, reservation.VehicleId, reservation.DeliveryDateTime, reservation.ExpectedReturnDateTime, reservation.Id, cancellationToken)) throw new ApiException("El vehÃ­culo ya tiene una reserva aprobada para el rango de fechas seleccionado.", StatusCodes.Status400BadRequest);
        reservation.Approve(request.ApprovedBy); await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<ReservationResponse>.Success(ToResponse(reservation));
    }
    public async Task<ResultReponse<ReservationResponse>> Handle(RejectReservationCommand request, CancellationToken cancellationToken) { Reservation reservation = await GetReservationAsync(request.TenantId, request.ReservationId, cancellationToken); reservation.Reject(request.Reason, request.RejectedBy); await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<ReservationResponse>.Success(ToResponse(reservation)); }
    public async Task<ResultReponse<ReservationResponse>> Handle(CancelReservationCommand request, CancellationToken cancellationToken) { Reservation reservation = await GetReservationAsync(request.TenantId, request.ReservationId, cancellationToken); reservation.Cancel(request.Reason, request.CancelledBy); await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<ReservationResponse>.Success(ToResponse(reservation)); }
    public async Task<ResultReponse<Guid>> Handle(DeleteReservationCommand request, CancellationToken cancellationToken) { Reservation reservation = await GetReservationAsync(request.TenantId, request.ReservationId, cancellationToken); reservation.Delete(request.ModifiedBy); await _unitOfWork.SaveChangesAsync(cancellationToken); return ResultReponse<Guid>.Success(reservation.Id); }
    private async Task<Reservation> GetReservationAsync(Guid tenantId, Guid id, CancellationToken ct) => await _reservations.GetByIdAsync(tenantId, id, ct) ?? throw new ApiException("Reserva no encontrada.", StatusCodes.Status404NotFound);
    private async Task ValidateCustomerAsync(Guid tenantId, Guid id, CancellationToken ct) { var customer = await _customers.GetByIdAsync(tenantId, id, ct); if (customer is null || !customer.IsActive) throw new ApiException("Cliente no encontrado.", StatusCodes.Status404NotFound); }
    private async Task<Vehicle> GetReservableVehicleAsync(Guid tenantId, Guid id, CancellationToken ct) { Vehicle vehicle = await _vehicles.GetByIdAsync(tenantId, id, ct) ?? throw new ApiException("VehÃ­culo no encontrado.", StatusCodes.Status404NotFound); if (!vehicle.IsActive || vehicle.Status is VehicleStatus.Maintenance or VehicleStatus.OutOfService) throw new ApiException("El vehÃ­culo no estÃ¡ disponible para reservas.", StatusCodes.Status400BadRequest); return vehicle; }
    private static VehicleRate GetRate(Vehicle vehicle, RentalType rentalType) => vehicle.Rates.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.RentalType == rentalType) ?? throw new ApiException("El vehÃ­culo no tiene una tarifa configurada para este tipo de renta.", StatusCodes.Status400BadRequest);
    private async Task<(string, decimal)> ResolveDeliveryAsync(Guid tenantId, Guid? id, string? name, decimal fee, CancellationToken ct) { if (!id.HasValue) return (RequireLocationName(name), fee); TenantLocation location = await _locations.GetByIdAsync(tenantId, id.Value, ct) ?? throw new ApiException("La ubicaciÃ³n de entrega no estÃ¡ disponible.", StatusCodes.Status400BadRequest); if (!location.IsActive) throw new ApiException("La ubicaciÃ³n de entrega no estÃ¡ disponible.", StatusCodes.Status400BadRequest); if (!location.AllowsDelivery) throw new ApiException("La ubicaciÃ³n de entrega no permite entregas.", StatusCodes.Status400BadRequest); return (location.DisplayName, location.DeliveryFee); }
    private async Task<(string, decimal)> ResolveReturnAsync(Guid tenantId, Guid? id, string? name, decimal fee, CancellationToken ct) { if (!id.HasValue) return (RequireLocationName(name), fee); TenantLocation location = await _locations.GetByIdAsync(tenantId, id.Value, ct) ?? throw new ApiException("La ubicaciÃ³n de devoluciÃ³n no estÃ¡ disponible.", StatusCodes.Status400BadRequest); if (!location.IsActive) throw new ApiException("La ubicaciÃ³n de devoluciÃ³n no estÃ¡ disponible.", StatusCodes.Status400BadRequest); if (!location.AllowsPickup) throw new ApiException("La ubicaciÃ³n de devoluciÃ³n no permite recogidas.", StatusCodes.Status400BadRequest); return (location.DisplayName, location.PickupFee); }
    private static string RequireLocationName(string? name) => !string.IsNullOrWhiteSpace(name) ? name : throw new ApiException("El nombre de la ubicaciÃ³n es requerido.", StatusCodes.Status400BadRequest);
    private static ReservationResponse ToResponse(Reservation reservation) => new(reservation.Id, reservation.Code, reservation.Status, reservation.TotalAmount);
}
