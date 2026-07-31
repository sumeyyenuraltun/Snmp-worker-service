using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using RabbitMQ.Client;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Providers
{
    public class SnmpV2Provider : ISnmpProvider
    {
        private readonly ILogger<SnmpV2Provider> _logger;

        public SnmpV2Provider(ILogger<SnmpV2Provider> logger)
        {
            _logger = logger;
        }

        public SnmpVersion Version => SnmpVersion.V2c;

        public async Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(request.IpAddress),request.Port);

            var variables = new List<Variable>
            {
                new Variable(new ObjectIdentifier(request.Oid))
            };

            _logger.LogInformation("SNMP V2 Request -> IP:{Ip}, Port:{Port}, Community:{Community}, OID:{Oid}",request.IpAddress, request.Port, request.Credential.Community, request.Oid);

            var result = await Task.Run(() =>
                Messenger.Get(
                    VersionCode.V2,
                    endpoint,
                    new OctetString(request.Credential.Community!),
                    variables,
                    request.TimeoutMilliseconds),
                cancellationToken);

            if (result.Count > 0)
            {
                return result[0].Data.ToString();
            }

            return null;
        }

        public async Task<IList<Variable>> WalkAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(request.IpAddress), request.Port);

            var result = new List<Variable>();

            await Task.Run(() =>
                 Messenger.Walk(
                     VersionCode.V2,
                     endpoint,
                     new OctetString(request.Credential.Community!),
                     new ObjectIdentifier(request.Oid),
                     result,
                     request.TimeoutMilliseconds,
                      WalkMode.WithinSubtree
                 ), cancellationToken);
            return result;

        }
        public async Task<string?> GetNextAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(
                IPAddress.Parse(request.IpAddress),
                request.Port);

            var variables = new List<Variable>
           {
              new Variable(new ObjectIdentifier(request.Oid))
           };

            var getNextRequest = new GetNextRequestMessage(Messenger.NextRequestId, VersionCode.V2,new OctetString(request.Credential.Community!),variables);

            var response = await Task.Run(
                () => getNextRequest.GetResponse(request.TimeoutMilliseconds, endpoint),
                cancellationToken);

            var pdu = response.Pdu();

            return pdu.Variables.Count > 0
                ? pdu.Variables[0].Data.ToString()
                : null;
        }
    }
}
