using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Context;
using Snmp.Common.Configuration;
using Snmp.EventWorker.Snmp.Polling;
using Snmp.EventWorker.Strategies;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.BackgroundServices
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly ILogger<RabbitMQListener> _logger;
        private readonly RabbitMQSetting _settings;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPollingManager _pollingManager;
        public RabbitMQListener(ILogger<RabbitMQListener> logger, IOptions<RabbitMQSetting> settings, IServiceScopeFactory scopeFactory, IPollingManager pollingManager)
        {
            _logger = logger;
            _settings = settings.Value;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _scopeFactory = scopeFactory;
            _pollingManager = pollingManager;
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

                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
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
                    routingKey: "snmp.#",
                    cancellationToken: cancellationToken
                    );

                _logger.LogInformation("RabbitMQ queue bound to exchange with routing key pattern : snmp.device.*");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing RabbitMQ");
                throw;
            }
        }

        private async Task ProcessEventAsync(string message, string routingKey, CancellationToken cancellationToken)
        {
            try
            {
                var eventMessage = JsonSerializer.Deserialize<EventMessage>(message, _jsonSerializerOptions);
                if (eventMessage == null)
                {
                    _logger.LogInformation("Failed to deserialize event message. RoutingKey: {RoutingKey}", routingKey);
                    return;
                }
                using var scope = _scopeFactory.CreateScope();

                var dispatcher = scope.ServiceProvider
                    .GetRequiredService<IEventDispatcher>();

                using (LogContext.PushProperty("CorrelationId", eventMessage.CorrelationId))
                {
                    _logger.LogInformation(
                        "Processing event {EventType}. CorrelationId: {CorrelationId}",
                        eventMessage.EventType,
                        eventMessage.CorrelationId);

                    await dispatcher.DispatchAsync(
                        eventMessage,
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proccessing event message. RoutingKey: {RoutingKey}", routingKey);
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("RabbitMQEventConsumerService is starting");

                await InitializeRabbitMqAsync(stoppingToken);

                if (_channel == null)
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
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;

                        _logger.LogDebug(
                            "Received message with RoutingKey : {RoutingKey}, Body: {Body}",
                            routingKey,
                            message);

                        await ProcessEventAsync(message, routingKey, stoppingToken);

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in consumer received event");
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: _settings.QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "RabbitMQEventConsumerService is consuming from queue : {QueueName}",
                    _settings.QueueName);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("RabbitMQ listener cancellation requested.");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQEventConsumerService is stopping");

            await _pollingManager.StopAllAsync(cancellationToken);

            await base.StopAsync(cancellationToken);

            if (_channel != null)
            {
                await _channel.CloseAsync(cancellationToken);
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync(cancellationToken);
                await _connection.DisposeAsync();
            }

            _logger.LogInformation("RabbitMQEventConsumerService has stopped");
        }

    }
}
