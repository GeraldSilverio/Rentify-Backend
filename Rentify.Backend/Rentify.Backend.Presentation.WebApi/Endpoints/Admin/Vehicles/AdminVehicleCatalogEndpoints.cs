using MediatR;
using Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.CreateVehicleBrand;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Admin.Vehicles
{
    public static class AdminVehicleCatalogEndpoints
    {
        public static IEndpointRouteBuilder MapAdminVehicleCatalogEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/admin/vehicle-catalog/brands", (CreateVehicleBrandCommand command, ISender sender) =>
            {
                return sender.Send(command);
            })
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin - Vehicle Catalog")
            .WithSummary("Create a new vehicle brand");

            return app;

        }
    }
}
