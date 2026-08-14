using Snmp.Business.DTOs.Devices;
using Snmp.Business.Results;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.DeviceService
{
    public interface IDeviceService 
    {
        Task<Result> AddAsync(AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateDeviceDTO updateDeviceDTO, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Result<List<DeviceDTO>>> GetAllAsync(CancellationToken cancellationToken);

        Task<Result<DeviceDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
