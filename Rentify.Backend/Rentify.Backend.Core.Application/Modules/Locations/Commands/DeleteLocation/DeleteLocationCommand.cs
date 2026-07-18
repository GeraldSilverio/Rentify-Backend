using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteLocation;

public sealed record DeleteLocationCommand(Guid LocationId, string ModifiedBy) : IRequest<ResultReponse<bool>>;
