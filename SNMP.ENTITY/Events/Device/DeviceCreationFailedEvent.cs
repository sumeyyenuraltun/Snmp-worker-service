using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceCreationFailedEvent : BaseEvent
    {

        public string IpAddress { get; }
        public string DeviceName { get;  }
        public int Port { get; }
        public string ErrorMessage { get; }
        public string? ErrorDetails { get; }
    
        public DeviceCreationFailedEvent(string ıpAddress, string deviceName, int port, string errorMessage, string? errorDetails)
        {
            IpAddress = ıpAddress;
            DeviceName = deviceName;
            Port = port;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
            
        }
    }
}
