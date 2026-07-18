using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.UpdateVehicleBrand
{
    public sealed class UpdateVehicleBrandHandler : IRequestHandler<UpdateVehicleBrandCommand, ResultReponse<UpdateVehicleBrandResponse>>
    {
        private readonly IVehicleCatalogRepository _vehicleCatalogRepository;
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVehicleBrandHandler(IVehicleCatalogRepository vehicleCatalogRepository, ICurrentRequestContext currentRequestContext, IUnitOfWork unitOfWork)
        {
            _vehicleCatalogRepository = vehicleCatalogRepository;
            _currentRequestContext = currentRequestContext;
            _unitOfWork = unitOfWork;
        }

        public Task<ResultReponse<UpdateVehicleBrandResponse>> Handle(UpdateVehicleBrandCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
