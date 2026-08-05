using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.DeviceParameter
{
    public class DeviceParameterUpdatedEvent : BaseEvent
    {
        public DeviceParameterUpdatedEvent(int deviceId)
        {
            DeviceId = deviceId;
        }

        public int DeviceId { get; }
        

    }
}

