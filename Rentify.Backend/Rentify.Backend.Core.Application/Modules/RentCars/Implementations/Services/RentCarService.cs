using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Implementations.Services;

public sealed class RentCarService : IRentCarService
{
    private readonly IRentCarRepository _rentCarRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RentCarService(IRentCarRepository rentCarRepository, IUnitOfWork unitOfWork)
    {
        _rentCarRepository = rentCarRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> CreateRentCarAsync(CreateRentCarCommand rentCarCommand,CancellationToken cancellationToken = default)
    {
        if(await _rentCarRepository.ValidateEmailExistAsync(rentCarCommand.ContactInformation.Email))
            throw new ApiException($"Rent car with Email '{rentCarCommand.ContactInformation.Email}' already exists.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidatePhoneNumberExistAsync(rentCarCommand.ContactInformation.PhoneNumber))
            throw new ApiException($"Rent car with Phone Number '{rentCarCommand.ContactInformation.PhoneNumber}' already exists.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidateWhatsAppExistAsync(rentCarCommand.ContactInformation.WhatsApp))
            throw new ApiException($"Rent car with WhatsApp '{rentCarCommand.ContactInformation.WhatsApp}' already exists.",StatusCodes.Status400BadRequest);

        RentCar rentCar = RentCar.Create
        (rentCarCommand.Name,
            rentCarCommand.Description,
            rentCarCommand.ContactInformation.PhoneNumber,
            rentCarCommand.ContactInformation.Email,
            rentCarCommand.ContactInformation.WhatsApp,
            rentCarCommand.AddressInformation.Street,
            rentCarCommand.AddressInformation.City,
            rentCarCommand.AddressInformation.Country,
            rentCarCommand.CreatedBy,
            rentCarCommand.TenantId
            );
        
        await _rentCarRepository.AddAsync(
            rentCar,
            cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return rentCar.Id;
    }
}