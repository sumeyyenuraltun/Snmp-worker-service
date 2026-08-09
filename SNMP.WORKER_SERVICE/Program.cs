using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Snmp.Business.Abstract.Redis;
using Snmp.Business.Mapping;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Queries.Concrete;
using Snmp.Common.Configuration;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.DataAccess.Concrete.Redis;
using Snmp.EventWorker.BackgroundServices;
using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.Device;
using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using Snmp.EventWorker.EventHandlers.Concrete.Device;
using Snmp.EventWorker.EventHandlers.Concrete.DeviceParameter;
using Snmp.EventWorker.EventHandlers.Concrete.Snmp;
using Snmp.EventWorker.EventHandlers.Concrete.SnmpCredential;
using Snmp.EventWorker.Snmp.Helpers;
using Snmp.EventWorker.Snmp.Manager;
using Snmp.EventWorker.Snmp.Operations.Get;
using Snmp.EventWorker.Snmp.Operations.GetNext;
using Snmp.EventWorker.Snmp.Operations.Set;
using Snmp.EventWorker.Snmp.Operations.Walk;
using Snmp.EventWorker.Snmp.Polling;
using Snmp.EventWorker.Snmp.Providers;
using Snmp.EventWorker.Snmp.Services;
using Snmp.EventWorker.Strategies;
using Snmp.EventWorker.Strategies.Device;
using Snmp.EventWorker.Strategies.DeviceParameter;
using Snmp.EventWorker.Strategies.Snmp;
using Snmp.EventWorker.Strategies.SnmpCredential;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using StackExchange.Redis;
using System.Text.Json;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [CorrelationId:{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(
                new Uri(builder.Configuration["ElasticSearch:Uri"]!))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "snmp-worker-logs-{0:yyyy.MM}"
            });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

var redisConnection = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn") ?? "localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.SectionName));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapProfile>());

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
builder.Services.AddScoped<IEventStrategy, SnmpGetRequestedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpWalkRequestedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpGetNextRequestedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpSetRequestedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpCredentialCreatedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpCredentialUpdatedStrategy>();
builder.Services.AddScoped<IEventStrategy, SnmpCredentialDeletedStrategy>();
builder.Services.AddScoped<IEventStrategy, DeviceParameterCreatedStrategy>();
builder.Services.AddScoped<IEventStrategy, DeviceParameterDeletedStrategy>();
builder.Services.AddScoped<IEventStrategy, DeviceParameterUpdatedStrategy>();


builder.Services.AddSingleton<IPollingManager, PollingManager>();

builder.Services.AddScoped<ISnmpCredentialQueryService, SnmpCredentialQueryService>();
builder.Services.AddScoped<IDeviceParameterQueryService, DeviceParameterQueryService>();
builder.Services.AddScoped<IDeviceQueryService, DeviceQueryService>();

builder.Services.AddSingleton<ISnmpProviderFactory, SnmpProviderFactory>();
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<IRedisService, RedisService>();


builder.Services.AddSingleton<ISnmpRequestFactory, SnmpRequestFactory>();

builder.Services.AddScoped<ISnmpProvider, SnmpV2Provider>();
builder.Services.AddScoped<ISnmpProvider, SnmpV3Provider>();

builder.Services.AddScoped<ISnmpGetOperation, SnmpGetOperation>();
builder.Services.AddScoped<ISnmpGetNextOperation, SnmpGetNextOperation>();
builder.Services.AddScoped<ISnmpWalkOperation, SnmpWalkOperation>();
builder.Services.AddScoped<ISnmpSetOperation, SnmpSetOperation>();

builder.Services.AddScoped<ISnmpRequestManager, SnmpRequestManager>();

builder.Services.AddScoped<ISnmpGetRequestedEventHandler, SnmpGetRequestedEventHandler>();
builder.Services.AddScoped<ISnmpWalkRequestedEventHandler, SnmpWalkRequestedEventHandler>();
builder.Services.AddScoped<ISnmpGetNextRequestedEventHandler, SnmpGetNextRequestedEventHandler>();
builder.Services.AddScoped<ISnmpSetRequestedEventHandler, SnmpSetRequestedEventHandler>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDeviceDAL, DeviceDAL>();
builder.Services.AddScoped<ISnmpCredentialDAL, SnmpCredentialDAL>();
builder.Services.AddScoped<IDeviceParameterDAL, DeviceParameterDAL>();

builder.Services.AddScoped<ISnmpCredentialCreatedEventHandler, SnmpCredentialCreatedEventHandler>();
builder.Services.AddScoped<ISnmpCredentialUpdatedEventHandler, SnmpCredentialUpdatedEventHandler>();
builder.Services.AddScoped<ISnmpCredentialDeletedEventHandler, SnmpCredentialDeletedEventHandler>();

builder.Services.AddScoped<IDeviceParameterCreatedEventHandler, DeviceParameterCreatedEventHandler>();
builder.Services.AddScoped<IDeviceParameterUpdatedEventHandler, DeviceParameterUpdatedEventHandler>();
builder.Services.AddScoped<IDeviceParameterDeletedEventHandler, DeviceParameterDeletedEventHandler>();

builder.Services.AddSingleton<IDeviceConfigurationCache, DeviceConfigurationCache>();
var host = builder.Build();
host.Run();
