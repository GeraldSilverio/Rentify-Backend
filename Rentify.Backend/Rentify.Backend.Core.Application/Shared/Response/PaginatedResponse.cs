namespace Rentify.Backend.Core.Application.Common.Response;

public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);
