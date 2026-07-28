using Snmp.Business.DTOs.Devices;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface IDeviceService 
    {
        Task AddAsync(AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken);

        Task UpdateAsync(UpdateDeviceDTO updateDeviceDTO, CancellationToken cancellationToken);

        Task DeleteAsync(int id, CancellationToken cancellationToken);

        Task<List<DeviceDTO>> GetAllAsync();

        Task<DeviceDTO?> GetByIdAsync(int id);
    }
}
