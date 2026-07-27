using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Device
{
    public interface IDeviceDeletedEventHandler
    {
        Task HandleAsync(DeviceDeletedEvent deviceDeletedEvent, CancellationToken cancellation);
    }
}
