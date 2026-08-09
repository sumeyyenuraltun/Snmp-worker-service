using Lextm.SharpSnmpLib.Security;
using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Clients
{
    public interface ISnmpV3SecurityFactory
    {
        IPrivacyProvider Create(SnmpCredentialDTO credential);
    }
}
