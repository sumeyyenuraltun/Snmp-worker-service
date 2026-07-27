using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceDeletedEvent : BaseEvent
    {
        public DeviceDeletedEvent(string ıpAddress, string deviceName, DateTime deletedAt)
        {
            IpAddress = ıpAddress;
            DeviceName = deviceName;
            DeletedAt = deletedAt;
        }

        public string IpAddress { get; }
        public string DeviceName { get; }
        public DateTime DeletedAt { get; }
    }
}
