using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
