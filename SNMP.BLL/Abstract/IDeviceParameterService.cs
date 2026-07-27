using Snmp.Business.DTOs.DeviceParameter;
using Snmp.DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IDeviceParameterService
    {
        Task<List<DeviceParameterDTO>> GetByDeviceIdAsync(int deviceId);

        Task<DeviceParameterDTO> AddAsync(AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken);

        Task DeleteAsync(int id);
        Task<DeviceParameterDTO> UpdateAsync(UpdateDeviceParameterDTO updateDeviceParameterDTO);
        Task<DeviceParameterDTO?> GetByIdAsync(int id);
    }
}
