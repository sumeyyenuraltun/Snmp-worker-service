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
    public class DeviceParameterUpdatedStrategy : IEventStrategy
    {
        private readonly ILogger<DeviceParameterUpdatedStrategy> _logger;
        private readonly IDeviceParameterUpdatedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public DeviceParameterUpdatedStrategy(ILogger<DeviceParameterUpdatedStrategy> logger, IDeviceParameterUpdatedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType =>nameof(DeviceParameterUpdatedEvent);

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

                var deviceUpdatedEvent = JsonSerializer.Deserialize<DeviceParameterUpdatedEvent>(json, _jsonOptions);

                if (deviceUpdatedEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize DeviceParameterUpdatedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(deviceUpdatedEvent, cancellationToken);
                _logger.LogInformation("Processed DeviceParameterUpdatedEvent. DeviceId: {DeviceId}", deviceUpdatedEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling DeviceParameterUpdatedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
