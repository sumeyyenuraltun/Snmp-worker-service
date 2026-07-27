using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Concrete
{
    public class ParameterDAL : BaseRepository<Parameter>, IParameterDAL
    {
        public ParameterDAL(AppDbContext context) : base(context)
        {
        }
    }
}
