using Snmp.Business.Abstract.Redis;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.DTOs.SnmpValue;
using Snmp.Business.Queries.Abstract;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Services;
using SNMP.ENTITY.Events.Snmp;


namespace Snmp.EventWorker.Snmp.Manager
{
    public class SnmpRequestManager : ISnmpRequestManager
    {
        private readonly ILogger<SnmpRequestManager> _logger;
        private readonly IDeviceQueryService _deviceQueryService;
        private readonly ISnmpCredentialQueryService _credentialQueryService;
        private readonly ISnmpService _snmpService;
        private readonly IRedisService _redisService;
        private readonly IDeviceParameterQueryService _deviceParameterQueryService;

        public SnmpRequestManager(ILogger<SnmpRequestManager> logger, IDeviceQueryService deviceQueryService, ISnmpCredentialQueryService credentialQueryService, ISnmpService snmpService, IRedisService redisService, IDeviceParameterQueryService deviceParameterQueryService)
        {
            _logger = logger;
            _deviceQueryService = deviceQueryService;
            _credentialQueryService = credentialQueryService;
            _snmpService = snmpService;
            _redisService = redisService;
            _deviceParameterQueryService = deviceParameterQueryService;
        }

        public async Task ExecuteGetAsync(SnmpGetRequestedEvent snmpGetRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpGetRequestedEvent.DeviceId, snmpGetRequestedEvent.ParameterId,snmpGetRequestedEvent.TimeoutMilliseconds);

            var result = await _snmpService.GetAsync(request,cancellationToken);

            if (result != null)
            {
                await _redisService.SaveLatestValueAsync(new SnmpValue
                {
                    DeviceId = snmpGetRequestedEvent.DeviceId,
                    ParameterId = snmpGetRequestedEvent.ParameterId,
                    Oid = request.Oid,
                    Value = result,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("SNMP GET completed. DeviceId:{DeviceId}, ParameterId:{ParameterId}, Result:{Result}",snmpGetRequestedEvent.DeviceId,snmpGetRequestedEvent.ParameterId, result);
        }

        public async Task ExecuteGetNextAsync(SnmpGetNextRequestedEvent snmpGetNextRequestedEvent,CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpGetNextRequestedEvent.DeviceId, snmpGetNextRequestedEvent.ParameterId,snmpGetNextRequestedEvent.TimeoutMilliseconds);

            var result = await _snmpService.GetNextAsync(request,cancellationToken);

            if (result != null)
            {
                await _redisService.SaveLatestValueAsync(new SnmpValue
                {
                    DeviceId = snmpGetNextRequestedEvent.DeviceId,
                    ParameterId = snmpGetNextRequestedEvent.ParameterId,
                    Oid = request.Oid,
                    Value = result,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("SNMP GETNEXT completed. DeviceId:{DeviceId}, ParameterId:{ParameterId}, Result:{Result}",snmpGetNextRequestedEvent.DeviceId, snmpGetNextRequestedEvent.ParameterId,result);
        }

        public async Task ExecuteSetAsync(SnmpSetRequestedEvent snmpSetRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(snmpSetRequestedEvent.DeviceId,snmpSetRequestedEvent.ParameterId, snmpSetRequestedEvent.TimeoutMilliseconds);

            request.Value = snmpSetRequestedEvent.Value;

            await _snmpService.SetAsync( request,cancellationToken);

            _logger.LogInformation("SNMP SET completed. DeviceId:{DeviceId}, OID:{Oid}",snmpSetRequestedEvent.DeviceId, snmpSetRequestedEvent.ParameterId);
        }

        public async Task ExecuteWalkAsync(SnmpWalkRequestedEvent snmpWalkRequestedEvent, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestAsync(  snmpWalkRequestedEvent.DeviceId,snmpWalkRequestedEvent.RootParameterId, snmpWalkRequestedEvent.TimeoutMilliseconds);

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

        private async Task<SnmpRequest> BuildRequestAsync(int deviceId, int parameterId, int timeoutMilliseconds)
        {
            var deviceResult = await _deviceQueryService.GetByIdAsync(deviceId);

            if (!deviceResult.IsSuccess || deviceResult.Value == null)
                throw new Exception($"Device not found. DeviceId:{deviceId}");

            var device = deviceResult.Value;

            var credentialResult = await _credentialQueryService.GetByDeviceIdAsync(deviceId);

            if (!credentialResult.IsSuccess || credentialResult.Value == null)
                throw new Exception($"Credential not found. DeviceId:{deviceId}");

            var credential = credentialResult.Value;

            var parameterResult =await _deviceParameterQueryService.GetByDeviceIdAndParameterIdAsync(deviceId, parameterId);

            if (!parameterResult.IsSuccess || parameterResult.Value == null)
                throw new Exception($"DeviceParameter not found. DeviceId:{deviceId}, ParameterId:{parameterId}");

            var parameter = parameterResult.Value;

            return new SnmpRequest
            {
                IpAddress = device.IpAddress,
                Port = device.Port,
                Oid = parameter.Oid,
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
