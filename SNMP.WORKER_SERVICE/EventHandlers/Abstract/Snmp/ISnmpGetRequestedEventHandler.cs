using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.Snmp
{
    public interface ISnmpGetRequestedEventHandler
    {
        Task HandleAsync( SnmpGetRequestedEvent snmpGetRequestedEvent, CancellationToken cancellationToken = default);
    }
}
