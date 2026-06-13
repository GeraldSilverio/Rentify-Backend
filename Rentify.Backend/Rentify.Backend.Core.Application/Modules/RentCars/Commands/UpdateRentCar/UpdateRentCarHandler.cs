using MediatR;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public sealed class UpdateRentCarHandler : IRequestHandler<UpdateRentCarCommand, ResultReponse<Guid>>
{
    private readonly IRentCarService _rentCarService;

    public UpdateRentCarHandler(IRentCarService rentCarService)
    {
        _rentCarService = rentCarService;
    }

    public async Task<ResultReponse<Guid>> Handle(UpdateRentCarCommand request, CancellationToken cancellationToken)
    {
        Guid rentCarId = await _rentCarService.UpdateRentCarAsync(request, cancellationToken);

        return ResultReponse<Guid>.Success(rentCarId);
    }
}
