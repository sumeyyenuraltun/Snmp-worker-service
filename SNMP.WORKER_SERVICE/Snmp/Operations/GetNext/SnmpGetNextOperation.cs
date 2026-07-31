using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.GetNext
{
    public class SnmpGetNextOperation : ISnmpGetNextOperation
    {
        private readonly IEnumerable<ISnmpProvider> _providers;

        public SnmpGetNextOperation(IEnumerable<ISnmpProvider> providers)
        {
            _providers = providers;
        }

        public async Task<string?> ExecuteAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            var provider = _providers.FirstOrDefault(x => x.Version == request.Credential.Version);

            if (provider == null)
                throw new NotSupportedException($"SNMP Version {request.Credential.Version} is not supported.");

            return await provider.GetNextAsync(request, cancellationToken);
        }

    }
}
