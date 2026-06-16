using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

namespace Rentify.Backend.Core.Application.Modules.Customers;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateCustomerEndpoint();
        app.MapUpdateCustomerEndpoint();
        app.MapDeleteCustomerEndpoint();
        app.MapSearchCustomersEndpoint();
        app.MapUploadCustomerDocumentEndpoint();

        return app;
    }
}
