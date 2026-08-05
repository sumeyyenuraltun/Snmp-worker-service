using SNMP.ENTITY.Events.Device;
using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter
{
    public interface IDeviceParameterCreatedEventHandler
    {
        Task HandleAsync(DeviceParameterCreatedEvent deviceParameterCreatedEvent, CancellationToken cancellationToken = default);
    }
}
