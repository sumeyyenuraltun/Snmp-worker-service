using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Abstract
{
    public interface ISnmpCredentialQueryService
    {
        Task<Result<SnmpCredentialDTO>> GetByDeviceIdAsync(int deviceId);
    }
}
