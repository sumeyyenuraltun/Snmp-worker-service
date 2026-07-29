//using Snmp.WorkerService;

using Microsoft.EntityFrameworkCore;
using Snmp.Business.Abstract;
using Snmp.Business.Concrete;
using Snmp.Business.Mapping;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.EventWorker.BackgroundServices;
using Snmp.EventWorker.EventHandler.Device;
using Snmp.EventWorker.EventHandlers.Device;
using Snmp.EventWorker.EventHandlers.Polling;
using Snmp.EventWorker.EventHandlers.Snmp;
using Snmp.EventWorker.Helpers;
using Snmp.EventWorker.Helpers;
using Snmp.EventWorker.Polling;
using Snmp.EventWorker.Services;
using Snmp.EventWorker.Strategies;
using Snmp.Infrastructure.Configuration;
using SNMP.DAL.Context;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.SectionName));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapProfile>());
//builder.Services.AddHostedService<RabbitMQEventConsumerService>();
builder.Services.AddHostedService<RabbitMQListener>();

builder.Services.AddScoped<IDeviceCreatedEventHandler, DeviceCreatedEventHandler>();
builder.Services.AddScoped<IDeviceDeletedEventHandler, DeviceDeletedEventHandler>();
builder.Services.AddScoped<IPollingStartedEventHandler, PollingStartedEventHandler>();
builder.Services.AddScoped<IPollingStoppedEventHandler, PollingStoppedEventHandler>();

builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventStrategy, DeviceCreatedStrategy>();
builder.Services.AddScoped<IEventStrategy, DeviceDeletedStrategy>();
builder.Services.AddScoped<IEventStrategy, DevicePollingStartedStrategy>();
builder.Services.AddScoped<IEventStrategy, DevicePollingStoppedStrategy>();

builder.Services.AddSingleton<IPollingManager, PollingManager>();

builder.Services.AddScoped<ISnmpCredentialService, SnmpCredentialService>();
builder.Services.AddScoped<IDeviceParameterService, DeviceParameterService>();
builder.Services.AddScoped<ISnmpCredentialDAL, SnmpCredentialDAL>();
builder.Services.AddScoped<IDeviceParameterDAL, DeviceParameterDAL>();

builder.Services.AddSingleton<ISnmpProviderFactory, SnmpProviderFactory>();
builder.Services.AddSingleton<ISnmpService, SnmpService>();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
var host = builder.Build();
host.Run();
