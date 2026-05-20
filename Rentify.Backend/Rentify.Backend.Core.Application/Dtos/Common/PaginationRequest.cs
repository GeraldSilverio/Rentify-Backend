namespace Rentify.Backend.Core.Application.Dtos.Common;

public record PaginationRequest(int PageNumber = 1, int PageSize = 10);
