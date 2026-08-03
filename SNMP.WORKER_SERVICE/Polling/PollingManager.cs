using AutoMapper.Execution;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.DTOs.SnmpValue;
using Snmp.Business.Queries.Abstract;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.Entity.Concrete;
using Snmp.EventWorker.Redis.Repositories;
using Snmp.EventWorker.Redis.Services;
using Snmp.EventWorker.Snmp.Models;
using Snmp.EventWorker.Snmp.Services;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
    
        public PollingManager(ILogger<PollingManager> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
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

                        var credentialQueryService = scope.ServiceProvider.GetRequiredService<ISnmpCredentialQueryService>();
                        var deviceParameterQueryService = scope.ServiceProvider.GetRequiredService<IDeviceParameterQueryService>();
                        var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                        var snmpService = scope.ServiceProvider.GetRequiredService<ISnmpService>();

                        var credentialResult = await credentialQueryService.GetByDeviceIdAsync(eventMessage.DeviceId);
                        var parameterResult = await deviceParameterQueryService.GetByDeviceIdAsync(eventMessage.DeviceId);

                        if (!parameterResult.IsSuccess || parameterResult.Value == null)
                        {
                            _logger.LogWarning("No parameters found. DeviceId:{DeviceId}", eventMessage.DeviceId);
                            continue;
                        }

                        var deviceParameters = parameterResult.Value;

                        if (!credentialResult.IsSuccess || credentialResult.Value == null)
                        {
                            _logger.LogWarning("Credentials not found for DeviceId: {DeviceId}. Polling skipped.", eventMessage.DeviceId);
                        }
                        else
                        {
                            var credentialDto = credentialResult.Value!;

                            foreach (var parameter in deviceParameters)
                            {
                                try
                                {
                                    var request = new SnmpRequest
                                    {
                                        IpAddress = eventMessage.IpAddress,
                                        Port = eventMessage.Port,
                                        Oid = parameter.Oid,
                                        Credential = credentialDto
                                    };
                                    var result = await snmpService.GetAsync(request, cts.Token);

                                    if (result != null)
                                    {
                                        await redisService.SaveLatestValueAsync(new SnmpValue
                                        {
                                            DeviceId = eventMessage.DeviceId,
                                            ParameterId = parameter.ParameterId,
                                            Oid = parameter.Oid,
                                            Value = result,
                                            Timestamp = DateTime.UtcNow
                                        });
                                    }
                                    _logger.LogInformation(
                                    "Parameter: {Parameter}, Value: {Value}",parameter.ParameterName,result);

                                    _logger.LogInformation("SNMP Result for Device {DeviceId}: {Result}", eventMessage.DeviceId, result);
                                }
                                catch (Exception ex) 
                                {
                                    _logger.LogError(ex, "SNMP query failed. DeviceId:{DeviceId}, OID:{Oid}", eventMessage.DeviceId, parameter.Oid);
                                }
                                
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
