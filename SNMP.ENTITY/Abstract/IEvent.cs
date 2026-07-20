using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Entity.Abstract
{
    public interface IEvent
    {
        public int EventId { get; }
        public DateTime OccuredAt { get; }
        public string EventType { get; }
        public int AggregateId { get;}
    }
}
