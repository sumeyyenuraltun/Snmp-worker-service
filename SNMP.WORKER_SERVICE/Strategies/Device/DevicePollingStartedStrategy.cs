using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Device
{
    public class DevicePollingStartedStrategy : IEventStrategy
    {
        private readonly IPollingStartedEventHandler _handler;
        private readonly ILogger<DevicePollingStartedStrategy> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public DevicePollingStartedStrategy(IPollingStartedEventHandler handler, ILogger<DevicePollingStartedStrategy> logger, JsonSerializerOptions jsonOptions)
        {
            _handler = handler;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(DevicePollingStartedEvent);

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
                var pollingStartedEvent = JsonSerializer.Deserialize<DevicePollingStartedEvent>(json, _jsonOptions);

                if (pollingStartedEvent == null)
                {
                    _logger.LogWarning("Failed to deseriaize DevicePollingStartedEvent. EventId:{EventId}", eventMessage.EventId);

                    return;
                }

                await _handler.HandleAsync(pollingStartedEvent, cancellationToken);

                _logger.LogInformation("Processed DevicePollingStartedEvent. DeviceId: {DeviceId}", pollingStartedEvent.DeviceId);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while handling DevicePollingStartedEvent. EventId : {EventId}", eventMessage.EventId);

                throw;
            }
        }
    }
}
