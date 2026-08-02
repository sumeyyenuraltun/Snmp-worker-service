using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.DTOs.SnmpValue;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using Snmp.EventWorker.Redis.Services;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Services;
using SNMP.BLL.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Manager
{
    public class SnmpRequestManager : ISnmpRequestManager
    {
        private readonly ILogger<SnmpRequestManager> _logger;
        private readonly IDeviceDAL _deviceDAL;
        private readonly IMapper _mapper;
        private readonly ISnmpCredentialService _credentialService;
        private readonly ISnmpService _snmpService;
        private readonly IRedisService _redisService;
        private readonly IDeviceParameterDAL _deviceParameterDAL;

        public SnmpRequestManager(ILogger<SnmpRequestManager> logger, IDeviceDAL deviceDAL, IMapper mapper, ISnmpCredentialService credentialService, ISnmpService snmpService, IRedisService redisService, IDeviceParameterDAL deviceParameterDAL)
        {
            _logger = logger;
            _deviceDAL = deviceDAL;
            _mapper = mapper;
            _credentialService = credentialService;
            _snmpService = snmpService;
            _redisService = redisService;
            _deviceParameterDAL = deviceParameterDAL;
        }

        public async Task ExecuteGetAsync(SnmpGetRequestedEvent snmpGetRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpGetRequestedEvent.DeviceId, snmpGetRequestedEvent.Oid, snmpGetRequestedEvent.TimeoutMilliseconds);

            var result = await _snmpService.GetAsync(request, cancellationToken);

            if (result != null)
            {
                await _redisService.SaveLatestValueAsync(new SnmpValue
                {
                    DeviceId = snmpGetRequestedEvent.DeviceId,
                    Oid = snmpGetRequestedEvent.Oid,
                    Value = result,
                    Timestamp = DateTime.UtcNow
                    
                });
            }

            _logger.LogInformation( "SNMP GET completed. Result:{Result}",result);
        }

        public async Task ExecuteGetNextAsync(SnmpGetNextRequestedEvent snmpGetNextRequestedEvent,CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpGetNextRequestedEvent.DeviceId,snmpGetNextRequestedEvent.Oid, snmpGetNextRequestedEvent.TimeoutMilliseconds);

            var result = await _snmpService.GetNextAsync(request,cancellationToken);

            if (result != null)
            {
                await _redisService.SaveLatestValueAsync(new SnmpValue
                {
                    DeviceId = snmpGetNextRequestedEvent.DeviceId,
                    Oid = snmpGetNextRequestedEvent.Oid,
                    Value = result,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("SNMP GETNEXT completed. Result:{Result}", result);
        }

        public async Task ExecuteSetAsync(SnmpSetRequestedEvent snmpSetRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpSetRequestedEvent.DeviceId,snmpSetRequestedEvent.Oid, snmpSetRequestedEvent.TimeoutMilliseconds);

            request.Value = snmpSetRequestedEvent.Value;

            await _snmpService.SetAsync( request,cancellationToken);

            _logger.LogInformation("SNMP SET completed. DeviceId:{DeviceId}, OID:{Oid}",snmpSetRequestedEvent.DeviceId, snmpSetRequestedEvent.Oid);
        }

        public async Task ExecuteWalkAsync(SnmpWalkRequestedEvent snmpWalkRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(  snmpWalkRequestedEvent.DeviceId,snmpWalkRequestedEvent.RootOid, snmpWalkRequestedEvent.TimeoutMilliseconds);

            var result = await _snmpService.WalkAsync(request, cancellationToken);

            foreach (var variable in result)
            {
                await _redisService.SaveLatestValueAsync(new SnmpValue
                {
                    DeviceId = snmpWalkRequestedEvent.DeviceId,
                    Oid = variable.Id.ToString(),
                    Value = variable.Data.ToString(),
                    Timestamp = DateTime.UtcNow
                });
                _logger.LogInformation("OID: {Oid}, Value: {Value}", variable.Id, variable.Data);
            }
            

            _logger.LogInformation("SNMP WALK completed. Count:{Count}",result.Count);
        }

        private async Task<SnmpRequest> BuildRequestAsync(int deviceId, string oid, int timeoutMilliseconds)
        {
            var device = await _deviceDAL.GetByIdAsync(deviceId);

            if (device == null)
                throw new Exception($"Device not found. DeviceId:{deviceId}");

            var credentialResult = await _credentialService.GetByDeviceIdAsync(deviceId);

            if (!credentialResult.IsSuccess || credentialResult.Value == null)
                throw new Exception($"Credential not found. DeviceId:{deviceId}");

            var credential = credentialResult.Value;


            return new SnmpRequest
            {
                IpAddress = device.IpAddress,
                Port = device.Port,
                Oid = oid,
                TimeoutMilliseconds = timeoutMilliseconds,
                Credential = new SnmpCredentialDTO
                {
                    Id = credential.Id,
                    DeviceId = credential.DeviceId,
                    Version = credential.Version,
                    Community = credential.Community,
                    UserName = credential.UserName,
                    SecurityLevel = credential.SecurityLevel,
                    AuthProtocol = credential.AuthProtocol,
                    PrivacyProtocol = credential.PrivacyProtocol,
                    AuthPassword = credential.AuthPassword,
                    PrivacyPassword = credential.PrivacyPassword
                }
            };
        }
    }
}
