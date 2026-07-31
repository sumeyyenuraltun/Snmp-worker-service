using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Snmp
{
    public class SnmpGetNextRequestedStrategy :IEventStrategy
    {
        private readonly ISnmpGetNextRequestedEventHandler _handler;
        private readonly ILogger<SnmpGetNextRequestedStrategy> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public SnmpGetNextRequestedStrategy(ISnmpGetNextRequestedEventHandler handler, ILogger<SnmpGetNextRequestedStrategy> logger, JsonSerializerOptions jsonOptions)
        {
            _handler = handler;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpGetNextRequestedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = eventMessage.Data?.ToString();

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning("Event data is empty.");
                    return;
                }
                var snmpEvent = JsonSerializer.Deserialize<SnmpGetNextRequestedEvent>(json, _jsonOptions);

                if (snmpEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpGetNextRequestedEvent. EventId:{EventId}", eventMessage.EventId);

                    return;
                }

                await _handler.HandleAsync(snmpEvent, cancellationToken);

                _logger.LogInformation("Processed SnmpGetNextRequestedEvent. DeviceId:{DeviceId}", snmpEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling SnmpGetNextRequestedEvent. EventId:{EventId}", eventMessage.EventId);

                throw;
            }
        }
    }
}
