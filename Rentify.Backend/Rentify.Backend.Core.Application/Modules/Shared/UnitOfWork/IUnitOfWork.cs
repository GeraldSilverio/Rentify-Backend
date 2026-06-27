namespace Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}