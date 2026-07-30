using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Device
{
    public class DevicePollingStoppedStrategy : IEventStrategy
    {
        private readonly ILogger<DevicePollingStoppedStrategy> _logger;
        private readonly IPollingStoppedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;
        public DevicePollingStoppedStrategy(ILogger<DevicePollingStoppedStrategy> logger, IPollingStoppedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(DevicePollingStoppedEvent);

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

                var pollingStoppedEvent =JsonSerializer.Deserialize<DevicePollingStoppedEvent>(json,_jsonOptions);

                if (pollingStoppedEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize DevicePollingStopedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(pollingStoppedEvent, cancellationToken);
                _logger.LogInformation("Processed DevicePollingStoppedEvent. DeviceId: {DeviceId}", pollingStoppedEvent.DeviceId);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while handling DevicePollingStoppedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
            
            
        }
    }
}
