using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Walk
{
    public interface ISnmpWalkOperation
    {
        Task<IList<Variable>> ExecuteAsync(SnmpRequest request,CancellationToken cancellationToken = default);
    }
}
