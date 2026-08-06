using Snmp.Business.Abstract.Security;
using Snmp.WebAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete.Security
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtSettings;
        public string CreateToken(int userId, string username)
        {
            throw new NotImplementedException();
        }
    }
}
