using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SNMP.ENTITY.Concrete
{
    public class SnmpLog : BaseEntity
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public int DeviceParameterId { get; set; }
        public DeviceParameter DeviceParameter { get; set; }

        public string Value { get; set; }
        public string Type { get; set; }
    }
}
