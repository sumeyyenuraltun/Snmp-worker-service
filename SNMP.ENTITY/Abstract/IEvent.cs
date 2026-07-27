using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Entity.Abstract
{
    public interface IEvent
    {
        public Guid EventId { get;  }
        public DateTime OccuredAt { get; }
        public string EventType { get; }
        public int AggregateId { get;}
    }
}
