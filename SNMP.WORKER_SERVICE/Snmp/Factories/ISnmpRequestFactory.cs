using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Helpers
{
    public interface ISnmpRequestFactory
    {
        GetRequestMessage CreateV3GetRequest(SnmpCredentialDTO snmpCredentialDTO, IList<Variable> variables, ISnmpMessage report);

    }
}
