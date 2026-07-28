using Snmp.EventWorker.EventHandlers.Polling;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies
{
    public class DevicePollingStartedStrategy : IEventStrategy
    {
        private readonly IPollingStartedEventHandler _handler;
        private readonly ILogger<DevicePollingStartedStrategy> _logger;

        public DevicePollingStartedStrategy(IPollingStartedEventHandler handler, ILogger<DevicePollingStartedStrategy> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public string EventType => nameof(DevicePollingStartedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            try
            {
                var pollingStartedEvent = JsonSerializer.Deserialize<DevicePollingStartedEvent>(eventMessage.Data.ToString() ?? string.Empty, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
