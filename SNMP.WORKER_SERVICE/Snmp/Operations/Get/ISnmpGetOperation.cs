using Snmp.EventWorker.Snmp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Get
{
    public interface ISnmpGetOperation
    {
        Task<string?> ExecuteAsync (SnmpRequest snmpRequest, CancellationToken cancellationToken = default);
    }
}
