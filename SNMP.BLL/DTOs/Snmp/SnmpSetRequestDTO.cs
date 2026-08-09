using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Snmp
{
    public class SnmpSetRequestDTO
    {
        public int DeviceId { get; set; }
        public int ParameterId { get; set; }
        public string Value { get; set; } = string.Empty;
        public int TimeoutMilliseconds { get; set; } = 5000;
    }
}
