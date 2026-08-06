using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using Snmp.EventWorker.Strategies.DeviceParameter;
using SNMP.ENTITY.Events.DeviceParameter;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.SnmpCredential
{
    public class SnmpCredentialCreatedStrategy : IEventStrategy
    {
        private readonly ILogger<SnmpCredentialCreatedStrategy> _logger;
        private readonly ISnmpCredentialCreatedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public SnmpCredentialCreatedStrategy(ILogger<SnmpCredentialCreatedStrategy> logger, ISnmpCredentialCreatedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpCredentialCreatedEvent);

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

                var snmpCredentialEvent = JsonSerializer.Deserialize<SnmpCredentialCreatedEvent>(json, _jsonOptions);

                if (snmpCredentialEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpCredentialCreatedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(snmpCredentialEvent, cancellationToken);
                _logger.LogInformation("Processed SnmpCredentialCreatedEvent. DeviceId: {DeviceId}", snmpCredentialEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling SnmpCredentialCreatedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
