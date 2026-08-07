using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Auth
{
    public class LoginRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
