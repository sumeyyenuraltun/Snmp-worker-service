using Snmp.Business.DTOs.User;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.Security
{
    public interface IJWTService
    {
        string CreateToken(UserAuthDTO userAuthDTO);  
    }
}
