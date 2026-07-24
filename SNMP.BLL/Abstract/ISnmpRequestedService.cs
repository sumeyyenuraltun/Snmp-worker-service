using Snmp.Business.DTOs.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface ISnmpRequestedService
    {
        Task SendGetRequestedAsync(SnmpRequestDTO snmpRequestDTO, CancellationToken cancellationToken);
        Task SendWalkRequestedAsync(SnmpWalkRequestDTO snmpWalkRequestDTO, CancellationToken cancellationToken);
    }
}
