namespace Rentify.Backend.Core.Application.Modules.Shared.Context;

public interface ICurrentUserService
{
    Guid UserId { get; }

    string? UserName { get; }

    string? Email { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsAuthenticated { get; }

    bool IsSuperAdmin { get; }

    string ModifiedBy { get; }

    string GetUserId();
}
