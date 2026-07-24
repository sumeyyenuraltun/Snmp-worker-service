using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStopped : BaseEvent
    {
        public DevicePollingStopped(int deviceId)
        {
            DeviceId = deviceId;
        }

        public int DeviceId { get;  }
    }
}
