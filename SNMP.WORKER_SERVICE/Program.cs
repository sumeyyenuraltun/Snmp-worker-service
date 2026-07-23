//using Snmp.WorkerService;


using Snmp.EventWorker.EventHandler.Device;
using Snmp.EventWorker.Services;
using Snmp.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<RabbitMQEventConsumerService>();

builder.Services.AddScoped<IDeviceCreatedEventHandler, DeviceCreatedEventHandler>();
var host = builder.Build();
host.Run();
