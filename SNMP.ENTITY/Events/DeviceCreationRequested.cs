using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events
{
    public class DeviceCreationRequested : BaseEvent
    {
        public string IpAddress { get; }
        public string DeviceName { get;  }
        public int Port { get; }


    }
}
