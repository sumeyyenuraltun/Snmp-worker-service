using Snmp.DataAccess.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Concrete
{
    public class UserDAL : BaseRepository<User>, IUserDAL
    {
        public UserDAL(AppDbContext context) : base(context)
        {
        }
    }
}
