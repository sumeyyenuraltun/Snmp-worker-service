using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Parameter
{
    public class AddParameterDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Oid { get; set; } = string.Empty;
        public SnmpDataType DataType { get; set; }
        public string? Description { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
