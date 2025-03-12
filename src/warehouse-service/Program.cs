using Consul;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.ServiceDiscovery;
using warehouse_service;
using warehouse_service.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets("b48748b7-fd77-4285-ab9d-6c13d28ef4de");
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddSingleton<IConsulClient>(consul => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri(builder.Configuration["Consul:Host"] ??
                                   throw new ArgumentException(" Consul host is not set"));
}));
builder.Services.AddSingleton<IServiceDiscovery, ConsultProvider>();
builder.Services.Configure<ServiceRegistration>(builder.Configuration.GetSection("ServiceRegistration"));
builder.Services.AddDbContextPool<WarehouseDbContext>(optionsAction =>
{
    optionsAction.UseNpgsql(builder.Configuration.GetConnectionString("warehouse"));
});
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IProuctRepository, ProductRepository>();

var app = builder.Build();

//Register service in consul
var serviceRegistration = app.Services.GetRequiredService<IOptions<ServiceRegistration>>().Value;
var serviceDiscovery = app.Services.GetRequiredService<IServiceDiscovery>();
await serviceDiscovery.RegisterServiceAsync(serviceRegistration);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("openapi/spec");
}

app.UseHttpsRedirection();
app.MapStockEndpoints();
app.MapProductsEndpoints();


await app.RunAsync();