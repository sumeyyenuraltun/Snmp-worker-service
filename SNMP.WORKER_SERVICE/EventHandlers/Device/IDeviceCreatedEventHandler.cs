using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandler.Device
{
    public interface IDeviceCreatedEventHandler
    {
        Task HandleAsync(DeviceCreatedEvent deviceCreatedEvent, CancellationToken cancellationToken = default);

    }
}
