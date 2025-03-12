using Consul;
using order_service;
using Shared.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.Configure<ServiceRegistration>(builder.Configuration.GetSection(nameof(ServiceRegistration)));
builder.Services.AddSingleton<IConsulClient>(consul => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri(builder.Configuration["Consul:Host"] ??
                                   throw new ArgumentException(" Consul host is not set"));
}));
builder.Services.AddSingleton<IServiceDiscovery, ConsultProvider>();
builder.Services.AddWareHouseHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();