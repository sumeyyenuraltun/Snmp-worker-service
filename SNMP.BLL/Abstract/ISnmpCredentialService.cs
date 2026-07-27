using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface ISnmpCredentialService
    {
        Task AddAsync(AddSnmpCredentialDTO dto, CancellationToken cancellationToken);

        Task UpdateAsync(UpdateSnmpCredentialDTO dto);

        Task DeleteAsync(int id);

        Task<SnmpCredentialDTO?> GetByDeviceIdAsync(int deviceId);
        Task<List<SnmpCredentialDTO>> GetAllAsync();

        Task<SnmpCredentialDTO?> GetByIdAsync(int id);
    }
}
