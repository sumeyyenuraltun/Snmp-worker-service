using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Walk
{
    public class SnmpWalkOperation : ISnmpWalkOperation
    {
        private readonly IEnumerable<ISnmpProvider> _provider;

        public SnmpWalkOperation(IEnumerable<ISnmpProvider> provider)
        {
            _provider = provider;
        }

        public async Task<IList<Variable>> ExecuteAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var provider = _provider.FirstOrDefault(x => x.Version == request.Credential.Version);
            if (provider == null)
            {
                throw new NotSupportedException();
            }

            return await provider.WalkAsync(request, cancellationToken);
        }
    }
}
