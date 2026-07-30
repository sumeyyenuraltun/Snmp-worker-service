using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Helpers
{
    public class SnmpProviderFactory : ISnmpProviderFactory
    {
        public IAuthenticationProvider CreateAuthentication(AuthProtocol authProtocol, string authPassword)
        {
            if (string.IsNullOrEmpty(authPassword)) return null;

            return authProtocol switch
            {
                AuthProtocol.MD5 => new MD5AuthenticationProvider(new OctetString(authPassword)),
                AuthProtocol.SHA1 => new SHA1AuthenticationProvider(new OctetString(authPassword)),
                AuthProtocol.SHA384 => new SHA384AuthenticationProvider(new OctetString(authPassword)),
                AuthProtocol.SHA256 => new SHA256AuthenticationProvider(new OctetString(authPassword)),
                AuthProtocol.SHA512 => new SHA512AuthenticationProvider(new OctetString(authPassword)),

                _ => null
            };
        }

        public IPrivacyProvider CreatePrivacy(PrivacyProtocol privacyProtocol, string privacyPassword, IAuthenticationProvider authentication)
        {
            if (string.IsNullOrEmpty(privacyPassword) || authentication== null)
                return null;

            return privacyProtocol switch
            {
                PrivacyProtocol.DES => new DESPrivacyProvider(new OctetString(privacyPassword), authentication),
                PrivacyProtocol.AES192 => new AES192PrivacyProvider(new OctetString(privacyPassword), authentication),
                PrivacyProtocol.AES256 => new AES256PrivacyProvider(new OctetString(privacyPassword), authentication),

                _ => null
            };
        }
    }
}
