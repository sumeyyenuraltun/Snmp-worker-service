using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Snmp
{
    public class SnmpSetRequestedEvent : BaseEvent
    {
        public SnmpSetRequestedEvent(int deviceId, int parameterId, string value, int timeoutMilliseconds)
        {
            DeviceId = deviceId;
            ParameterId = parameterId;
            Value = value;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public int DeviceId { get; }

        public int ParameterId { get; set; }
        public string Value { get; }
        public int TimeoutMilliseconds { get; set; }

    }
}
