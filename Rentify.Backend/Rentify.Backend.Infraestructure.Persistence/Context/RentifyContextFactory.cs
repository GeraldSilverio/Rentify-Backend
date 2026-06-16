using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Rentify.Backend.Infraestructure.Persistence.Context;

public sealed class RentifyContextFactory : IDesignTimeDbContextFactory<RentifyContext>
{
    public RentifyContext CreateDbContext(string[] args)
    {
        string connectionString = BuildConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<RentifyContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly(typeof(RentifyContext).Assembly.FullName));

        return new RentifyContext(optionsBuilder.Options);
    }

    private static string BuildConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = GetEnvironmentVariableOrDefault("DB_HOST", "localhost"),
            Port = int.Parse(GetEnvironmentVariableOrDefault("DB_PORT", "5432")),
            Database = GetEnvironmentVariableOrDefault("DB_DATABASE_NAME", "rentify"),
            Username = GetEnvironmentVariableOrDefault("DB_USER", "postgres"),
            Password = GetEnvironmentVariableOrDefault("DB_PASSWORD", "postgres")
        };

        return builder.ConnectionString;
    }

    private static string GetEnvironmentVariableOrDefault(string key, string defaultValue)
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}
