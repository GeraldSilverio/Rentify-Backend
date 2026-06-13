using MediatR;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;

public sealed class UploadRentCarLogoHandler : IRequestHandler<UploadRentCarLogoCommand, ResultReponse<string>>
{
    private readonly IRentCarService _rentCarService;

    public UploadRentCarLogoHandler(IRentCarService rentCarService)
    {
        _rentCarService = rentCarService;
    }

    public async Task<ResultReponse<string>> Handle(UploadRentCarLogoCommand request, CancellationToken cancellationToken)
    {
        string logoUrl = await _rentCarService.UploadLogoAsync(request, cancellationToken);

        return ResultReponse<string>.Success(logoUrl);
    }
}
