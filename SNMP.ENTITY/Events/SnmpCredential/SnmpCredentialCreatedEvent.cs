using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpCredential
{
    public class SnmpCredentialCreatedEvent : BaseEvent
    {
        public SnmpCredentialCreatedEvent(int deviceId)
        {
            DeviceId = deviceId;
        }

        public int DeviceId { get; }
    }
}
