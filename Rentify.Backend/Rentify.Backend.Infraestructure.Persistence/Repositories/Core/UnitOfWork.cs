using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly RentifyContext _context;

    public UnitOfWork(RentifyContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyException(
                "The requested data was changed or deleted by another operation.",
                exception);
        }
        catch (DbUpdateException exception) when (TryGetUniqueConstraintMessage(exception, out string message))
        {
            throw new ApiException(message, StatusCodes.Status400BadRequest);
        }
    }

    private static bool TryGetUniqueConstraintMessage(
        DbUpdateException exception,
        out string message)
    {
        message = string.Empty;

        if (exception.GetBaseException() is not PostgresException postgresException ||
            postgresException.SqlState != PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }

        string constraintName = postgresException.ConstraintName ?? string.Empty;

        if (constraintName.Contains("Tenants", StringComparison.OrdinalIgnoreCase) &&
            constraintName.Contains("Rnc", StringComparison.OrdinalIgnoreCase))
        {
            message = "Este RNC ya está en uso por otra empresa.";
            return true;
        }

        if (constraintName.Contains("Code", StringComparison.OrdinalIgnoreCase))
        {
            message = "El código ya está en uso.";
            return true;
        }

        message = "Ya existe un registro con el mismo valor único.";
        return true;
    }
}
