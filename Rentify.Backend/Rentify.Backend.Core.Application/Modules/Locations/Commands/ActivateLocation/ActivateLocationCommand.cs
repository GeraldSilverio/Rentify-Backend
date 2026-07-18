using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateLocation;

public sealed record ActivateLocationCommand(Guid LocationId, string ModifiedBy) : IRequest<ResultReponse<bool>>;
