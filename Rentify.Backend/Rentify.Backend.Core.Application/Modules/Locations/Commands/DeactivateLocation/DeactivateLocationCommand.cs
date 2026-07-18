using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateLocation;

public sealed record DeactivateLocationCommand(Guid LocationId, string ModifiedBy) : IRequest<ResultReponse<bool>>;
