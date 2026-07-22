using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpLog
{
    public class SnmpLogCreationRequested : BaseEvent
    {
        public int DeviceId { get; }
        public string Oid { get; }
        public string Value { get; }
        public string Type { get; }
        public SnmpLogCreationRequested(int deviceId, string oid, string value, string type)
        {
            DeviceId = deviceId;
            Oid = oid;
            Value = value;
            Type = type;
        }
    }
}
