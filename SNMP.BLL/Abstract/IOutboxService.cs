using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IOutboxService
    {
        Task AddMessageAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    }
}
