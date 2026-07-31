using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Snmp.Entity.Concrete
{
    public class DeviceParameter : BaseEntity
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public bool IsEnabled { get; set; }

        public int PollingIntervalSeconds { get; set; }

        public double? MinThreshold { get; set; }

        public double? MaxThreshold { get; set; }
    }
}
