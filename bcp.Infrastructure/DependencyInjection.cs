using Bcp.Application.Contracts;
using Bcp.Application.Interfaces;
using Bcp.Infrastructure.Contracts;
using Bcp.Infrastructure.Persistence;
using Bcp.Infrastructure.Repositories;
using Bcp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bcp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        _ = services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        _ = services.AddScoped<ITransactionFileService, TransactionFileService>();
        _ = services.AddScoped<ICnabFileProcessor, CnabFileProcessor>();
        _ = services.AddScoped<ITransactionParser, TransactionParser>();
        _ = services.AddScoped<IStoreResolver, StoreResolver>();
        _ = services.AddScoped<IBeneficiaryResolver, BeneficiaryResolver>();
        _ = services.AddScoped<IFileTransactionResolver, FileTransactionResolver>();
        _ = services.AddScoped<IFileNotificationService, FileNotificationService>();
        return services;
    }
}