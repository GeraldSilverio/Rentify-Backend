namespace Rentify.Backend.Core.Application.Shared.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}