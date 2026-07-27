using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Strategies
{
    public class EventMessage
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime OccuredAt { get; set; }
        public int AggregateId { get; set; }
        public object Data { get; set; } = new();
    }
}
