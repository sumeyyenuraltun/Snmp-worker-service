using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpWalkRequestedEvent : BaseEvent
    {
        public SnmpWalkRequestedEvent(int deviceId, string rootOid, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            RootOid = rootOid;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get; }

        public string RootOid { get; }
        public int TimeoutMilliseconds { get; set; }
    }
}
