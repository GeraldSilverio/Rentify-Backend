using FluentValidation;
using Rentify.Backend.Core.Application.Contracts.Repositories;
using Rentify.Backend.Core.Application.Contracts.Services;
using Rentify.Backend.Core.Application.Dtos.Common;
using Rentify.Backend.Core.Application.Dtos.RentCars;
using Rentify.Backend.Core.Application.Wrappers;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Services;

public class RentCarService : IRentCarService
{
    private readonly IRentCarRepository _repository;
    private readonly IValidator<CreateRentCarRequest> _createValidator;
    private readonly IValidator<UpdateRentCarRequest> _updateValidator;

    public RentCarService(
        IRentCarRepository repository,
        IValidator<CreateRentCarRequest> createValidator,
        IValidator<UpdateRentCarRequest> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<RentCarResponse>> CreateAsync(CreateRentCarRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<RentCarResponse>.Failure(
                Error.Validation(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        var entity = RentCar.Create(
            request.Name,
            request.Description,
            request.Phone,
            request.Email,
            request.WhatsApp,
            request.Street,
            request.City,
            request.Country,
            request.CreatedBy);

        await _repository.AddAsync(entity);

        return Result<RentCarResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<RentCarResponse>> UpdateAsync(Guid id, UpdateRentCarRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<RentCarResponse>.Failure(
                Error.Validation(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return Result<RentCarResponse>.Failure(Error.NotFound($"RentCar with Id '{id}' was not found."));

        entity.Update(
            request.Name,
            request.Description,
            request.Phone,
            request.Email,
            request.WhatsApp,
            request.Street,
            request.City,
            request.Country,
            request.UpdatedBy);

        await _repository.UpdateAsync(entity);

        return Result<RentCarResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<RentCarResponse>> DisableAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return Result<RentCarResponse>.Failure(Error.NotFound($"RentCar with Id '{id}' was not found."));

        entity.Disable();
        await _repository.UpdateAsync(entity);

        return Result<RentCarResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<RentCarResponse>> EnableAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return Result<RentCarResponse>.Failure(Error.NotFound($"RentCar with Id '{id}' was not found."));

        entity.Enable();
        await _repository.UpdateAsync(entity);

        return Result<RentCarResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<RentCarResponse>> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return Result<RentCarResponse>.Failure(Error.NotFound($"RentCar with Id '{id}' was not found."));

        return Result<RentCarResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<PaginatedResponse<RentCarResponse>>> GetPagedAsync(PaginationRequest request)
    {
        var totalCount = await _repository.GetTotalCountAsync();
        var entities = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var items = entities.Select(MapToResponse).ToList();

        var response = new PaginatedResponse<RentCarResponse>(
            items, request.PageNumber, request.PageSize, totalCount, totalPages);

        return Result<PaginatedResponse<RentCarResponse>>.Success(response);
    }

    private static RentCarResponse MapToResponse(RentCar entity)
    {
        return new RentCarResponse(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.Phone.Value,
            entity.Email.Value,
            entity.WhatsApp.Value,
            entity.Address.Street,
            entity.Address.City,
            entity.Address.Country,
            entity.IsActive,
            entity.CreatedDate,
            entity.ModifiedDate,
            entity.CreatedBy,
            entity.ModifiedBy);
    }
}
