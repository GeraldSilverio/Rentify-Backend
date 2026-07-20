using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Implementations.Services;

public sealed class ReservationCodeGenerator : IReservationCodeGenerator
{
    private readonly IReservationRepository _reservations;

    public ReservationCodeGenerator(IReservationRepository reservations)
    {
        _reservations = reservations;
    }

    public async Task<string> GenerateAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        int year = DateTime.UtcNow.Year;
        string? lastCode =
            await _reservations.GetLastCodeAsync(
                tenantId,
                year,
                cancellationToken);

        int sequence = 1;

        if (lastCode is not null
            && int.TryParse(lastCode.Split('-').LastOrDefault(), out int value))
        {
            sequence = value + 1;
        }

        string code;

        do
        {
            code = $"RV-{year}-{sequence++:D6}";
        }
        while (await _reservations.CodeExistsAsync(tenantId, code, cancellationToken));

        return code;
    }
}
