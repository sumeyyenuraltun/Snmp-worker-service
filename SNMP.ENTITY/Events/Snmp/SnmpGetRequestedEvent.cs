using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpGetRequestedEvent : BaseEvent
    {
        public SnmpGetRequestedEvent(int deviceId, int parameterId, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            ParameterId = parameterId;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get;  }
        public int ParameterId { get; set; }
        public int TimeoutMilliseconds { get; set; }
    }
}
