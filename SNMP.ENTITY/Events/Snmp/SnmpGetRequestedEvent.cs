using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpGetRequestedEvent : BaseEvent
    {
        public SnmpGetRequestedEvent(int deviceId, string oid)
        {
            DeviceId = deviceId;
            Oid = oid;
        }

        public int DeviceId { get;  }

        public string Oid { get; }
    }
}
