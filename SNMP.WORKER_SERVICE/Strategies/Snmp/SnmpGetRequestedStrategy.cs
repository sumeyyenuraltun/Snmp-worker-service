using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Snmp
{
    public class SnmpGetRequestedStrategy : IEventStrategy
    {
        private readonly ISnmpGetRequestedEventHandler _handler;
        private readonly ILogger<SnmpGetRequestedStrategy> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public SnmpGetRequestedStrategy(ISnmpGetRequestedEventHandler handler,ILogger<SnmpGetRequestedStrategy> logger, JsonSerializerOptions jsonOptions)
        {
            _handler = handler;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpGetRequestedEvent);

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
                var snmpEvent = JsonSerializer.Deserialize<SnmpGetRequestedEvent>(json,_jsonOptions);

                if (snmpEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpGetRequestedEvent. EventId:{EventId}", eventMessage.EventId);

                    return;
                }

                await _handler.HandleAsync(snmpEvent, cancellationToken);

                _logger.LogInformation("Processed SnmpGetRequestedEvent. DeviceId:{DeviceId}", snmpEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error while handling SnmpGetRequestedEvent. EventId:{EventId}",eventMessage.EventId);

                throw;
            }
        }
    }
}
