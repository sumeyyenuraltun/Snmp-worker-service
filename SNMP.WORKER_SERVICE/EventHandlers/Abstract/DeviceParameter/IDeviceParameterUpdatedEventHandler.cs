using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter
{
    public interface IDeviceParameterUpdatedEventHandler
    {
        Task HandleAsync(DeviceParameterUpdatedEvent deviceParameterUpdatedEvent, CancellationToken cancellationToken = default);
    }
}
