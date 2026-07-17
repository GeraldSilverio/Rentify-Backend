using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerEmailAvailability;

public sealed class CheckCustomerEmailAvailabilityHandler
    : IRequestHandler<CheckCustomerEmailAvailabilityQuery, ResultReponse<CustomerAvailabilityResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public CheckCustomerEmailAvailabilityHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<CustomerAvailabilityResponse>> Handle(
        CheckCustomerEmailAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        bool exists = await _customerRepository.EmailExistsAsync(
            request.TenantId,
            request.Email,
            request.ExcludeCustomerId,
            cancellationToken);

        CustomerAvailabilityResponse response = exists
            ? new CustomerAvailabilityResponse(false, "Ya existe un cliente con este correo electrónico.")
            : new CustomerAvailabilityResponse(true, null);

        return ResultReponse<CustomerAvailabilityResponse>.Success(response);
    }
}
