using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.User
{
    public class UserAuthDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
