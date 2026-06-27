using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantPaymentPolicy;

public sealed record UpdateTenantPaymentPolicyRequest(
    string Name,
    PaymentFrequency PaymentFrequency,
    DayOfWeek CutoffDayOfWeek,
    int GraceDays,
    DayOfWeek ReminderStartDayOfWeek,
    bool LateFeeEnabled);
