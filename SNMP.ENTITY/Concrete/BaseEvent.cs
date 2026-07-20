using Snmp.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Entity.Concrete
{
    public abstract class BaseEvent : IEvent
    {
        public int EventId { get; set; }

        public DateTime OccuredAt { get; set; } 

        public string EventType { get; set;  }

        public int AggregateId { get;  set;  }

       
    }
}
