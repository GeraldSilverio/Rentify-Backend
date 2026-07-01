using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ManageVehicleCatalog;

public enum VehicleCatalogKind
{
    Brand = 1,
    Model = 2,
    Type = 3
}

public enum VehicleCatalogAction
{
    Create = 1,
    Update = 2,
    Activate = 3,
    Deactivate = 4
}

public sealed record ManageVehicleCatalogRequest(
    string? Name,
    Guid? VehicleBrandId = null);

public sealed record ManageVehicleCatalogCommand(
    VehicleCatalogKind CatalogKind,
    VehicleCatalogAction Action,
    Guid? CatalogId,
    string? Name,
    Guid? VehicleBrandId,
    string ModifiedBy) : IRequest<ResultReponse<object>>;
