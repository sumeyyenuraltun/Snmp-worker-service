using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Snmp.Business.DTOs.SnmpCredentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Helpers
{
    public class SnmpRequestFactory : ISnmpRequestFactory
    {
        private readonly ISnmpProviderFactory _providerFactory;

        public SnmpRequestFactory(ISnmpProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
        public GetRequestMessage CreateV3GetRequest(SnmpCredentialDTO snmpCredentialDTO, IList<Variable> variables, ISnmpMessage report)
        {
            var auth = _providerFactory.CreateAuthentication(snmpCredentialDTO.AuthProtocol!.Value, snmpCredentialDTO.AuthPassword!);

            var privacy = _providerFactory.CreatePrivacy(snmpCredentialDTO.PrivacyProtocol!.Value, snmpCredentialDTO.PrivacyPassword!, auth);

            return new GetRequestMessage(VersionCode.V3,
            Messenger.NextMessageId,
            Messenger.NextRequestId,
            new OctetString(snmpCredentialDTO.UserName!),
            OctetString.Empty,
            variables,
            privacy,
            Messenger.MaxMessageSize,
            report);
        }
    }
}
