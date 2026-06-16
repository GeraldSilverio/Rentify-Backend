using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

public sealed class SearchCustomersHandler : IRequestHandler<SearchCustomersQuery, ResultReponse<IReadOnlyList<CustomerResponse>>>
{
    private readonly ICustomerRepository _customerRepository;

    public SearchCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<IReadOnlyList<CustomerResponse>>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.SearchAsync(request.TenantId, request.SearchTerm, cancellationToken);
        var response = customers.Select(x => new CustomerResponse(
            x.Id,
            x.TenantId,
            x.FirstName,
            x.LastName,
            x.Email,
            x.PhoneNumber,
            x.LicenseNumber,
            x.LicenseExpirationDate)).ToList();

        return ResultReponse<IReadOnlyList<CustomerResponse>>.Success(response);
    }
}
