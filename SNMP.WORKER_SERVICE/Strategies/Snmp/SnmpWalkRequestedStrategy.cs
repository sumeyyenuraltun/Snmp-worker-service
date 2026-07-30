using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Snmp
{
    public class SnmpWalkRequestedStrategy : IEventStrategy
    {
        private readonly ISnmpWalkRequestedEventHandler _handler;
        private readonly ILogger<SnmpWalkRequestedStrategy> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public SnmpWalkRequestedStrategy(ISnmpWalkRequestedEventHandler handler, ILogger<SnmpWalkRequestedStrategy> logger, JsonSerializerOptions jsonOptions)
        {
            _handler = handler;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpWalkRequestedEvent);

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
                var snmpEvent = JsonSerializer.Deserialize<SnmpWalkRequestedEvent>(json, _jsonOptions);

                if (snmpEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpWalkRequestedEvent. EventId:{EventId}", eventMessage.EventId);

                    return;
                }

                await _handler.HandleAsync(snmpEvent, cancellationToken);

                _logger.LogInformation("Processed SnmpWalkRequestedEvent. DeviceId:{DeviceId}", snmpEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling SnmpWalkRequestedEvent. EventId:{EventId}", eventMessage.EventId);

                throw;
            }
        }
    }
}
