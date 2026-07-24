using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStarted: BaseEvent
    {
        public DevicePollingStarted(int deviceId, int ıntervalSeconds)
        {
            DeviceId = deviceId;
            IntervalSeconds = ıntervalSeconds;
        }

        public int DeviceId { get; }

        public int IntervalSeconds { get; }
    }
}
