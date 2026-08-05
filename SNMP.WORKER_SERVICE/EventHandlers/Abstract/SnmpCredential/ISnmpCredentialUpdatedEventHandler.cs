using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential
{
    public interface ISnmpCredentialUpdatedEventHandler
    {
        Task HandleAsync(SnmpCredentialUpdatedEvent deviceParameterUpdatedEvent, CancellationToken cancellationToken = default);
    }
}
