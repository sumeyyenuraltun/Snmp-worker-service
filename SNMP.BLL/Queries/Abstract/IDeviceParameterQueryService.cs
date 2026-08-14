using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Abstract
{
    public interface IDeviceParameterQueryService
    {
        Task<Result<List<DeviceParameterDTO>>> GetByDeviceIdAsync(int deviceId, CancellationToken cancellationToken);
        Task<Result<DeviceParameterDTO>> GetByDeviceIdAndParameterIdAsync(int deviceId,int parameterId, CancellationToken cancellationToken);
    }
}
