using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandler.Device
{
    public interface IDeviceCreatedEventHandler
    {
        Task HandleAsync(DeviceCreated deviceCreatedEvent, CancellationToken cancellationToken = default);

    }
}
