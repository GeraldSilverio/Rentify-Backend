using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;

public interface IRentCarRepository
{
    Task AddAsync(
        RentCar rentCar,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateEmailExistAsync(string email,
        CancellationToken cancellationToken = default);

    Task<bool> ValidatePhoneNumberExistAsync(string phoneNumber,
        CancellationToken cancellationToken = default);
    Task<bool> ValidateWhatsAppExistAsync(string whatsApp,
        CancellationToken cancellationToken = default);
}