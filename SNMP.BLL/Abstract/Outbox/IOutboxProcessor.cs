using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.Outbox
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}
