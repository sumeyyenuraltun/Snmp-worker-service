using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpSetRequestedEvent : BaseEvent
    {
        public SnmpSetRequestedEvent(int deviceId, string oid, string value)
        {
            DeviceId = deviceId;
            Oid = oid;
            Value = value;
        }

        public int DeviceId { get; }

        public string Oid { get; }
        public string Value { get; }

    }
}
