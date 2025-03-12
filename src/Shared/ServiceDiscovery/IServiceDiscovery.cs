using Consul;

namespace Shared.ServiceDiscovery;

public interface IServiceDiscovery
{
    Task RegisterServiceAsync(ServiceRegistration serviceRegistration);

    Task<CatalogService?> GetServiceAsync(string name);
}