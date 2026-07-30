using Snmp.Business.DTOs.SnmpValue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Redis.Services
{
    public interface IRedisService
    {
        Task SaveLatestValueAsync(SnmpValue value);
        Task<SnmpValue> GetLatestValueAsync(int deviceId, int parameterId);
    }
}
