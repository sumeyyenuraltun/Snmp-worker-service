using Snmp.Business.Results;
using Snmp.EventWorker.Cache.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Cache
{
    public interface IDeviceConfigurationCache
    {
        Task<DeviceConfiguration?> GetAsync(int deviceId, CancellationToken cancellationToken);

        Task RefreshAsync(int deviceId, CancellationToken cancellationToken);
        void Remove(int deviceId);
    }
}
