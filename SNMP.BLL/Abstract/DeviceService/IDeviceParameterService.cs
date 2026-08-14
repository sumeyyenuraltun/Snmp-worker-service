using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.DTOs.Snmp;
using Snmp.Business.Results;
using Snmp.DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.DeviceService
{
    public interface IDeviceParameterService
    {
        Task<Result<List<DeviceParameterDTO>>> GetByDeviceIdAsync(int deviceId, CancellationToken cancellationToken);

        Task<Result<DeviceParameterDTO>> GetByIdAsync(int id ,CancellationToken cancellationToken);

        Task<Result> AddAsync(AddDeviceParameterDTO addDeviceParameterDTO,CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateDeviceParameterDTO updateDeviceParameterDTO,CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id,CancellationToken cancellationToken);
        Task<Result<LatestSnmpValueDTO>> GetLatestValueAsync(int deviceId, int parameterId);
    }
}
