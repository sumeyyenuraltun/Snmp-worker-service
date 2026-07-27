using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStoppedEvent : BaseEvent
    {
        public DevicePollingStoppedEvent(int deviceId)
        {
            DeviceId = deviceId;
        }

        public int DeviceId { get;  }
    }
}
