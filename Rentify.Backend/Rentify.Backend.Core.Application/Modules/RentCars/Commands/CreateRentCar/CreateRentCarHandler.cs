using MediatR;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;

public sealed class CreateRentCarHandler : IRequestHandler<CreateRentCarCommand,ResultReponse<Guid>>
{
    private readonly IRentCarService  _rentCarService;

    public CreateRentCarHandler(IRentCarService rentCarService)
    {
        _rentCarService = rentCarService;
    }

    public async Task<ResultReponse<Guid>> Handle(CreateRentCarCommand request, CancellationToken cancellationToken)
    {
        Guid rentcardId = await _rentCarService.CreateRentCarAsync(request, cancellationToken);

        return ResultReponse<Guid>.Success(rentcardId);
    }
}