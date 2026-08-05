using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpCredential
{
    public class SnmpCredentialUpdatedEvent : BaseEvent
    {
        public SnmpCredentialUpdatedEvent(int deviceId)
        {
            DeviceId = deviceId;
        }

        public int DeviceId { get; }
    }
}
