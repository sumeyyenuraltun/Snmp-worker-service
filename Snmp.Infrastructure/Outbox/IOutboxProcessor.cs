using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Infrastructure.Outbox
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}
