using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.SnmpCredentials
{
    public class AddSnmpCredentialDTO
    {
        public int DeviceId { get; set; }

        public string? UserName { get; set; }
        public SnmpVersion Version { get; set; }
        public string? Community { get; set; }
        public SecurityLevel? SecurityLevel { get; set; }

        public AuthProtocol? AuthProtocol { get; set; }

        public string? AuthPassword { get; set; }

        public PrivacyProtocol? PrivacyProtocol { get; set; }

        public string? PrivacyPassword { get; set; }
    }
}
