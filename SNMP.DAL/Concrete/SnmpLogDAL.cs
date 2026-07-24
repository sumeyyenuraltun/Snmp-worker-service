using Snmp.DataAccess.Concrete;
using SNMP.DAL.Abstract;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.DAL.Concrete
{
    public class SnmpLogDAL : RedisBaseRepository<SnmpLog>, ISnmpLogDAL
    {
        public SnmpLogDAL(IConnectionMultiplexer redis) : base(redis)
        {
        }
    }
}
