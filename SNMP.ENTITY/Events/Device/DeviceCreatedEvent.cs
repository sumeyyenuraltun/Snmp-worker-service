using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceCreatedEvent : BaseEvent
    {
        public string IpAddress { get;  }
        public string DeviceName { get; }
        public int Port { get; }
        public DateTime CreatedAt { get; }

        public DeviceCreatedEvent(string ipAddress, string deviceName, int port, DateTime createdAt)
        {
            IpAddress = ipAddress;
            DeviceName = deviceName;
            Port = port;
            CreatedAt = createdAt;
        }
    }
}
