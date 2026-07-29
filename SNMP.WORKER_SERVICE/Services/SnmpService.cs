using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.EventWorker.Helpers;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Services
{
    public class SnmpService : ISnmpService
    {
        private readonly ILogger<SnmpService> _logger;
        private readonly ISnmpProviderFactory _providerFactory;
        public SnmpService(ILogger<SnmpService> logger, ISnmpProviderFactory snmpProviderFactory)
        {
            _logger = logger;
            _providerFactory = snmpProviderFactory;
        }

        public async Task<string?> GetAsync(
           string ipAddress,
           int port,
           string oid,
           SnmpCredentialDTO credential,
           CancellationToken cancellationToken = default)
        {
            try
            {
                return credential.Version switch
                {
                    SnmpVersion.V2c => await GetV2Async(
                        ipAddress,
                        port,
                        oid,
                        credential,
                        cancellationToken),

                    SnmpVersion.V3 => await GetV3Async(
                        ipAddress,
                        port,
                        oid,
                        credential,
                        cancellationToken),

                    _ => throw new NotSupportedException(
                        "SNMP version not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "SNMP request failed. IP:{Ip}, OID:{Oid}",
                    ipAddress,
                    oid);

                throw;
            }
        }


        private async Task<string?> GetV3Async(
            string ipAddress,
            int port,
            string oid,
            SnmpCredentialDTO credential,
            CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(
                IPAddress.Parse(ipAddress),
                port);


            var variables = new List<Variable>
            {
                new Variable(new ObjectIdentifier(oid))
            };


            var report = Discover(endpoint);


            var request = CreateV3GetRequest(
                credential,
                variables,
                report);


            var response = await Task.Run(
                () => request.GetResponse(5000, endpoint),
                cancellationToken);


            var pdu = response.Pdu();


            if (pdu?.Variables != null &&
                pdu.Variables.Count > 0)
            {
                return pdu.Variables[0].Data.ToString();
            }


            return null;
        }



        private async Task<string?> GetV2Async(
            string ipAddress,
            int port,
            string oid,
            SnmpCredentialDTO credential,
            CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(
                IPAddress.Parse(ipAddress),
                port);


            var variables = new List<Variable>
            {
                new Variable(new ObjectIdentifier(oid))
            };

            _logger.LogInformation(
               "SNMP V2 Request -> IP:{Ip}, Port:{Port}, Community:{Community}, OID:{Oid}",
               ipAddress,
               port,
               credential.Community,
               oid);

            var result = await Task.Run(() =>
                Messenger.Get(
                    VersionCode.V2,
                    endpoint,
                    new OctetString(credential.Community!),
                    variables,
                    5000),
                cancellationToken);


            if (result.Count > 0)
            {
                return result[0].Data.ToString();
            }


            return null;
        }



        private ISnmpMessage Discover(IPEndPoint endpoint)
        {
            var discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            return discovery.GetResponse(5000, endpoint);
        }

        private GetRequestMessage CreateV3GetRequest(SnmpCredentialDTO credential,IList<Variable> variables, ISnmpMessage report)
        {
            var auth = _providerFactory.CreateAuthentication(credential.AuthProtocol!.Value, credential.AuthPassword!);

            var privacy = _providerFactory.CreatePrivacy(credential.PrivacyProtocol!.Value, credential.PrivacyPassword!, auth);

            return new GetRequestMessage(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                new OctetString(credential.UserName!),
                OctetString.Empty,
                variables,
                privacy,
                Messenger.MaxMessageSize,
                report
                );

        }
        
    }
}
