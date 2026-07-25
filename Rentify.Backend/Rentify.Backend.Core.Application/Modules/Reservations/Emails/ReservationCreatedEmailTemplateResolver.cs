using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Emails;

public static class ReservationCreatedEmailTemplateResolver
{
    public static string Resolve(ReservationChannel channel)
    {
        return channel switch
        {
            ReservationChannel.PublicWeb => EmailTemplateCodes.ReservationWeb,
            ReservationChannel.Internal => EmailTemplateCodes.ReservationTenant,
            _ => throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                "Unsupported reservation channel.")
        };
    }
}
