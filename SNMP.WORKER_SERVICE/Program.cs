//using Snmp.WorkerService;

using Snmp.Business.Abstract;
using Snmp.EventWorker.BackgroundServices;
using Snmp.EventWorker.EventHandler.Device;
using Snmp.EventWorker.EventHandlers.Device;
using Snmp.EventWorker.EventHandlers.Polling;
using Snmp.EventWorker.EventHandlers.Snmp;
using Snmp.Infrastructure.Configuration;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.SectionName));

//builder.Services.AddHostedService<RabbitMQEventConsumerService>();
builder.Services.AddHostedService<RabbitMQListener>();

builder.Services.AddScoped<IDeviceCreatedEventHandler, DeviceCreatedEventHandler>();
builder.Services.AddScoped<IDeviceDeletedEventHandler, DeviceDeletedEventHandler>();
builder.Services.AddScoped<IPollingStartedEventHandler, PollingStartedEventHandler>();
builder.Services.AddScoped<IPollingStoppedEventHandler, PollingStoppedEventHandler>();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
var host = builder.Build();
host.Run();
