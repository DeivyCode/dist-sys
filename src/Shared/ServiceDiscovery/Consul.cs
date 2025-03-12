using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Shared.ServiceDiscovery;

public class ConsultProvider : IServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsultProvider> _logger;
    private readonly IMemoryCache _memoryCache;


    public ConsultProvider(IConsulClient consulClient, ILogger<ConsultProvider> logger, IMemoryCache memoryCache)
    {
        _consulClient = consulClient;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task RegisterServiceAsync(ServiceRegistration serviceRegistration)
    {
        var registration = new AgentServiceRegistration
        {
            ID = serviceRegistration.Id,
            Name = serviceRegistration.Name,
            Address = serviceRegistration.Address,
            Port = serviceRegistration.Port,
        };
        _logger.LogInformation("Registering service {ServiceName} with ID {ServiceId}", serviceRegistration.Name,
            serviceRegistration.Id);
        await _consulClient.Agent.ServiceDeregister(serviceRegistration.Id);
        await _consulClient.Agent.ServiceRegister(registration);

        _logger.LogInformation("Service {ServiceName} with ID {ServiceId} registered", serviceRegistration.Name,
            serviceRegistration.Id);
    }


    public async Task<CatalogService?> GetServiceAsync(string name)
    {
        return await _memoryCache.GetOrCreateAsync(name, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            var services = await _consulClient.Catalog.Service(name);
            if (services.Response.Length != 0) return services.Response[0];
            _logger.LogWarning("Service {ServiceName} not found", name);
            return null;
        });
    }
}