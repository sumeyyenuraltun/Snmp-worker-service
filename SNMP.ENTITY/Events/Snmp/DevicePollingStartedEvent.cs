using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStartedEvent: BaseEvent
    {
        public DevicePollingStartedEvent(int deviceId, string ıpAddress, int port, int ıntervalSeconds)
        {
            DeviceId = deviceId;
            IpAddress = ıpAddress;
            Port = port;
            IntervalSeconds = ıntervalSeconds;

            AggregateId = deviceId;
        }

        public int DeviceId { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public int IntervalSeconds { get; }
    }
}
