using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;

public sealed record UploadRentCarLogoCommand(
    Guid RentCarId,
    IFormFile Logo,
    string ModifiedBy) : IRequest<ResultReponse<string>>;
