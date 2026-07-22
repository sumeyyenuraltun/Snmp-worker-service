using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.SnmpLog
{
    public class SnmpLogCreationFailed : BaseEvent
    {
        public int DeviceId { get; }
        public string Oid { get; }
        public string Value { get; }
        public string Type { get; }
        public string ErrorMessage { get; }
        public string? ErrorDetails { get; }
        public int RequestedBy { get; }
        public SnmpLogCreationFailed(int deviceId, string oid, string value, string type, string errorMessage, string? errorDetails, int requestedBy)
        {
            DeviceId = deviceId;
            Oid = oid;
            Value = value;
            Type = type;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
            RequestedBy = requestedBy;
        }
    }
}
