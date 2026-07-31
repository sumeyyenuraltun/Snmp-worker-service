using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpSetRequestedEvent : BaseEvent
    {
        public SnmpSetRequestedEvent(int deviceId, string oid, string value, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            Oid = oid;
            Value = value;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get; }

        public string Oid { get; }
        public string Value { get; }
        public int TimeoutMilliseconds { get; set; }

    }
}
