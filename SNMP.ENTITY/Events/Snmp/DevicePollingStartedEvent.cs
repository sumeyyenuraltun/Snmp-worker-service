using Snmp.Entity.Concrete;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class DevicePollingStartedEvent: BaseEvent
    {
        public DevicePollingStartedEvent(int deviceId)
        {
            DeviceId = deviceId;

            AggregateId = deviceId;
        }

        public int DeviceId { get; }
        
    }
}
