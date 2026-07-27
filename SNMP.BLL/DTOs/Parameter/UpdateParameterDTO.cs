using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Parameter
{
    public class UpdateParameterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Oid { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
    }
}
