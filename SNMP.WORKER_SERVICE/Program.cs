//using Snmp.WorkerService;

using Snmp.EventWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddInfrastructure(builder.Configuration).WithSecretVault().WithStorage();

builder.Services.AddHostedService<RabbitMQEventConsumerService>();

var host = builder.Build();
host.Run();
