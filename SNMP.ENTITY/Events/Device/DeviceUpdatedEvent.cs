using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceUpdatedEvent : BaseEvent
    {
        public DeviceUpdatedEvent(int deviceId, string ipAddress, int port)
        {
            DeviceId = deviceId;
            IpAddress = ipAddress;
            Port = port;
        }

        public int DeviceId { get; }
        public string IpAddress { get; }
        public int Port { get; }
    }
}
