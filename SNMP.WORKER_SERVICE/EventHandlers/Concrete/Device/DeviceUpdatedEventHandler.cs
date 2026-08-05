using Snmp.EventWorker.Cache;
using Snmp.EventWorker.EventHandlers.Abstract.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Device
{
    public class DeviceUpdatedEventHandler : IDeviceUpdatedEventHandler
    {
        private readonly ILogger<DeviceUpdatedEventHandler> _logger;
        private readonly IDeviceConfigurationCache _cache;

        public DeviceUpdatedEventHandler(ILogger<DeviceUpdatedEventHandler> logger, IDeviceConfigurationCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task HandleAsync(DeviceUpdatedEvent deviceUpdatedEvent, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(deviceUpdatedEvent);

            await _cache.RefreshAsync(deviceUpdatedEvent.AggregateId,cancellation);

            _logger.LogInformation("Device updated. Cache refreshed. DeviceId={DeviceId}",deviceUpdatedEvent.AggregateId);
        }
    }
}
