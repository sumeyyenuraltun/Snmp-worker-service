using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.DeviceParameter
{
    public class DeviceParameterDeletedEvent : BaseEvent
    {
        public DeviceParameterDeletedEvent(int deviceId, int deviceParameterId, int parameterId)
        {
            DeviceId = deviceId;
            DeviceParameterId = deviceParameterId;
            ParameterId = parameterId;
        }

        public int DeviceId { get; }
        public int DeviceParameterId { get; }
        public int ParameterId { get; }


    }
}
