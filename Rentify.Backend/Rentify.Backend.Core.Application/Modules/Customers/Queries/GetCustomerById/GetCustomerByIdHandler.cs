using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdHandler
    : IRequestHandler<GetCustomerByIdQuery, ResultReponse<CustomerDetailsResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<CustomerDetailsResponse>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        CustomerDetailsResponse response = await _customerRepository.GetDetailsAsync(
            request.TenantId,
            request.CustomerId,
            cancellationToken)
            ?? throw new ApiException("Customer not found.", StatusCodes.Status404NotFound);

        return ResultReponse<CustomerDetailsResponse>.Success(response);
    }
}
