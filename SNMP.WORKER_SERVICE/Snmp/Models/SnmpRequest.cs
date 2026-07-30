using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Models
{
    public class SnmpRequest
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Oid { get; set; }
        public SnmpCredentialDTO Credential { get; set; } = null!;

    }
}
