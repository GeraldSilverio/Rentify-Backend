using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantPaymentPolicyResponse(
    Guid Id,
    string Name,
    PaymentFrequency PaymentFrequency,
    DayOfWeek CutoffDayOfWeek,
    int GraceDays,
    DayOfWeek ReminderStartDayOfWeek,
    bool LateFeeEnabled,
    bool IsDefault);
