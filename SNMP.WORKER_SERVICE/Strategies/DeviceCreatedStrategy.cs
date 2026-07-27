using Snmp.EventWorker.EventHandler.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies
{
    public class DeviceCreatedStrategy : IEventStrategy
    {
        private readonly IDeviceCreatedEventHandler _handler;
        private readonly ILogger<DeviceCreatedStrategy> _logger;

        public DeviceCreatedStrategy(IDeviceCreatedEventHandler handler, ILogger<DeviceCreatedStrategy> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public string EventType => nameof(DeviceCreatedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            var deviceCreatedEvent = JsonSerializer.Deserialize<DeviceCreatedEvent>(eventMessage.Data.ToString() ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if(deviceCreatedEvent == null)
            {
                _logger.LogWarning("DeviceCreated deserialize failed");
                return;
            }

            deviceCreatedEvent.AggregateId = eventMessage.AggregateId;

            await _handler.HandleAsync(deviceCreatedEvent, cancellationToken);

            _logger.LogInformation("DeviceCreated strategy completed. DeviceId: {DeviceId}", eventMessage.AggregateId);
        }
    }
}
