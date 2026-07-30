using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.EventWorker.Snmp.Helpers;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Operations.Get;
using Snmp.EventWorker.Snmp.Operations.GetNext;
using Snmp.EventWorker.Snmp.Operations.Set;
using Snmp.EventWorker.Snmp.Operations.Walk;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Snmp.EventWorker.Snmp.Services
{
    public class SnmpService : ISnmpService
    {
        private readonly ISnmpGetOperation _getOperation;
        private readonly ISnmpWalkOperation _walkOperation;
        private readonly ILogger<SnmpService> _logger;

        public SnmpService(ILogger<SnmpService> logger,ISnmpGetOperation getOperation, ISnmpWalkOperation walkOperation)
        {
            _logger = logger;
            _getOperation = getOperation;
            _walkOperation = walkOperation;
        }

        public async Task<string?> GetAsync(string ipAddress,int port,string oid, SnmpCredentialDTO credential,CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new SnmpRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Oid = oid,
                    Credential = credential
                };

                return await _getOperation.ExecuteAsync(request,cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"SNMP request failed. IP:{Ip}, OID:{Oid}",ipAddress, oid);

                throw;
            }
        }

        public async Task<IList<Variable>> WalkAsync(string ipAddress, int port, string oid, SnmpCredentialDTO credential, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new SnmpRequest
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Oid = oid,
                    Credential = credential
                };

                return await _walkOperation.ExecuteAsync(request, cancellationToken);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "SNMP request failed. IP:{Ip}, OID:{Oid}", ipAddress, oid);
                throw;
            }
        }
    }
}
