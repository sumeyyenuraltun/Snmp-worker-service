using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.SnmpCredential
{
    public class SnmpCredentialDeletedStrategy : IEventStrategy
    {
        private readonly ILogger<SnmpCredentialDeletedStrategy> _logger;
        private readonly ISnmpCredentialDeletedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public SnmpCredentialDeletedStrategy(ILogger<SnmpCredentialDeletedStrategy> logger, ISnmpCredentialDeletedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(SnmpCredentialDeletedEvent);

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

                var snmpCredentialEvent = JsonSerializer.Deserialize<SnmpCredentialDeletedEvent>(json, _jsonOptions);

                if (snmpCredentialEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize SnmpCredentialDeletedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(snmpCredentialEvent, cancellationToken);
                _logger.LogInformation("Processed SnmpCredentialDeletedEvent. DeviceId: {DeviceId}", snmpCredentialEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling SnmpCredentialDeletedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
