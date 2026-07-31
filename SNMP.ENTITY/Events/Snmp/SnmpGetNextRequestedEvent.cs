using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpGetNextRequestedEvent : BaseEvent
    {
        public SnmpGetNextRequestedEvent(int deviceId, string oid, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            Oid = oid;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get; }

        public string Oid { get; }
        public int TimeoutMilliseconds { get; set; }
    }
}
