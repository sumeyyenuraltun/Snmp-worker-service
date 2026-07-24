using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Concrete
{
    public class Device : BaseEntity
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public int Port { get; set; }
        public bool PollingEnabled { get; set; }
        public int PollingIntervalSeconds { get; set; }
        public SnmpCredential Credential { get; set; }
        public ICollection<SnmpLog> SnmpLogs { get; set; } = new List<SnmpLog>();
    }
}
