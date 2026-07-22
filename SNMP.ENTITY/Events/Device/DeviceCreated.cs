using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceCreated : BaseEvent
    {
        public string IpAddress { get;  }
        public string DeviceName { get; }
        public int Port { get; }
        public DateTime CreatedAt { get; }

        public DeviceCreated(string ipAddress, string deviceName, int port, DateTime createdAt)
        {
            IpAddress = ipAddress;
            DeviceName = deviceName;
            Port = port;
            CreatedAt = createdAt;
        }
    }
}
