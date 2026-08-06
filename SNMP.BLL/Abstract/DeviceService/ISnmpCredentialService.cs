using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.Results;
using Snmp.DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.DeviceService
{
    public interface ISnmpCredentialService
    {
        Task<Result> AddAsync(AddSnmpCredentialDTO dto, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateSnmpCredentialDTO dto, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Result<SnmpCredentialDTO>> GetByDeviceIdAsync(int deviceId);
        Task<Result<List<SnmpCredentialDTO>>> GetAllAsync();

        Task<Result<SnmpCredentialDTO>> GetByIdAsync(int id);
    }
}
