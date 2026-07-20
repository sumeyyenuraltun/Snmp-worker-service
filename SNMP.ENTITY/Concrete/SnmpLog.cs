using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Concrete
{
    public class SnmpLog : BaseEntity
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public string Oid { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
