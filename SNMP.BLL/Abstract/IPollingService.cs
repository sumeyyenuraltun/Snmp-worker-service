using Snmp.Business.DTOs.Polling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IPollingService
    {
        Task StartAsync(StartPollingDTO startPollingDTO, CancellationToken cancellationToken);
        Task StopAsync(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken);
    }
}
