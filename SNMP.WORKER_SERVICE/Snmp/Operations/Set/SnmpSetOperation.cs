using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Operations.Set
{
    public class SnmpSetOperation : ISnmpSetOperation
    {
        private readonly IEnumerable<ISnmpProvider> _providers;

        public SnmpSetOperation(IEnumerable<ISnmpProvider> providers)
        {
            _providers = providers;
        }

        public async Task ExecuteAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(request.IpAddress),request.Port);

            var variables = new List<Variable>
            {
                new Variable(new ObjectIdentifier(request.Oid), new OctetString(request.Value ?? string.Empty))
            };

            var setRequest = new SetRequestMessage(Messenger.NextRequestId,VersionCode.V2, new OctetString(request.Credential.Community!),variables);

            await Task.Run(
                () => setRequest.GetResponse(5000, endpoint),cancellationToken);
        }
    }
}
