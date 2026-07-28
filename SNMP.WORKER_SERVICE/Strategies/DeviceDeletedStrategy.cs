using Snmp.EventWorker.EventHandlers.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies
{
    public class DeviceDeletedStrategy : IEventStrategy
    {
        private readonly ILogger<DeviceDeletedStrategy> _logger;
        private readonly IDeviceDeletedEventHandler _handler;

        public DeviceDeletedStrategy(ILogger<DeviceDeletedStrategy> logger, IDeviceDeletedEventHandler handler)
        {
            _logger = logger;
            _handler = handler;
        }

        public string EventType => nameof(DeviceDeletedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            var deviceDeletedEvent = JsonSerializer.Deserialize<DeviceDeletedEvent>(eventMessage.Data.ToString() ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if(deviceDeletedEvent == null)
            {
                _logger.LogWarning("DeviceDeleted deserialize failed");
                return;
            }

            deviceDeletedEvent.AggregateId = eventMessage.AggregateId;

            await _handler.HandleAsync(deviceDeletedEvent, cancellationToken);

            _logger.LogInformation("DeviceDeleted strategy completed. DeviceId: {DeviceId}", eventMessage.AggregateId);

        }
    }
}
