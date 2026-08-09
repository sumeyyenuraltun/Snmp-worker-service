using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Snmp
{
    public class LatestSnmpValueDTO
    {
        public int DeviceId { get; set; }
        public int ParameterId { get; set; }
        public string Oid { get; set; } = string.Empty;
        public string? Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
