using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Concrete
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}
