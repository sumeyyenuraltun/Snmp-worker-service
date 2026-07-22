using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpLog
{
    public class SnmpLogCreated : BaseEvent
    {

        public int DeviceId { get; }
        public string Oid { get; }
        public string Value { get; }
        public string Type { get; }
        public DateTime CreatedAt { get; }
        public SnmpLogCreated(int deviceId, string oid, string value, string type, DateTime createdAt)
        {
            DeviceId = deviceId;
            Oid = oid;
            Value = value;
            Type = type;
            CreatedAt = createdAt;
        }

    }
}
