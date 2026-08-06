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

        public async Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(
                IPAddress.Parse(request.IpAddress),
                request.Port);

            var variables = new List<Variable>
            {
                 new Variable(new ObjectIdentifier(request.Oid))
            };

            var discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            var report = discovery.GetResponse(request.TimeoutMilliseconds, endpoint);

            var getRequest = _requestFactory.CreateV3GetRequest(request.Credential,variables,report);

            var response = await Task.Run(() => getRequest.GetResponse(request.TimeoutMilliseconds, endpoint),cancellationToken);

            if (response is ReportMessage reportMessage)
            {
                var reportVariable = reportMessage.Pdu().Variables.FirstOrDefault();

                throw new InvalidOperationException($"SNMPv3 Report received. OID: {reportVariable?.Id}, Value: {reportVariable?.Data}");
            }

            var pdu = response.Pdu();

            if (pdu == null || pdu.Variables.Count == 0)
                return null;

            return pdu.Variables[0].Data.ToString();
        }
        public async Task<string?> GetNextAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(
                IPAddress.Parse(request.IpAddress),
                request.Port);

            var variables = new List<Variable>
    {
        new Variable(new ObjectIdentifier(request.Oid))
    };

            var discovery = Messenger.GetNextDiscovery(SnmpType.GetNextRequestPdu);
            var report = discovery.GetResponse(request.TimeoutMilliseconds, endpoint);

            var getNextRequest = _requestFactory.CreateV3GetNextRequest(request.Credential,variables,report);

            var response = await Task.Run(() => getNextRequest.GetResponse(request.TimeoutMilliseconds, endpoint),cancellationToken);

            if (response is ReportMessage reportMessage)
            {
                var reportVariable = reportMessage.Pdu().Variables.FirstOrDefault();

                throw new InvalidOperationException($"SNMPv3 Report received. OID: {reportVariable?.Id}, Value: {reportVariable?.Data}");
            }

            var pdu = response.Pdu();

            if (pdu == null || pdu.Variables.Count == 0)
                return null;

            return pdu.Variables[0].Data.ToString();
        }
        public async Task<IList<Variable>> WalkAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(request.IpAddress),request.Port);

            var result = new List<Variable>();

            var discovery = Messenger.GetNextDiscovery(SnmpType.GetNextRequestPdu);
            var report = discovery.GetResponse(request.TimeoutMilliseconds, endpoint);

            var currentOid = new ObjectIdentifier(request.Oid);

            while (true)
            {
                var variables = new List<Variable>
            {
                new Variable(currentOid)
            };

                var getNextRequest = _requestFactory.CreateV3GetNextRequest(
                    request.Credential,
                    variables,
                    report);

                var response = await Task.Run(
                    () => getNextRequest.GetResponse(request.TimeoutMilliseconds, endpoint),
                    cancellationToken);

                var variable = response.Pdu().Variables.First();

                if (variable.Data is EndOfMibView)
                    break;

                if (!variable.Id.ToString().StartsWith(request.Oid))
                    break;

                result.Add(variable);

                currentOid = variable.Id;
            }

            return result;
        }
    }
}
