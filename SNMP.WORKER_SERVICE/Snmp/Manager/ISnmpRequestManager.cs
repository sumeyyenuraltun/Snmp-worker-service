using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Manager
{
    public interface ISnmpRequestManager
    {
        Task ExecuteGetAsync( SnmpGetRequestedEvent snmpGetRequestedEvent,CancellationToken cancellationToken = default);

        Task ExecuteWalkAsync(SnmpWalkRequestedEvent snmpWalkRequestedEvent,CancellationToken cancellationToken = default);

        Task ExecuteGetNextAsync(SnmpGetNextRequestedEvent snmpGetNextRequestedEvent,CancellationToken cancellationToken = default);

        Task ExecuteSetAsync(SnmpSetRequestedEvent snmpSetRequestedEvent, CancellationToken cancellationToken = default);
    }
}
