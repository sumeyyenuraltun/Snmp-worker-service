using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Role
{
    public class UpdateRoleDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
