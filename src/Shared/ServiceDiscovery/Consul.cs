using Consul;
using Microsoft.Extensions.Logging;

namespace Shared.ServiceDiscovery;

public class ConsultProvider : IServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsultProvider> _logger;


    public ConsultProvider(IConsulClient consulClient, ILogger<ConsultProvider> logger)
    {
        _consulClient = consulClient;
        _logger = logger;
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
}