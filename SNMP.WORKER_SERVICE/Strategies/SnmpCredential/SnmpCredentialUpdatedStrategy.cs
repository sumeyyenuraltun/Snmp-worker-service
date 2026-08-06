using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.SnmpCredential
{
    public class SnmpCredentialUpdatedStrategy : IEventStrategy
    {
        private readonly ILogger<SnmpCredentialUpdatedStrategy> _logger;
        private readonly ISnmpCredentialUpdatedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public SnmpCredentialUpdatedStrategy(ILogger<SnmpCredentialUpdatedStrategy> logger, ISnmpCredentialUpdatedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpCredentialUpdatedEvent);

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

                var snmpCredentialEvent = JsonSerializer.Deserialize<SnmpCredentialUpdatedEvent>(json, _jsonOptions);

                if (snmpCredentialEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpCredentialUpdatedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(snmpCredentialEvent, cancellationToken);
                _logger.LogInformation("Processed SnmpCredentialUpdatedEvent. DeviceId: {DeviceId}", snmpCredentialEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling SnmpCredentialUpdatedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
