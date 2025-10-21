using DotNetEnv;

namespace Bcp.Infrastructure.Configuration;

public static class ConnectionStringBuilder
{
    private const string MissingEnvironmentVariable = "You are missing an environment variable '{0}'. Check README.md for setup.";
    private const string PostgresDb = "POSTGRES_DB";
    private const string PostgresUser = "POSTGRES_USER";
    private const string PostgresPassword = "POSTGRES_PASSWORD";

    public static string BuildFromEnvironment()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(PostgresDb)))
        {
            _ = Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
        }

        var dbName = Environment.GetEnvironmentVariable(PostgresDb) ??
            throw new InvalidDataException(string.Format(MissingEnvironmentVariable, PostgresDb));
        var dbUser = Environment.GetEnvironmentVariable(PostgresUser) ??
            throw new InvalidDataException(string.Format(MissingEnvironmentVariable, PostgresUser));
        var dbPassword = Environment.GetEnvironmentVariable(PostgresPassword) ??
            throw new InvalidDataException(string.Format(MissingEnvironmentVariable, PostgresPassword));

        return $"Host=db;Database={dbName};Username={dbUser};Password={dbPassword}";
    }
}