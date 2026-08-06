using Microsoft.AspNetCore.Identity;
using Snmp.Business.Abstract.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
