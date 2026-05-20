using Rentify.Backend.Core.Application.Dtos.Common;
using Rentify.Backend.Core.Application.Dtos.RentCars;
using Rentify.Backend.Core.Application.Wrappers;

namespace Rentify.Backend.Core.Application.Contracts.Services;

public interface IRentCarService
{
    Task<Result<RentCarResponse>> CreateAsync(CreateRentCarRequest request);
    Task<Result<RentCarResponse>> UpdateAsync(Guid id, UpdateRentCarRequest request);
    Task<Result<RentCarResponse>> DisableAsync(Guid id);
    Task<Result<RentCarResponse>> EnableAsync(Guid id);
    Task<Result<RentCarResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginatedResponse<RentCarResponse>>> GetPagedAsync(PaginationRequest request);
}
