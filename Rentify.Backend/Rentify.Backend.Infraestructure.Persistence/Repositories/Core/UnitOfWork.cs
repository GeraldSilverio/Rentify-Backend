using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly RentifyContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(RentifyContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(
                exception,
                "EF concurrency failure while saving changes. Entries: {ConcurrencyEntries}",
                BuildConcurrencyEntriesDiagnostic(exception));

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

    private static IReadOnlyList<object> BuildConcurrencyEntriesDiagnostic(DbUpdateConcurrencyException exception)
    {
        return exception.Entries.Select(entry => new
        {
            EntityType = entry.Metadata.ClrType.Name,
            State = entry.State.ToString(),
            Id = TryGetCurrentValue(entry, "Id"),
            TenantId = TryGetCurrentValue(entry, "TenantId"),
            CustomerId = TryGetCurrentValue(entry, "CustomerId"),
            CurrentValues = GetSafeCurrentValues(entry),
            OriginalConcurrencyValues = GetOriginalConcurrencyValues(entry)
        }).ToArray();
    }

    private static IReadOnlyDictionary<string, object?> GetSafeCurrentValues(EntityEntry entry)
    {
        string[] excludedProperties =
        [
            "Name",
            "Url",
            "PublicId"
        ];

        return entry.CurrentValues.Properties
            .Where(property => !excludedProperties.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(
                property => property.Name,
                property => entry.CurrentValues[property.Name]);
    }

    private static IReadOnlyDictionary<string, object?> GetOriginalConcurrencyValues(EntityEntry entry)
    {
        return entry.Metadata.GetProperties()
            .Where(property => property.IsConcurrencyToken)
            .ToDictionary(
                property => property.Name,
                property => TryGetOriginalValue(entry, property));
    }

    private static object? TryGetCurrentValue(EntityEntry entry, string propertyName)
    {
        return entry.CurrentValues.Properties.Any(property => property.Name == propertyName)
            ? entry.CurrentValues[propertyName]
            : null;
    }

    private static object? TryGetOriginalValue(EntityEntry entry, IProperty property)
    {
        try
        {
            return entry.OriginalValues[property.Name];
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
