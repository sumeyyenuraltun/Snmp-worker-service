using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.SnmpValue
{
    public class SnmpValue
    {
        public int DeviceId { get; set; }
        public int ParameterId { get; set; }
        public string Oid { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
