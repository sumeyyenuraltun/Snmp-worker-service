using Snmp.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Entity.Concrete
{
    public abstract class BaseEvent : IEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();

        public DateTime OccuredAt { get; set; } 

        public string EventType { get; set;  }

        public int AggregateId { get;  set;  }

        protected BaseEvent()
        {
            EventType = this.GetType().Name;

            OccuredAt = DateTime.UtcNow;
        }
    }
}
