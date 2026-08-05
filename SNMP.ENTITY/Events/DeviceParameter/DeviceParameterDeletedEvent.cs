using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.DeviceParameter
{
    public class DeviceParameterDeletedEvent : BaseEvent
    {
        public DeviceParameterDeletedEvent(int deviceId)
        {
            DeviceId = deviceId;
     
        }

        public int DeviceId { get; }
      


    }
}
