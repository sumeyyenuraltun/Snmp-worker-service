using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Clients
{
    public class SnmpV3Client :ISnmpClient
    {
        private readonly ISnmpV3SecurityFactory _securityFactory;
        private readonly ILogger<SnmpV3Client> _logger;

        public SnmpV3Client(ISnmpV3SecurityFactory securityFactory, ILogger<SnmpV3Client> logger)
        {
            _securityFactory = securityFactory;
            _logger = logger;
        }

        public SnmpVersion Version => SnmpVersion.V3;

        public async Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);
            var report = Discover(SnmpType.GetRequestPdu, request, endpoint);

            var message = new GetRequestMessage(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                UserName(request),
                OctetString.Empty,
                SingleVariable(request.Oid),
                _securityFactory.Create(request.Credential),
                Messenger.MaxMessageSize,
                report);

            var response = await SendAsync(message, request, endpoint, cancellationToken);

            return FirstValueOrNull(response);
        }

        public async Task<string?> GetNextAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);
            var report = Discover(SnmpType.GetNextRequestPdu, request, endpoint);

            var message = CreateGetNextMessage(request, request.Oid, report);

            var response = await SendAsync(message, request, endpoint, cancellationToken);

            return FirstValueOrNull(response);
        }

        public async Task SetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);
            var report = Discover(SnmpType.SetRequestPdu, request, endpoint);

            var variables = new List<Variable>
           {
                new(new ObjectIdentifier(request.Oid),
                  SnmpDataFactory.Create(request))
           };

            var message = new SetRequestMessage(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                UserName(request),
                OctetString.Empty,
                variables,
                _securityFactory.Create(request.Credential),
                Messenger.MaxMessageSize,
                report);

            await SendAsync(message, request, endpoint, cancellationToken);
        }

        public async Task<IList<Variable>> WalkAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = CreateEndpoint(request);
            var report = Discover(SnmpType.GetNextRequestPdu, request, endpoint);

            var result = new List<Variable>();
            var currentOid = request.Oid;

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = CreateGetNextMessage(request, currentOid, report);

                var response = await SendAsync(message, request, endpoint, cancellationToken);

                var variable = response.Pdu().Variables.FirstOrDefault();

                if (variable is null || variable.Data is EndOfMibView)
                    break;

                if (!variable.Id.ToString().StartsWith(request.Oid))
                    break;

                result.Add(variable);
                currentOid = variable.Id.ToString();
            }

            return result;
        }


        private static IPEndPoint CreateEndpoint(SnmpRequest request) =>
            new(IPAddress.Parse(request.IpAddress), request.Port);

        private static OctetString UserName(SnmpRequest request) =>
            new(request.Credential.UserName!);

        private static List<Variable> SingleVariable(string oid) =>
            new() { new Variable(new ObjectIdentifier(oid)) };

        private static ISnmpMessage Discover(SnmpType requestType, SnmpRequest request, IPEndPoint endpoint)
        {
            var discovery = Messenger.GetNextDiscovery(requestType);
            return discovery.GetResponse(request.TimeoutMilliseconds, endpoint);
        }

        private GetNextRequestMessage CreateGetNextMessage(SnmpRequest request, string oid, ISnmpMessage report) =>
            new(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                UserName(request),
                OctetString.Empty,
                SingleVariable(oid),
                _securityFactory.Create(request.Credential),
                Messenger.MaxMessageSize,
                report);

        private static async Task<ISnmpMessage> SendAsync(
            ISnmpMessage message,
            SnmpRequest request,
            IPEndPoint endpoint,
            CancellationToken cancellationToken)
        {
            var response = await Task.Run(
                () => message.GetResponse(request.TimeoutMilliseconds, endpoint),
                cancellationToken);

            if (response is ReportMessage reportMessage)
            {
                var reportVariable = reportMessage.Pdu().Variables.FirstOrDefault();

                throw new InvalidOperationException(
                    $"SNMPv3 Report received. OID: {reportVariable?.Id}, Value: {reportVariable?.Data}");
            }

            return response;
        }

        private static string? FirstValueOrNull(ISnmpMessage response)
        {
            var pdu = response.Pdu();

            return pdu is not null && pdu.Variables.Count > 0
                ? pdu.Variables[0].Data.ToString()
                : null;
        }
        
    }
}

