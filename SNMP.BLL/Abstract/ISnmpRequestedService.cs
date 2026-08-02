using Snmp.Business.DTOs.Snmp;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface ISnmpRequestedService
    {
        Task<Result> SendGetRequestedAsync(SnmpRequestDTO snmpRequestDTO, CancellationToken cancellationToken);
        Task<Result> SendWalkRequestedAsync(SnmpWalkRequestDTO snmpWalkRequestDTO, CancellationToken cancellationToken);
        Task<Result> SendGetNextRequestedAsync(SnmpGetNextRequestDTO snmpGetNextRequestDTO, CancellationToken cancellationToken);
        Task<Result> SendSetRequestedAsync(SnmpSetRequestDTO snmpSetRequestDTO, CancellationToken cancellationToken);
    }
}
