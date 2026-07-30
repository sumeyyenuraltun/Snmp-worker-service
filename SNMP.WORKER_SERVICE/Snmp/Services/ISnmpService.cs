using Lextm.SharpSnmpLib;
using Snmp.Business.DTOs.SnmpCredentials;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Services
{
    public interface ISnmpService
    {
        Task<string?> GetAsync(string ipAddress,int port, string oid,SnmpCredentialDTO credentialDTO, CancellationToken cancellationToken = default);
        Task<IList<Variable>> WalkAsync(string ipAddress, int port, string oid, SnmpCredentialDTO credential, CancellationToken cancellationToken = default);
    }
}
