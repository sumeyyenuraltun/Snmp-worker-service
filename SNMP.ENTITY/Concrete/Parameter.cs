using SNMP.ENTITY.Concrete;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Entity.Concrete
{
    public class Parameter : BaseEntity
    {
        public string Name { get; set; }
        public string Oid { get; set; }
        public SnmpDataType DataType { get; set; }
        public string Unit { get; set; }
        public ICollection<DeviceParameter> DeviceParametres { get; set; } = new List<DeviceParameter>();
    }
}
