using SNMP.BLL.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Concrete
{
    public class SnmpLogService : BaseService<SnmpLog>, ISnmpLogService
    {
        public SnmpLogService(IBaseRepository<SnmpLog> repository) : base(repository)
        {
        }
    }
}
