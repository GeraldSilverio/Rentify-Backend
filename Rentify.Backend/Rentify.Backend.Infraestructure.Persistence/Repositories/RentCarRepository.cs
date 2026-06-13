using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.ValueObjects;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class RentCarRepository
    : IRentCarRepository
{
    private readonly RentifyContext _context;

    public RentCarRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RentCar rentCar,
        CancellationToken cancellationToken = default)
    {
        await _context.RentCars.AddAsync(
            rentCar,
            cancellationToken);
    }

    public async Task<bool> ValidateEmailExistAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.RentCars.AnyAsync(x => x.Email.Value == email, cancellationToken);
    }

    public async Task<bool> ValidatePhoneNumberExistAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.RentCars.AnyAsync(x => x.Phone.Value == phoneNumber, cancellationToken);
    }

    public async Task<bool> ValidateWhatsAppExistAsync(string whatsApp, CancellationToken cancellationToken = default)
    {
        return await _context.RentCars.AnyAsync(x => x.WhatsApp.Value == whatsApp, cancellationToken);
    }
}