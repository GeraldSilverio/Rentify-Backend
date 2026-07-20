using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Customers;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Services;

public sealed class ReservationCustomerValidator : IReservationCustomerValidator
{
    private readonly ICustomerRepository _customerRepository;

    public ReservationCustomerValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task ValidateAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        Customer? customer =
            await _customerRepository.GetByIdAsync(
                tenantId,
                customerId,
                cancellationToken);

        if (customer is null || !customer.IsActive)
        {
            throw new ApiException(
                "Cliente no encontrado.",
                StatusCodes.Status404NotFound);
        }
    }
}
