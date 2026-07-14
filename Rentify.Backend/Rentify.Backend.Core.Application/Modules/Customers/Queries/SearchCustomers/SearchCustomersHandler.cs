using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

public sealed class SearchCustomersHandler : IRequestHandler<SearchCustomersQuery, ResultReponse<PaginatedResponse<CustomerResponse>>>
{
    private readonly ICustomerRepository _customerRepository;

    public SearchCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<CustomerResponse>>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        PaginatedResponse<CustomerResponse> response = await _customerRepository.SearchAsync(request, cancellationToken);

        return ResultReponse<PaginatedResponse<CustomerResponse>>.Success(response);
    }
}
