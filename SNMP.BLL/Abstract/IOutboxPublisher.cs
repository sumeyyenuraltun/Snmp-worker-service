using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IOutboxPublisher
    {
        Task PublishPendingMessagesAsync(CancellationToken cancellationToken = default);
    }
}
