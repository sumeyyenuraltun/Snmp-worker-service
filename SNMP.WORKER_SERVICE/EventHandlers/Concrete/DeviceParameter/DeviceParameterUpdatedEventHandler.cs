using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.Device;
using SNMP.ENTITY.Events.DeviceParameter;


namespace Snmp.EventWorker.EventHandlers.Concrete.DeviceParameter
{
    public class DeviceParameterUpdatedEventHandler : IDeviceParameterUpdatedEventHandler
    {
        private readonly ILogger<DeviceParameterUpdatedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public DeviceParameterUpdatedEventHandler(ILogger<DeviceParameterUpdatedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(DeviceParameterUpdatedEvent deviceParameterUpdatedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterUpdatedEvent);

            await _pollingManager.RestartAsync(deviceParameterUpdatedEvent.DeviceId, cancellationToken);

            _logger.LogInformation("Device parameter updated. Cache refreshed. DeviceId: {DeviceId}", deviceParameterUpdatedEvent.AggregateId);
            await Task.CompletedTask;


        }
    }
}

