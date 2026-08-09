using Snmp.Entity.Concrete;
using SNMP.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Abstract
{
    public interface IDeviceParameterDAL : IBaseRepository<DeviceParameter>
    {
        Task<List<DeviceParameter>> GetByDeviceIdAsync(int deviceId);
        Task<DeviceParameter?> GetByDeviceIdAndParameterIdAsync(int deviceId,int parameterId);
    }
}
