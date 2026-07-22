using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.SnmpLogs
{
    public class UpdateSnmpLogDTO
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
