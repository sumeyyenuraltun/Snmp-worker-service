using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Concrete
{
    public class OutboxMessage : BaseEntity
    {
        public string EventType { get; set; } = null!;

        public string Payload { get; set; } = null!;

        public DateTime OccurredOn { get; set; }

        public DateTime? ProcessedOn { get; set; }

        public bool IsProcessed { get; set; }
    }
}
