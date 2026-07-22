using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.Device
{
    public class DeviceCreationFailed : BaseEvent
    {

        public string IpAddress { get; }
        public string DeviceName { get;  }
        public int Port { get; }
        public string ErrorMessage { get; }
        public string? ErrorDetails { get; }
        public int RequestedBy { get;  }

        public DeviceCreationFailed(string ıpAddress, string deviceName, int port, string errorMessage, string? errorDetails, int requestedBy)
        {
            IpAddress = ıpAddress;
            DeviceName = deviceName;
            Port = port;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
            RequestedBy = requestedBy;
        }
    }
}
