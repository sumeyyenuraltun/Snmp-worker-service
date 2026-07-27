using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Strategies
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(EventMessage eventMessage, CancellationToken cancellationToken);
    }
}
