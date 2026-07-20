using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;
namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed record UpdateReservationCommand(Guid TenantId, Guid ReservationId, DateTime DeliveryDateTime, DateTime ExpectedReturnDateTime, RentalType RentalType, Guid? DeliveryTenantLocationId, string? DeliveryLocationName, string? DeliveryAddressDetails, decimal DeliveryFee, Guid? ReturnTenantLocationId, string? ReturnLocationName, string? ReturnAddressDetails, decimal ReturnFee, decimal DiscountAmount, string? Notes, string ModifiedBy) : IRequest<ResultReponse<ReservationResponse>>;
