using Snmp.DataAccess.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Concrete
{
    public class OutboxMessageDAL : BaseRepository<OutboxMessage>, IOutboxMessageDAL
    {
        public OutboxMessageDAL(AppDbContext context) : base(context)
        {
        }
    }
}
