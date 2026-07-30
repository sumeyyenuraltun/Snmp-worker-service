using Snmp.EventWorker.EventHandlers.Abstract.Device;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Device
{
    public class DeviceCreatedEventHandler(ILogger<DeviceCreatedEventHandler> logger) : IDeviceCreatedEventHandler
    {
        public async Task HandleAsync(DeviceCreatedEvent deviceCreatedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceCreatedEvent);

            logger.LogInformation("Handling DeviceCreated event: DeviceId={DeviceId}, Port={Port}, CreatedAt={CreatedAt}, IpAddress={IpAddress}", deviceCreatedEvent.AggregateId,deviceCreatedEvent.Port, deviceCreatedEvent.CreatedAt,deviceCreatedEvent.IpAddress);

            await Task.CompletedTask;
        }
    }
}
