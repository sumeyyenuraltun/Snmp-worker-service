using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using Snmp.Business.DTOs.SnmpCredentials;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Clients
{
    public class SnmpV3SecurityFactory : ISnmpV3SecurityFactory
    {
        public IPrivacyProvider Create(SnmpCredentialDTO credential)
        {
            var auth = CreateAuthentication(credential);

            if (auth is null)
                return DefaultPrivacyProvider.DefaultPair;

            var privacy = CreatePrivacy(credential, auth);
            return privacy ?? new DefaultPrivacyProvider(auth);
        }

        private static IAuthenticationProvider? CreateAuthentication(SnmpCredentialDTO credential)
        {
            if (string.IsNullOrEmpty(credential.AuthPassword) || credential.AuthProtocol is null)
                return null;

            var password = new OctetString(credential.AuthPassword);

            return credential.AuthProtocol.Value switch
            {
                AuthProtocol.MD5 => new MD5AuthenticationProvider(password),
                AuthProtocol.SHA1 => new SHA1AuthenticationProvider(password),
                AuthProtocol.SHA256 => new SHA256AuthenticationProvider(password),
                AuthProtocol.SHA384 => new SHA384AuthenticationProvider(password),
                AuthProtocol.SHA512 => new SHA512AuthenticationProvider(password),
                _ => null
            };
        }

        private static IPrivacyProvider? CreatePrivacy(SnmpCredentialDTO credential, IAuthenticationProvider auth)
        {
            if (string.IsNullOrEmpty(credential.PrivacyPassword) || credential.PrivacyProtocol is null)
                return null;

            var password = new OctetString(credential.PrivacyPassword);

            return credential.PrivacyProtocol.Value switch
            {
                PrivacyProtocol.DES => new DESPrivacyProvider(password, auth),
                PrivacyProtocol.AES128 => new AESPrivacyProvider(password, auth),
                PrivacyProtocol.AES192 => new AES192PrivacyProvider(password, auth),
                PrivacyProtocol.AES256 => new AES256PrivacyProvider(password, auth),
                _ => null
            };
        }
    }
}
