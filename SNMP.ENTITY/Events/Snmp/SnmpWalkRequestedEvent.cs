using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpWalkRequestedEvent : BaseEvent
    {
        public SnmpWalkRequestedEvent(int deviceId, string rootOid)
        {
            DeviceId = deviceId;
            RootOid = rootOid;
        }

        public int DeviceId { get; }

        public string RootOid { get; }
    }
}
