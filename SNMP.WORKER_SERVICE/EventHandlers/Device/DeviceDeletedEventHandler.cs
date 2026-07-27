using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Device
{
    public class DeviceDeletedEventHandler : IDeviceDeletedEventHandler
    {
        private readonly ILogger<DeviceDeletedEventHandler> _logger;

        public DeviceDeletedEventHandler(ILogger<DeviceDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(DeviceDeletedEvent deviceDeletedEvent, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(deviceDeletedEvent);
            _logger.LogInformation("Device deleted. DeviceId={DeviceId}",deviceDeletedEvent.AggregateId);
            await Task.CompletedTask;
        }
    }
}
