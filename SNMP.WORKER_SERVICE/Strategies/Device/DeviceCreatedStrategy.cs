using Snmp.EventWorker.EventHandlers.Abstract.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.Device
{
    public class DeviceCreatedStrategy : IEventStrategy
    {
        private readonly IDeviceCreatedEventHandler _handler;
        private readonly ILogger<DeviceCreatedStrategy> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public DeviceCreatedStrategy(IDeviceCreatedEventHandler handler, ILogger<DeviceCreatedStrategy> logger, JsonSerializerOptions jsonOptions)
        {
            _handler = handler;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(DeviceCreatedEvent);

        public async Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default)
        {
            var json = eventMessage.Data?.ToString();

            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Event data is empty.");
                return;
            }
            var deviceCreatedEvent = JsonSerializer.Deserialize<DeviceCreatedEvent>(json, _jsonOptions);

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
