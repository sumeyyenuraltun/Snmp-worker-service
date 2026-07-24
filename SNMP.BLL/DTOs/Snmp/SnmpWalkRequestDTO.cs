using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Snmp
{
    public class SnmpWalkRequestDTO
    {
        public int DeviceId { get; set; }
        public string RootOid { get; set; }
    }
}
