using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using SNMP.ENTITY.Events.Device;
using SNMP.ENTITY.Events.DeviceParameter;
using SNMP.ENTITY.Events.Snmp;
using System.Text.Json;

namespace Snmp.EventWorker.Strategies.DeviceParameter
{
    public class DeviceParameterCreatedStrategy : IEventStrategy
    {
        private readonly ILogger<DeviceParameterCreatedStrategy> _logger;
        private readonly IDeviceParameterCreatedEventHandler _handler;
        private readonly JsonSerializerOptions _jsonOptions;

        public DeviceParameterCreatedStrategy(ILogger<DeviceParameterCreatedStrategy> logger, IDeviceParameterCreatedEventHandler handler, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _handler = handler;
            _jsonOptions = jsonOptions;
        }

        public string EventType => nameof(DeviceParameterCreatedEvent);

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

                var deviceCreatedEvent = JsonSerializer.Deserialize<DeviceParameterCreatedEvent>(json, _jsonOptions);

                if (deviceCreatedEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize DeviceParameterCreatedEvent : EventId: {EventId}", eventMessage.EventId);
                    return;
                }
                await _handler.HandleAsync(deviceCreatedEvent, cancellationToken);
                _logger.LogInformation("Processed DeviceParameterCreatedEvent. DeviceId: {DeviceId}", deviceCreatedEvent.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling DeviceParameterCreatedEvent. EventId : {EventId}", eventMessage.EventId);
                throw;
            }
        }
    }
}
