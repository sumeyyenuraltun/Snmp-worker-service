using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.GetNext
{
    public class SnmpGetNextOperation : ISnmpGetNextOperation
    {
        public Task<Variable?> ExecuteAsync(SnmpRequest snmpRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
