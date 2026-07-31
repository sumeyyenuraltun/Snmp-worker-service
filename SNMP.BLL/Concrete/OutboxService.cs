using Snmp.Business.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete
{
    public class OutboxService : IOutboxService
    {
        public Task AddMessageAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
