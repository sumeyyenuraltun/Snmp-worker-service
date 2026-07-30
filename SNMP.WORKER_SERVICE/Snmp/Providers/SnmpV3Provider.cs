using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.EventWorker.Snmp.Helpers;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Providers
{
    public class SnmpV3Provider : ISnmpProvider
    {
        private readonly ISnmpRequestFactory _requestFactory;

        public SnmpV3Provider(ISnmpRequestFactory requestFactory)
        {
            _requestFactory = requestFactory;
        }

        public SnmpVersion Version => SnmpVersion.V3;

        public async Task<string?> GetAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(request.IpAddress),request.Port);

            var variables = new List<Variable>
            {
                new Variable(new ObjectIdentifier(request.Oid))
            };

            var discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            var report = discovery.GetResponse(5000, endpoint);

            var getRequest = _requestFactory.CreateV3GetRequest(request.Credential, variables,report);
      
            var response = await Task.Run(() => getRequest.GetResponse(5000, endpoint), cancellationToken);

            var pdu = response.Pdu();

            if (pdu?.Variables != null && pdu.Variables.Count > 0)
            {
                return pdu.Variables[0].Data.ToString();
            }

            return null;
        }

        public Task<IList<Variable>> WalkAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
