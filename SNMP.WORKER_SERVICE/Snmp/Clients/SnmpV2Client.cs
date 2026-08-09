using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System.Net;

namespace Snmp.EventWorker.Snmp.Clients
{
    public class SnmpV2Client : ISnmpClient
    {
        private readonly ILogger<SnmpV2Client> _logger;

        public SnmpV2Client(ILogger<SnmpV2Client> logger)
        {
            _logger = logger;
        }

        public SnmpVersion Version => SnmpVersion.V2c;

        public async Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);

            _logger.LogDebug("SNMPv2 GET -> IP:{Ip}, Port:{Port}, OID:{Oid}", request.IpAddress, request.Port, request.Oid);

            var result = await Task.Run(() =>
                Messenger.Get(
                    VersionCode.V2,
                    endpoint,
                    Community(request),
                    SingleVariable(request.Oid),
                    request.TimeoutMilliseconds),
                cancellationToken);

            return result.Count > 0 ? result[0].Data.ToString() : null;
        }

        public async Task<string?> GetNextAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);

            var message = new GetNextRequestMessage(
                Messenger.NextRequestId,
                VersionCode.V2,
                Community(request),
                SingleVariable(request.Oid));

            var response = await Task.Run(
                () => message.GetResponse(request.TimeoutMilliseconds, endpoint),
                cancellationToken);

            var pdu = response.Pdu();

            return pdu.Variables.Count > 0 ? pdu.Variables[0].Data.ToString() : null;
        }

        public async Task SetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);

            var variables = new List<Variable>
            {
                new(new ObjectIdentifier(request.Oid), new OctetString(request.Value ?? string.Empty))
            };

            var message = new SetRequestMessage(
                Messenger.NextRequestId,
                VersionCode.V2,
                Community(request),
                variables);

            await Task.Run(
                () => message.GetResponse(request.TimeoutMilliseconds, endpoint),
                cancellationToken);
        }

        public async Task<IList<Variable>> WalkAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);
            var result = new List<Variable>();

            await Task.Run(() =>
                Messenger.Walk(
                    VersionCode.V2,
                    endpoint,
                    Community(request),
                    new ObjectIdentifier(request.Oid),
                    result,
                    request.TimeoutMilliseconds,
                    WalkMode.WithinSubtree),
                cancellationToken);

            return result;
        }

        private static IPEndPoint CreateEndpoint(SnmpRequest request) =>
            new(IPAddress.Parse(request.IpAddress), request.Port);

        private static OctetString Community(SnmpRequest request) =>
            new(request.Credential.Community!);

        private static List<Variable> SingleVariable(string oid) =>
            new() { new Variable(new ObjectIdentifier(oid)) };
    }
}
