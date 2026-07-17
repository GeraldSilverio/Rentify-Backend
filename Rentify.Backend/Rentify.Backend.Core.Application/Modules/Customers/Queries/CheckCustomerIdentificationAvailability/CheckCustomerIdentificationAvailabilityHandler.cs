using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Customers;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerIdentificationAvailability;

public sealed class CheckCustomerIdentificationAvailabilityHandler
    : IRequestHandler<CheckCustomerIdentificationAvailabilityQuery, ResultReponse<CustomerAvailabilityResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public CheckCustomerIdentificationAvailabilityHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<CustomerAvailabilityResponse>> Handle(
        CheckCustomerIdentificationAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        string normalizedIdentificationNumber = Customer.NormalizeIdentificationNumber(request.IdentificationNumber);
        bool exists = await _customerRepository.IdentificationExistsAsync(
            request.TenantId,
            request.IdentificationType,
            normalizedIdentificationNumber,
            request.ExcludeCustomerId,
            cancellationToken);

        CustomerAvailabilityResponse response = exists
            ? new CustomerAvailabilityResponse(false, "Ya existe un cliente con esta identificación.")
            : new CustomerAvailabilityResponse(true, null);

        return ResultReponse<CustomerAvailabilityResponse>.Success(response);
    }
}
