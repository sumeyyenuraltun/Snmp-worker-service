using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.Security
{
    public interface IJWTService
    {
        string CreateToken(int userId, string username);  
    }
}
