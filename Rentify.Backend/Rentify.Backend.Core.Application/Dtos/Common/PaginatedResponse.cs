namespace Rentify.Backend.Core.Application.Dtos.Common;

public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);
