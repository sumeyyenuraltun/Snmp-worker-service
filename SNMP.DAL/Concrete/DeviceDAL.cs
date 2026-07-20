using SNMP.DAL.Abstract;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.DAL.Concrete
{
    public class DeviceDAL : BaseRepository<Device>, IDeviceDAL
    {
        public DeviceDAL(AppDbContext context) : base(context)
        {
        }
    }
}
