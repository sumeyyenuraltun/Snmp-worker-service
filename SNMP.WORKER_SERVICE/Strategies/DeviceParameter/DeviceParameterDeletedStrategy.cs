using Snmp.EventWorker.EventHandlers.Abstract.Device;
using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.Strategies.Device;
using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.DeviceParameter
{
    public class DeviceParameterDeletedStrategy : IEventStrategy
    {
        private readonly ILogger<DeviceParameterDeletedStrategy> _logger;
        private readonly IDeviceParameterDeletedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public DeviceParameterDeletedStrategy(ILogger<DeviceParameterDeletedStrategy> logger, IDeviceParameterDeletedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType =>nameof(DeviceParameterDeletedEvent);

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

                var deviceDeletedEvent = JsonSerializer.Deserialize<DeviceParameterDeletedEvent>(json, _jsonOptions);

                if (deviceDeletedEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize DeviceParameterDeletedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(deviceDeletedEvent, cancellationToken);
                _logger.LogInformation("Processed DeviceParameterDeletedEvent. DeviceId: {DeviceId}", deviceDeletedEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling DeviceParameterDeletedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
