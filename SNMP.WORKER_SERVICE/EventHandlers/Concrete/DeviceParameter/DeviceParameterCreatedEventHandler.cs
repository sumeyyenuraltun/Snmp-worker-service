using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.DeviceParameter
{
    public class DeviceParameterCreatedEventHandler : IDeviceParameterCreatedEventHandler
    {
        private readonly ILogger<DeviceParameterCreatedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public DeviceParameterCreatedEventHandler(ILogger<DeviceParameterCreatedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(DeviceParameterCreatedEvent deviceParameterCreatedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterCreatedEvent);

            await _pollingManager.RestartAsync(deviceParameterCreatedEvent.DeviceId,cancellationToken);

            _logger.LogInformation("DeviceParameterCreatedEvent handled for DeviceId: {DeviceId}", deviceParameterCreatedEvent.AggregateId);

            await Task.CompletedTask;
        }
    }
}
