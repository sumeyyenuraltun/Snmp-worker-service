using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Snmp.Business.DTOs.SnmpCredentials;
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

        public SnmpService(ILogger<SnmpService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> GetAsync(string ipAddress, int port, string oid, SnmpCredentialDTO snmpCredentialDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                var authProvider = GetAuthProvider(snmpCredentialDTO.AuthProtocol, snmpCredentialDTO.AuthPassword);
                var privProvider = GetPrivProvider(snmpCredentialDTO.PrivacyProtocol, snmpCredentialDTO.PrivacyPassword,authProvider);

                var user = new OctetString(snmpCredentialDTO.UserName);

                var authentication = authProvider ?? DefaultAuthenticationProvider.Instance;
                var privacy = privProvider ?? new DefaultPrivacyProvider(authentication);

                var vList = new List<Variable>
                {
                    new Variable(new ObjectIdentifier(oid))
                };

                var result = await Task.Run(() => Messenger.Get(
                    VersionCode.V3,
                    endpoint,
                    user,
                    authentication,
                    privacy,
                    WalkMode.WithinSubtree,
                    vList,
                    60000
                ), cancellationToken);

                if (result != null && result.Count > 0)
                {
                    return result[0].Data.ToString();
                }

                return null;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "SNMP v3 Get request failed for IP: {IpAddress}, OID: {Oid}", ipAddress, oid);
                throw;
            }
        }

        private IAuthenticationProvider? GetAuthProvider(AuthProtocol authProtocol, string password)
        {
            if (string.IsNullOrEmpty(password)) return null;

            return authProtocol switch
            {
                AuthProtocol.MD5 =>new  MD5AuthenticationProvider(new OctetString(password)),
                AuthProtocol.SHA1 =>new SHA1AuthenticationProvider(new OctetString(password)),
                AuthProtocol.SHA384 => new SHA384AuthenticationProvider(new OctetString(password)),
                AuthProtocol.SHA256 => new SHA256AuthenticationProvider(new OctetString(password)),
                AuthProtocol.SHA512 => new SHA512AuthenticationProvider(new OctetString(password)),

                _ => null
            };
        }

        private IPrivacyProvider? GetPrivProvider(PrivacyProtocol privProtocol, string privPassword, IAuthenticationProvider? authProvider)
        {
            if(string.IsNullOrEmpty(privPassword) || authProvider==null) 
                return null;

            return privProtocol switch
            {
                PrivacyProtocol.DES => new DESPrivacyProvider(new OctetString(privPassword),authProvider),
                PrivacyProtocol.AES192 => new AES192PrivacyProvider(new OctetString(privPassword),authProvider),
                PrivacyProtocol.AES256 => new AES256PrivacyProvider(new OctetString(privPassword),authProvider),

                _ => null
            };
        }
    }
}
