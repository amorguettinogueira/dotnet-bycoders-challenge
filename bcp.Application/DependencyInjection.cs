using bcp.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bcp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}