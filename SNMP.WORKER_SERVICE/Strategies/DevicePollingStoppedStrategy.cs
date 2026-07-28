using Snmp.EventWorker.EventHandlers.Polling;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies
{
    public class DevicePollingStoppedStrategy : IEventStrategy
    {
        private readonly ILogger<DevicePollingStoppedStrategy> _logger;
        private readonly IPollingStoppedEventHandler _handler;

        public DevicePollingStoppedStrategy(ILogger<DevicePollingStoppedStrategy> logger, IPollingStoppedEventHandler handler)
        {
            _logger = logger;
            _handler = handler;
        }

        public string EventType => nameof(DevicePollingStoppedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            try
            {
                var pollingStoppedEvent =JsonSerializer.Deserialize<DevicePollingStoppedEvent>(eventMessage.Data.ToString()!,new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
