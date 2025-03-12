namespace Shared.ServiceDiscovery;

public interface IServiceDiscovery
{
     Task RegisterServiceAsync(ServiceRegistration serviceRegistration);
}