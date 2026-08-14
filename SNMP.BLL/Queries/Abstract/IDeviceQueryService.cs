using Snmp.Business.DTOs.Devices;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Abstract
{
    public interface IDeviceQueryService
    {
        Task<Result<DeviceDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Result<List<DeviceDTO>>> GetAllAsync(CancellationToken cancellationToken);
    }
}
