using Microsoft.Extensions.DependencyInjection;

namespace Shared.ServiceDiscovery;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
    {
        services.AddSingleton<IServiceDiscovery, ConsultProvider>();
        return services;
    }
}