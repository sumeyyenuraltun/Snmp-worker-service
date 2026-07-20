using SNMP.BLL.Abstract;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Concrete
{
    public class DeviceService : BaseService<Device>, IDeviceService
    {
        public DeviceService(IBaseRepository<Device> repository) : base(repository)
        {
        }
    }
}
