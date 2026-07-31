using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.Snmp
{
    public interface ISnmpGetNextRequestedEventHandler
    {
        Task HandleAsync(SnmpGetNextRequestedEvent snmpGetNextRequestedEvent, CancellationToken cancellationToken = default);
    }
}
