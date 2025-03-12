using Shared.ServiceDiscovery;

namespace order_service;

public static class ServiceExtension
{
    public static void AddWareHouseHttpClient(this IServiceCollection services)
    {
        //check after the service is up and running consult provider is registered in consul
        var serviceProvider = services.BuildServiceProvider();
        var serviceDiscovery = serviceProvider.GetRequiredService<IServiceDiscovery>();
        var warehouseService = serviceDiscovery.GetServiceAsync("Warehouse Service").Result;

        if (warehouseService == null)
        {
            throw new Exception("Warehouse service not found");
        }

        services.AddHttpClient("WarehouseService",
            client =>
            {
                client.BaseAddress =
                    new Uri($"http://{warehouseService.ServiceAddress}:{warehouseService.ServicePort}");
            });
    }
}