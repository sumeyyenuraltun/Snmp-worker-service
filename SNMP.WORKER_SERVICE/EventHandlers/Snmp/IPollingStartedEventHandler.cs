using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Polling
{
    public interface IPollingStartedEventHandler
    {
        Task HandleAsync(DevicePollingStartedEvent devicePollingStartedEvent, CancellationToken cancellation = default);
    }
}
