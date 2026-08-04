using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Entity.Concrete;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Polling
{
    public class PollingContext
    {
        public SnmpCredentialDTO Credential { get; set; } = null!;

        public List<DeviceParameterDTO> Parameters { get; set; } = new();

        public string IpAddress { get; set; } = string.Empty;

        public int Port { get; set; }

        public int IntervalSeconds { get; set; }
    }
}
