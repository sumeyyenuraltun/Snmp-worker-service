using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Snmp.Infrastructure.Configuration;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Services
{
    public class RabbitMQEventConsumerService : BackgroundService
    {
        private class EventMessage
        {
            public int EventId { get; set; }
            public string EventType { get; set; } = string.Empty;
            public DateTime OccuredAt { get; set; }
            public int AggregateId { get; set; }
            public Object Payload { get; set; } = new();
        }

        private readonly ILogger<RabbitMQEventConsumerService> _logger;
        private readonly RabbitMQSetting _settings;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQEventConsumerService(ILogger<RabbitMQEventConsumerService> logger, IOptions<RabbitMQSetting> settings, IServiceProvider serviceProvider )
        {
            _logger = logger;
            _settings = settings.Value;
            this._serviceProvider = serviceProvider;
        }

        private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _settings.HostName,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    Port = _settings.Port,
                    VirtualHost = _settings.VirtualHost
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _logger.LogInformation("RabbitMQ connection establish {Host}: {Port}", _settings.HostName, _settings.Port);

                _channel = await _connection.CreateChannelAsync(cancellationToken : cancellationToken);
                _logger.LogInformation("RabbitMQ channel created");

                await _channel.ExchangeDeclareAsync(
                    exchange: _settings.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: _settings.Durable,
                    autoDelete: _settings.AutoDelete,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("RabbitMQ exchange declared: {ExchangeName}", _settings.ExchangeName);

                await _channel.QueueDeclareAsync(
                    queue: _settings.QueueName,
                    durable: _settings.Durable,
                    exclusive: false,
                    autoDelete: _settings.AutoDelete,
                    cancellationToken: cancellationToken
                    );

                _logger.LogInformation("RabbitMQ queue declared : {QueueName}", _settings.QueueName);
                await _channel.QueueBindAsync(
                    queue: _settings.QueueName,
                    exchange: _settings.ExchangeName,
                    routingKey: "snmp.device.*",
                    cancellationToken: cancellationToken
                    );

                _logger.LogInformation("RabbitMQ queue bound to exchange with routing key pattern : snmp.device.*")
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error initializing RabbitMQ");
                throw;
            }
        }

        private async Task DeviceCreatedEventAsync(EventMessage eventMessage, CancellationToken cancellationToken)
        {
            var deviceCreatedEvent = JsonSerializer.Deserialize<DeviceCreated>(eventMessage.Payload.ToString() ?? string.Empty);

            if(deviceCreatedEvent == null)
            {
                _logger.LogWarning("Failed to deserialize DeviceCreated event payload. EventId : {EventId}", eventMessage.EventId);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IDeviceCreatedEventHandler>();
            await handler.HandleAsync(deviceCreatedEvent, cancellationToken);

            _logger.LogWarning("Processed DeviceCreated event. EventId : {EventId} DeviceId: {DeviceId}", eventMessage.EventId, eventMessage.AggregateId);

        }

        private async Task ProcessEventAsync(string message, string routingKey, CancellationToken cancellationToken)
        {
            try
            {
                var eventMessage = JsonSerializer.Deserialize<EventMessage>(message);
                if(eventMessage == null)
                {
                    _logger.LogInformation("Failed to deserialize event message. RoutingKey: {RoutingKey}", routingKey);
                    return;
                }
                switch (eventMessage.EventType)
                {
                    case "DeviceCreated":
                        await DeviceCreatedEventAsync(eventMessage, cancellationToken);
                        break;
                    default:
                        _logger.LogWarning("Unhandled event type: {EventType}, EventId : {EventId}", eventMessage.EventType, eventMessage.EventId);
                        break;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error proccessing event message. RoutingKey: {RoutingKey}", routingKey);
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQEventConsumerService is starting");

            await InitializeRabbitMqAsync(stoppingToken);

            if(_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized.");
                return;
            }
            _logger.LogInformation("RabbitMQEventConsumerService has started.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    _logger.LogDebug("Received message with RoutingKey : {RoutingKey}", routingKey);
                    await ProcessEventAsync(message, routingKey, stoppingToken);
                    _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error in consumer received event");
                }
                
            };

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

    }
}
