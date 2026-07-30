using Snmp.EventWorker.EventHandlers.Abstract.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Device
{
    public class DeviceDeletedStrategy : IEventStrategy
    {
        private readonly ILogger<DeviceDeletedStrategy> _logger;
        private readonly IDeviceDeletedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;
        public DeviceDeletedStrategy(ILogger<DeviceDeletedStrategy> logger, IDeviceDeletedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(DeviceDeletedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            var json = eventMessage.Data?.ToString();

            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Event data is empty.");
                return;
            }
            var deviceDeletedEvent = JsonSerializer.Deserialize<DeviceDeletedEvent>(json, _jsonOptions);

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
