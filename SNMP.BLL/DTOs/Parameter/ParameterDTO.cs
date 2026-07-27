using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Parameter
{
    public class ParameterDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Oid { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;
    }
}
