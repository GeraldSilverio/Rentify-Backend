using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleTypes;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles;

public static class VehicleCatalogEndpoints
{
    public static IEndpointRouteBuilder MapVehicleCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetVehiclesEndpoint();
        app.MapGetVehicleDetailEndpoint();
        app.MapGetBrandsEndpoint();
        app.MapGetModelsEndpoint();
        app.MapGetVehicleTypesEndpoints();

        return app;
    }
}
