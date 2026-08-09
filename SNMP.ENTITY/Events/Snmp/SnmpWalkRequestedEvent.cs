using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpWalkRequestedEvent : BaseEvent
    {
        public SnmpWalkRequestedEvent(int deviceId, int rootParameterId, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            RootParameterId = rootParameterId;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get; }
        public int RootParameterId { get; set; }
        public int TimeoutMilliseconds { get; set; }
    }
}
