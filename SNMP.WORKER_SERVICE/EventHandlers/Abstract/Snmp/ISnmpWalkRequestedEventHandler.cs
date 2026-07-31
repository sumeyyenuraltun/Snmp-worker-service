using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.Snmp
{
    public interface ISnmpWalkRequestedEventHandler
    {
        Task HandleAsync(SnmpWalkRequestedEvent snmpWalkRequestedEvent, CancellationToken cancellationToken = default);
    }
}
