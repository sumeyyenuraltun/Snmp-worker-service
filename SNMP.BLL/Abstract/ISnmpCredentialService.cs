using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface ISnmpCredentialService
    {
        Task AddAsync(AddSnmpCredentialDTO dto);

        Task UpdateAsync(UpdateSnmpCredentialDTO dto);

        Task DeleteAsync(int id);

        Task<SnmpCredentialDTO?> GetByDeviceIdAsync(int deviceId);
    }
}
