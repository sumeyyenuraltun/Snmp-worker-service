using Snmp.Business.DTOs.Polling;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.Snmp
{
    public interface IPollingService
    {
        Task<Result> StartAsync(StartPollingDTO startPollingDTO, CancellationToken cancellationToken);
        Task<Result> StopAsync(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken);
    }
}
