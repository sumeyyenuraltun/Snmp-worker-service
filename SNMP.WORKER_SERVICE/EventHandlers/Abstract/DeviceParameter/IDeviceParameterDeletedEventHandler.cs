using SNMP.ENTITY.Events.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.DeviceParameter
{
    public interface IDeviceParameterDeletedEventHandler
    {
        Task HandleAsync(DeviceParameterDeletedEvent deviceParameterDeletedEvent, CancellationToken cancellationToken = default);
    }
}
