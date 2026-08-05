using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpCredential
{
    public class SnmpCredentialDeletedEvent : BaseEvent
    {
        public SnmpCredentialDeletedEvent(int deviceId)
        {
            DeviceId = deviceId;
        }
        public int DeviceId { get; }
    }
}
