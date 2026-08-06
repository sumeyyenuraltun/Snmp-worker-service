using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.User
{
    public class UpdateUserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
