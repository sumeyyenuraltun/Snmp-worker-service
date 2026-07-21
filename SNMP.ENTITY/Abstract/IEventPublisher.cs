using Snmp.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Abstract
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEvent @events, CancellationToken cancellationToken );
        Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);
    }
}
