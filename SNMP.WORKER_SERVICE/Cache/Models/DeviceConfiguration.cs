using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Cache.Models
{
    public class DeviceConfiguration
    {
        public int DeviceId { get; set; }
        public string IpAddress { get; set; } = string.Empty;

        public int Port { get; set; }

        public SnmpCredentialDTO Credential { get; set; } = null!;

        public List<DeviceParameterDTO> Parameters { get; set; } = new();
        public int IntervalSeconds { get; set; }
    }
}
