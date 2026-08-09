using Snmp.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEvent @events, string? correlationId,CancellationToken cancellationToken );
        Task PublishAsync(IEnumerable<IEvent> events, string? correlationId, CancellationToken cancellationToken);
    }
}
