using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.Snmp
{
    public interface IPollingStoppedEventHandler
    {
        Task HandleAsync(DevicePollingStoppedEvent devicePollingStoppedEvent, CancellationToken cancellationToken = default);
    }
}
