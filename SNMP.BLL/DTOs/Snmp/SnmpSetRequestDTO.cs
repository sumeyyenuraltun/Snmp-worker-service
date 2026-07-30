using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Snmp
{
    public class SnmpSetRequestDTO
    {
        public int DeviceId { get; set; }

        public string Oid { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}
