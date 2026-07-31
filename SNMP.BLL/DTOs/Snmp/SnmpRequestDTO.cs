using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Snmp
{
    public class SnmpRequestDTO
    {
        public int DeviceId { get; set; }
        public string Oid { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
    }
}
