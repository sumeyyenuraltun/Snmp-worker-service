using Snmp.EventWorker.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Text;
using static Snmp.EventWorker.BackgroundServices.RabbitMQListener;

namespace Snmp.EventWorker.Strategies
{
    public interface IEventStrategy
    {
        public string EventType { get; }
        public Task HandleEventAsync(EventMessage eventMessage, CancellationToken cancellationToken = default);
    }
}
