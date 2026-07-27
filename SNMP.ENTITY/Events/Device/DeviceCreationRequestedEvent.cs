using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceCreationRequestedEvent : BaseEvent
    {
        public string IpAddress { get; }
        public string DeviceName { get;  }
        public int Port { get; }

        public DeviceCreationRequestedEvent(string ipAddress, string deviceName, int port)
        {
            IpAddress = ipAddress;
            DeviceName = deviceName;
            Port = port;
        }

    }
}
