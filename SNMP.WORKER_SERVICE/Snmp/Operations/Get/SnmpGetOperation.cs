using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.EventWorker.Snmp.Helpers;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Providers;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Get
{
    public class SnmpGetOperation : ISnmpGetOperation
    {
        
        private readonly IEnumerable<ISnmpProvider> _providers;

        public SnmpGetOperation(IEnumerable<ISnmpProvider> providers)
        {
            _providers = providers;
        }

        public async Task<string?> ExecuteAsync(SnmpRequest snmpRequest, CancellationToken cancellationToken = default)
        {
            var provider = _providers.FirstOrDefault(x => x.Version == snmpRequest.Credential.Version);

            if (provider == null)
            {
                throw new NotSupportedException(
                    $"SNMP version '{snmpRequest.Credential.Version}' is not supported.");
            }

            return await provider.GetAsync(snmpRequest, cancellationToken);
        }

        

    }
}
