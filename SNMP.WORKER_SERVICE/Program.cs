//using Snmp.WorkerService;

using Microsoft.EntityFrameworkCore;
using Serilog;
using Snmp.Business.Abstract;
using Snmp.Business.Concrete;
using Snmp.Business.Mapping;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Queries.Concrete;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.EventWorker.BackgroundServices;
using Snmp.EventWorker.EventHandlers.Abstract.Device;
using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.EventHandlers.Concrete.Device;
using Snmp.EventWorker.EventHandlers.Concrete.Snmp;
using Snmp.EventWorker.Polling;
using Snmp.EventWorker.Redis.Repositories;
using Snmp.EventWorker.Redis.Services;
using Snmp.EventWorker.Snmp.Helpers;
using Snmp.EventWorker.Snmp.Manager;
using Snmp.EventWorker.Snmp.Operations.Get;
using Snmp.EventWorker.Snmp.Operations.GetNext;
using Snmp.EventWorker.Snmp.Operations.Set;
using Snmp.EventWorker.Snmp.Operations.Walk;
using Snmp.EventWorker.Snmp.Providers;
using Snmp.EventWorker.Snmp.Services;
using Snmp.EventWorker.Strategies;
using Snmp.EventWorker.Strategies.Device;
using Snmp.EventWorker.Strategies.Snmp;
using Snmp.Infrastructure.Configuration;
using SNMP.BLL.Abstract;
using SNMP.BLL.Concrete;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using StackExchange.Redis;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var host = builder.Build();
host.Run();
