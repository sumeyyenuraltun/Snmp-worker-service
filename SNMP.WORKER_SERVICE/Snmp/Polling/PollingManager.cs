using Snmp.Business.Abstract.Redis;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.DTOs.SnmpValue;
using Snmp.EventWorker.Cache;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Services;
using SNMP.ENTITY.Events.Snmp;
using System.Collections.Concurrent;


namespace Snmp.EventWorker.Snmp.Polling
{
    public class PollingManager : IPollingManager
    {
        private readonly ILogger<PollingManager> _logger;
        private readonly ConcurrentDictionary<int, PollingSession> _sessions = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDeviceConfigurationCache _deviceConfigurationCache;

        public PollingManager(ILogger<PollingManager> logger, IServiceScopeFactory serviceScopeFactory, IDeviceConfigurationCache deviceConfigurationCache)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _deviceConfigurationCache = deviceConfigurationCache;
        }

        public bool IsRunning(int deviceId)
        {
            return _sessions.ContainsKey(deviceId);
        }

        public async Task StartAsync(DevicePollingStartedEvent eventMessage, CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var configuration = await _deviceConfigurationCache.GetAsync(eventMessage.DeviceId, cancellationToken);

            if(configuration is null)
            {
                _logger.LogWarning("Configuration not found.DeviceId: {DeviceId}", eventMessage.DeviceId);
                cts.Dispose();
                return;
            }
            
            var session = new PollingSession
            {
                CancellationTokenSource = cts,
                Configuration = configuration
            };

            if (!_sessions.TryAdd(eventMessage.DeviceId, session))
            {
                _logger.LogWarning("Polling already running. DeviceId: {DeviceId}",  eventMessage.DeviceId);

                cts.Dispose();
                return;
            }

            foreach (var parameter in configuration.Parameters.Where(x => x.IsEnabled))
            {
                var task = Task.Run(
                    () => RunParameterPollingAsync(
                        eventMessage.DeviceId,
                        parameter,
                        cts.Token),
                    cts.Token);

                session.RunningTasks.Add(task);
            }

            _logger.LogInformation("Polling started. DeviceId:{DeviceId}", eventMessage.DeviceId);

            return;
        }

        private async Task RunParameterPollingAsync(int deviceId,DeviceParameterDTO parameter,CancellationToken token)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var snmpService = scope.ServiceProvider.GetRequiredService<ISnmpService>();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var configuration = await _deviceConfigurationCache.GetAsync(deviceId, token);

                    if (configuration is null)
                        return;

                    var request = new SnmpRequest
                    {
                        IpAddress = configuration.IpAddress,
                        Port = configuration.Port,
                        Oid = parameter.Oid,
                        Credential = configuration.Credential,
                        DataType = parameter.DataType,
                    };
                   
                    var result = await snmpService.GetAsync(request, token);

                    if (result != null)
                    {
                        await redisService.SaveLatestValueAsync(new SnmpValue
                        {
                            DeviceId = deviceId,
                            ParameterId = parameter.ParameterId,
                            Oid = parameter.Oid,
                            Value = result,
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    _logger.LogInformation( "Parameter:{Parameter}, Value:{Value}", parameter.Oid, result);

                    await Task.Delay(TimeSpan.FromSeconds(parameter.PollingIntervalSeconds),token);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation( "Polling cancelled. DeviceId:{DeviceId}, OID:{Oid}",deviceId, parameter.Oid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Polling failed. DeviceId:{DeviceId}, OID:{Oid}", deviceId,parameter.Oid);
            }
        }
        public async Task StopAsync(int deviceId)
        {
            if (_sessions.TryRemove(deviceId, out var session))
            {
                session.CancellationTokenSource.Cancel();

                try
                {
                    await Task.WhenAll(session.RunningTasks);
                }
                catch (OperationCanceledException)
                {
                }

                session.CancellationTokenSource.Dispose();

                _logger.LogInformation(
                    "Polling stopped. DeviceId:{DeviceId}",
                    deviceId);
            }
        }
        public async Task RestartAsync(int deviceId, CancellationToken cancellationToken)
        {
            await _deviceConfigurationCache.RefreshAsync(deviceId, cancellationToken);

            if (!IsRunning(deviceId))
            {
                _logger.LogInformation( "Configuration refreshed. Polling is not running. DeviceId:{DeviceId}", deviceId);

                return;
            }

            await StopAsync(deviceId);

            await StartAsync(new DevicePollingStartedEvent(deviceId), cancellationToken);

            _logger.LogInformation( "Polling restarted. DeviceId:{DeviceId}",deviceId);
        }
        public async Task StopAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping all polling sessions...");

            var deviceIds = _sessions.Keys.ToList();

            foreach (var deviceId in deviceIds)
            {
                await Task.WhenAll(deviceIds.Select(StopAsync));
            }

            _logger.LogInformation("All polling sessions stopped.");
        }
    }
}
