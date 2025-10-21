using Bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bcp.API.Extensions;

/// <summary>
/// Db migrations extensions.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply Db migrations during App loading (for simplicity and portability).
    /// </summary>
    /// <param name="app">Buit application.</param>
    public static void ApplyMigrations(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
