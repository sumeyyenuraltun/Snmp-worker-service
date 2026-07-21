using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Snmp.Entity.Abstract;
using Snmp.Infrastructure.Configuration;
using SNMP.ENTITY.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snmp.Infrastructure.Messaging
{
    public class RabbitMQEventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly RabbitMQSetting _settings;
        private readonly ILogger<RabbitMQEventPublisher> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _exchangeDeclared;
        private bool _disposed;
        private readonly SemaphoreSlim _initializationSemaphore = new SemaphoreSlim(1, 1);

        public RabbitMQEventPublisher(IOptions<RabbitMQSetting> settings, ILogger<RabbitMQEventPublisher> logger)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connectionFactory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port} with user {UserName}", _settings.HostName, _settings.Port, _settings.UserName);
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken = default) 
        {
            if (_connection != null && _channel != null && _exchangeDeclared)
                return;

            await _initializationSemaphore.WaitAsync(cancellationToken);
            try
            {
               if (_connection != null && _channel != null && _exchangeDeclared)
                    return;

               if(_connection == null)
                {
                    _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                    _logger.LogDebug("RabbitMQ connection established");
                }

                if (_channel == null)
                {
                    _channel = await _connection.CreateChannelAsync(cancellationToken : cancellationToken);
                    _logger.LogDebug("RabbitMQ channel created");
                }

                if (!_exchangeDeclared)
                {
                    await _channel.ExchangeDeclareAsync(
                        exchange: _settings.ExchangeName,
                        type: ExchangeType.Topic,
                        durable: _settings.Durable,
                        autoDelete: _settings.AutoDelete,
                        cancellationToken: cancellationToken);

                    _exchangeDeclared = true;
                    _logger.LogDebug("RabbitMQ exchange '{Exchange}' declared.", _settings.ExchangeName);
                        
                }
            }
            finally
            {
                _initializationSemaphore.Release();
            }

            await Task.CompletedTask;
        }

        public async Task PublishAsync(IEvent @events, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(@events);

            await EnsureInitializedAsync(cancellationToken);

            try
            {
                var eventMessage = new EventMessage
                {
                    EventId = @events.EventId,
                    EventType = @events.EventType,
                    OccurredAt = @events.OccuredAt,
                    AggregateId = @events.AggregateId,
                    Data = @events
                };

                var json = JsonSerializer.Serialize(eventMessage, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
               
                });

                var body = Encoding.UTF8.GetBytes(json);

                var routingKey = $"lighthouse.{@events.EventType.ToLowerInvariant()}";

                var basicProperties = new BasicProperties
                {
                    Persistent = true,
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                await _channel!.BasicPublishAsync(
                    exchange: _settings.ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: basicProperties,
                    body: body,
                    cancellationToken: cancellationToken
                    );

                _logger.LogInformation("Published event {EventType} with ID {EventId} to exchange {Exchange} with routing key {RoutingKey}", @events.EventType, @events.EventId, _settings.ExchangeName, routingKey);

            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex,"Failed to publish event {EventType} with ID {EventId}", @events.EventType, @events.EventId);
                throw;
            }
        }

        public async Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(@events);

            foreach(var @event in events)
            {
                await PublishAsync(@event, cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed) 
            { 
                _initializationSemaphore.Dispose();

                if (_channel != null)
                { 
                    await _channel.DisposeAsync();
                }

                if(_connection != null)
                {
                    await _connection.DisposeAsync();
                }
                _disposed = true;
                _logger.LogInformation("RabbitMQ Event Publisher disposed.");
            }

            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        private class EventMessage
        {
            public int EventId { get; set; } 
            public string EventType { get; set; } = string.Empty;
            public DateTime OccurredAt { get; set; }
            public int AggregateId { get; set; }
            public object Data { get; set; } = null!;
        }
    }
}
