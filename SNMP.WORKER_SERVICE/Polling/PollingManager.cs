using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using Snmp.EventWorker.Services;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Polling
{
    public class PollingManager : IPollingManager
    {
        private readonly ILogger<PollingManager> _logger;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningPollings = new();
        private readonly ISnmpService _snmpService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public PollingManager(ILogger<PollingManager> logger, ISnmpService snmpService, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _snmpService = snmpService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public bool IsRunning(int deviceId)
        {
            return _runningPollings.ContainsKey(deviceId);
        }

        public Task StartAsync(DevicePollingStartedEvent eventMessage, CancellationToken cancellationToken)
        {
            if (_runningPollings.ContainsKey(eventMessage.DeviceId))
            {
                _logger.LogWarning("Polling already running. DeviceId: {DeviceId}", eventMessage.DeviceId);
                return Task.CompletedTask;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!_runningPollings.TryAdd(eventMessage.DeviceId, cts))
            {
                _logger.LogWarning("Polling already running. DeviceId: {DeviceId}", eventMessage.DeviceId);
                return Task.CompletedTask;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();

                        var credentialService = scope.ServiceProvider.GetRequiredService<ISnmpCredentialService>();
                        var deviceParameterService = scope.ServiceProvider.GetRequiredService<IDeviceParameterService>();

                        var credentialEntity = await credentialService.GetByDeviceIdAsync(eventMessage.DeviceId);
                        var deviceParameters = await deviceParameterService.GetByDeviceIdAsync(eventMessage.DeviceId);

                        if (credentialEntity == null)
                        {
                            _logger.LogWarning("Credentials not found for DeviceId: {DeviceId}. Polling skipped.", eventMessage.DeviceId);
                        }
                        else
                        {
                            var credentialDto = new SnmpCredentialDTO
                            {
                                Id = credentialEntity.Id,
                                DeviceId = credentialEntity.DeviceId,
                                UserName = credentialEntity.UserName,
                                SecurityLevel = credentialEntity.SecurityLevel,
                                AuthProtocol = credentialEntity.AuthProtocol,
                                PrivacyProtocol = credentialEntity.PrivacyProtocol,
                                AuthPassword = credentialEntity.AuthPassword,
                                PrivacyPassword = credentialEntity.PrivacyPassword,
                                Version = credentialEntity.Version,
                                Community = credentialEntity.Community
                            };

                            foreach (var parameter in deviceParameters)
                            {
                                var result = await _snmpService.GetAsync(
                                    eventMessage.IpAddress,
                                    eventMessage.Port,
                                    parameter.Oid,
                                    credentialDto,
                                    cts.Token);

                                _logger.LogInformation(
                                    "Parameter: {Parameter}, Value: {Value}",
                                    parameter.ParameterName,
                                    result);

                                _logger.LogInformation("SNMP Result for Device {DeviceId}: {Result}", eventMessage.DeviceId, result);
                            }

                            
                        }

                        await Task.Delay(
                            TimeSpan.FromSeconds(eventMessage.IntervalSeconds),
                            cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Polling cancelled. DeviceId: {DeviceId}", eventMessage.DeviceId);
                }
                finally
                {
                    _runningPollings.TryRemove(eventMessage.DeviceId, out _);
                }

            }, cts.Token);

            _logger.LogInformation("Polling started. DeviceId: {DeviceId}", eventMessage.DeviceId);

            return Task.CompletedTask;
        }

        public Task StopAsync(int deviceId)
        {
            if (_runningPollings.TryRemove(deviceId, out var cts))
            {
                cts.Cancel();
                cts.Dispose();

                _logger.LogInformation("Polling stopped. DeviceId: {DeviceId}", deviceId);
            }

            return Task.CompletedTask;
        }
    }
}
