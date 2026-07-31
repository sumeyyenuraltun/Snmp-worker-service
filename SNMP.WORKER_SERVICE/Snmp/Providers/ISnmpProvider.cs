using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Providers
{
    public interface ISnmpProvider
    {
        SnmpVersion Version { get; }

        Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default);
        Task<IList<Variable>> WalkAsync(SnmpRequest request,CancellationToken cancellationToken = default);
        Task<string?> GetNextAsync( SnmpRequest request,CancellationToken cancellationToken = default);

    }
}
