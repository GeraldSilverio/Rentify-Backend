namespace Rentify.Backend.Core.Application.Common.Dtos;

public record PaginationRequest(int PageNumber = 1, int PageSize = 10);
