namespace Rentify.Backend.Core.Application.Modules.Shared.Request;

public record PaginationRequest(int PageNumber = 1, int PageSize = 10);
