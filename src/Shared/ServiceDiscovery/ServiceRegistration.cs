namespace Shared.ServiceDiscovery;

public record ServiceRegistration
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
}