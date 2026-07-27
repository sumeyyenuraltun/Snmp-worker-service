using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpLog
{
    public class SnmpLogCreationRequested : BaseEvent
    {
        public int DeviceId { get; }
        public int DeviceParameterId { get; }
        public string Value { get; }
        public string Type { get; }

        public SnmpLogCreationRequested(int deviceId,int deviceParameterId,string value,string type)
        {
            DeviceId = deviceId;
            DeviceParameterId = deviceParameterId;
            Value = value;
            Type = type;
        }
    }
}
