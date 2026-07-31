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
        private readonly ISnmpGetNextOperation _getNextOperation;
        private readonly ISnmpWalkOperation _walkOperation;
        private readonly ISnmpSetOperation _setOperation;
        private readonly ILogger<SnmpService> _logger;

        public SnmpService(ISnmpGetOperation getOperation, ISnmpGetNextOperation getNextOperation, ISnmpWalkOperation walkOperation, ISnmpSetOperation setOperation, ILogger<SnmpService> logger)
        {
            _getOperation = getOperation;
            _getNextOperation = getNextOperation;
            _walkOperation = walkOperation;
            _setOperation = setOperation;
            _logger = logger;
        }

        public async Task<string?> GetAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            try
            {
                return await _getOperation.ExecuteAsync(
                    request,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "SNMP GET failed. IP:{Ip}, OID:{Oid}",
                    request.IpAddress,
                    request.Oid);

                throw;
            }
        }

        public async Task<IList<Variable>> WalkAsync(SnmpRequest request,CancellationToken cancellationToken = default)
        {
            try
            {
                return await _walkOperation.ExecuteAsync(
                    request,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "SNMP WALK failed. IP:{Ip}, OID:{Oid}",
                    request.IpAddress,
                    request.Oid);

                throw;
            }
        }
        public async Task<string?> GetNextAsync( SnmpRequest request,CancellationToken cancellationToken = default)
        {
            try
            {
                return await _getNextOperation.ExecuteAsync(request,cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"SNMP GETNEXT failed. IP:{Ip}, OID:{Oid}", request.IpAddress, request.Oid);

                throw;
            }
        }
        public async Task SetAsync(SnmpRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await _setOperation.ExecuteAsync( request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(  ex, "SNMP SET failed. IP:{Ip}, OID:{Oid}", request.IpAddress, request.Oid);

                throw;
            }
        }
    }
}
