using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Mappers;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Reservations.Events;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;
using System.Diagnostics;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.ApproveReservation;

public sealed class ApproveReservationCommandHandler
    : IRequestHandler<ApproveReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IReservationVehicleResolver _vehicleResolver;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxService _outboxService;
    private readonly ICurrentRequestContext _currentRequestContext;
    private readonly ILogger<ApproveReservationCommandHandler> _logger;

    public ApproveReservationCommandHandler(
        IReservationRepository reservationRepository,
        ICustomerRepository customerRepository,
        IReservationVehicleResolver vehicleResolver,
        IUnitOfWork unitOfWork,
        IVehicleRepository vehicleRepository,
        IOutboxService outboxService,
        ICurrentRequestContext currentRequestContext,
        ILogger<ApproveReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _customerRepository = customerRepository;
        _vehicleResolver = vehicleResolver;
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
        _outboxService = outboxService;
        _currentRequestContext = currentRequestContext;
        _logger = logger;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        ApproveReservationCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Attempting to approve Reservation {ReservationId} in Tenant {TenantId}",
            request.ReservationId,
            request.TenantId);

        Reservation reservation =
            await _reservationRepository.GetByIdAsync(
                request.TenantId,
                request.ReservationId,
                cancellationToken)
            ?? throw new ApiException(
                "Reserva no encontrada.",
                StatusCodes.Status404NotFound);

        bool customerExists = await _customerRepository.ExistCustomerByIdAsync(
            request.TenantId,
            reservation.CustomerId,
            cancellationToken);

        if (!customerExists) throw new ApiException("El cliente no existe.", StatusCodes.Status404NotFound);

        await _vehicleResolver.GetReservableVehicleAsync(
            request.TenantId,
            reservation.VehicleId,
            cancellationToken);

        bool hasApprovedOverlap =
            await _reservationRepository.HasApprovedOverlapAsync(
                request.TenantId,
                reservation.VehicleId,
                reservation.DeliveryDateTime,
                reservation.ExpectedReturnDateTime,
                reservation.Id,
                cancellationToken);

        if (hasApprovedOverlap)
        {
            _logger.LogWarning(
                "Vehicle availability conflict detected for Vehicle {VehicleId} from {StartDateTime} to {EndDateTime} in Tenant {TenantId} while approving Reservation {ReservationId}",
                reservation.VehicleId,
                reservation.DeliveryDateTime,
                reservation.ExpectedReturnDateTime,
                reservation.TenantId,
                reservation.Id);

            throw new ApiException(
                "El vehículo ya tiene una reserva aprobada para el rango de fechas seleccionado.",
                StatusCodes.Status400BadRequest);
        }

        reservation.Approve(request.ApprovedBy);

        await _outboxService.AddAsync(
            request.TenantId,
            OutboxMessageTypes.ReservationApproved,
            new ReservationApprovedOutboxPayload(
                request.TenantId,
                reservation.Id,
                reservation.ApprovedAt!.Value),
            request.ApprovedBy,
            correlationId: Activity.Current?.GetTagItem("CorrelationId")?.ToString(),
            cancellationToken);

        VehicleUnavailableDate vehicleUnavailableDate =
            VehicleUnavailableDate.Create(reservation.TenantId,
            reservation.VehicleId,
            DateOnly.FromDateTime(reservation.DeliveryDateTime.Date),
            DateOnly.FromDateTime(reservation.ExpectedReturnDateTime.Date),
            VehicleStatus.Reserved.ToString(),
            request.ApprovedBy);

        await _vehicleRepository.AddUnavailableDateAsync(vehicleUnavailableDate, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reservation {ReservationId} approved for Vehicle {VehicleId} in Tenant {TenantId} by {ApprovedBy}; availability validated {AvailabilityValidated} with block {AvailabilityBlockId} and status {PreviousStatus} to {NewStatus}",
            reservation.Id,
            reservation.VehicleId,
            reservation.TenantId,
            RequestContextLogValues.GetUserId(_currentRequestContext),
            true,
            vehicleUnavailableDate.Id,
            ReservationStatus.Pending,
            reservation.Status);

        _logger.LogInformation(
            "Email {EmailTemplateCode} enqueued for Reservation {ReservationId} in Tenant {TenantId}",
            EmailTemplateCodes.ReservationApproved,
            reservation.Id,
            reservation.TenantId);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
