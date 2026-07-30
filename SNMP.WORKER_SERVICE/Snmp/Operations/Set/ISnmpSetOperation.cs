using Snmp.EventWorker.Snmp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Set
{
    public interface ISnmpSetOperation
    {
        Task ExecuteAsync(SnmpRequest request, CancellationToken cancellationToken = default);
    }
}
