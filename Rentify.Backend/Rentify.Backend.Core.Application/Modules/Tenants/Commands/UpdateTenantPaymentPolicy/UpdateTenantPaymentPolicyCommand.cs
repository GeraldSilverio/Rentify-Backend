using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantPaymentPolicy;

public sealed record UpdateTenantPaymentPolicyCommand(
    Guid TenantId,
    string Name,
    PaymentFrequency PaymentFrequency,
    DayOfWeek CutoffDayOfWeek,
    int GraceDays,
    DayOfWeek ReminderStartDayOfWeek,
    bool LateFeeEnabled,
    string ModifiedBy) : IRequest<ResultReponse<TenantPaymentPolicyResponse>>;
