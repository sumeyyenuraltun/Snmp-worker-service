using Snmp.Entity.Concrete;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStartedEvent: BaseEvent
    {
        public DevicePollingStartedEvent(int deviceId, string ipAddress, int port, int intervalSeconds)
        {
            DeviceId = deviceId;
            IpAddress = ipAddress;
            Port = port;
            IntervalSeconds = intervalSeconds;
            

            AggregateId = deviceId;
        }

        public int DeviceId { get; }
        public string IpAddress { get; }
        public int Port { get; }
        public int IntervalSeconds { get; }

        
    }
}
