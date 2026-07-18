using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.CreateVehicleBrand
{
    public sealed class CreateVehicleBrandHandler : IRequestHandler<CreateVehicleBrandCommand, ResultReponse<CreateVehicleBrandResponse>>
    {
        private readonly IVehicleCatalogRepository _vehicleCatalogRepository;
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly IUnitOfWork _unitOfWork;

        public CreateVehicleBrandHandler(IVehicleCatalogRepository vehicleCatalogRepository, ICurrentRequestContext currentRequestContext, IUnitOfWork unitOfWork)
        {
            _vehicleCatalogRepository = vehicleCatalogRepository;
            _currentRequestContext = currentRequestContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultReponse<CreateVehicleBrandResponse>> Handle(CreateVehicleBrandCommand request, CancellationToken cancellationToken)
        {
            if (await _vehicleCatalogRepository.BrandNameExistsAsync(request.BrandName, default, cancellationToken))
            {
                throw new ApiException("Esta marca ya existe en el catálogo", StatusCodes.Status400BadRequest);
            }

            VehicleBrand vehicleBrand = VehicleBrand.Create(request.BrandName, _currentRequestContext.UserId.ToString());

            await _vehicleCatalogRepository.AddBrandAsync(vehicleBrand, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultReponse<CreateVehicleBrandResponse>.Success(new CreateVehicleBrandResponse(vehicleBrand.Id, vehicleBrand.Name));
        }
    }
}
