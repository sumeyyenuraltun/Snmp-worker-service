using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Polling
{
    public interface IPollingManager
    {
        Task StartAsync(DevicePollingStartedEvent devicePollingStartedEvent, CancellationToken cancellationToken);
        Task StopAsync(int deviceId);
        bool IsRunning(int deviceId);
    }
}
