using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.DeviceParameter
{
    public class DeviceParameterDeletedEventHandler : IDeviceParameterDeletedEventHandler
    {
        private readonly ILogger<DeviceParameterDeletedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public DeviceParameterDeletedEventHandler(ILogger<DeviceParameterDeletedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(DeviceParameterDeletedEvent deviceParameterDeletedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterDeletedEvent);

            await _pollingManager.RestartAsync(deviceParameterDeletedEvent.DeviceId,cancellationToken);

            _logger.LogInformation("Device parameter deleted. Cache refreshed. DeviceId: {DeviceId}", deviceParameterDeletedEvent.AggregateId);
            await Task.CompletedTask;
        }
    }
}
