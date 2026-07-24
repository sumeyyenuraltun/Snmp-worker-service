//using Snmp.WorkerService;


using Snmp.EventWorker.EventHandler.Device;
using Snmp.EventWorker.Services;
using Snmp.Infrastructure.Configuration;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<RabbitMQEventConsumerService>();

builder.Services.AddScoped<IDeviceCreatedEventHandler, DeviceCreatedEventHandler>();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
var host = builder.Build();
host.Run();
