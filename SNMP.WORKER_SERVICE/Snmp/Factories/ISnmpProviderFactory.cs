using Lextm.SharpSnmpLib.Security;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Helpers
{
    public interface ISnmpProviderFactory
    {
        IAuthenticationProvider? CreateAuthentication(AuthProtocol authProtocol, string authPassword);
        IPrivacyProvider? CreatePrivacy(PrivacyProtocol privacyProtocol, string privacyPassword, IAuthenticationProvider authentication);
    }
}
