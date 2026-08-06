using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.Outbox
{
    public interface IOutboxService
    {
        Task AddMessageAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    }
}
