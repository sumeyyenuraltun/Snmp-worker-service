using Lextm.SharpSnmpLib.Security;
using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Device
{
    public class DeviceDeletedEventHandler : IDeviceDeletedEventHandler
    {
        private readonly ILogger<DeviceDeletedEventHandler> _logger;
        private readonly IDeviceConfigurationCache _cache;

        public DeviceDeletedEventHandler(ILogger<DeviceDeletedEventHandler> logger, IDeviceConfigurationCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task HandleAsync(DeviceDeletedEvent deviceDeletedEvent, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(deviceDeletedEvent);
            _cache.Remove(deviceDeletedEvent.AggregateId);
            _logger.LogInformation("Device deleted. DeviceId={DeviceId}",deviceDeletedEvent.AggregateId);
            await Task.CompletedTask;
        }
    }
}
