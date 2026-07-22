using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.SnmpLogs
{
    public class SnmpLogDTO
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceIpAddress { get; set; }
        public string Oid { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
