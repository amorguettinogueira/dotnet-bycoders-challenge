using bcp.Application.Configuration;
using bcp.Application.Interfaces;
using bcp.Infrastructure.Persistence;
using bcp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace bcp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        _ = services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        _ = services.AddScoped<ITransactionFileService, TransactionFileService>();
        return services;
    }
}