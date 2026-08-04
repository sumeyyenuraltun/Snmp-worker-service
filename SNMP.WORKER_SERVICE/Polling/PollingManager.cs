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
        private readonly ConcurrentDictionary<int, PollingSession> _sessions = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
    
        public PollingManager(ILogger<PollingManager> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            
        }

        public bool IsRunning(int deviceId)
        {
            return _sessions.ContainsKey(deviceId);
        }

        public Task StartAsync(DevicePollingStartedEvent eventMessage, CancellationToken cancellationToken)
        {
            if (_sessions.ContainsKey(eventMessage.DeviceId))
            {
                _logger.LogWarning("Polling already running. DeviceId: {DeviceId}", eventMessage.DeviceId);
                return Task.CompletedTask;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using var scope = _serviceScopeFactory.CreateScope();

            var credentialQueryService = scope.ServiceProvider.GetRequiredService<ISnmpCredentialQueryService>();
            var deviceParameterQueryService = scope.ServiceProvider.GetRequiredService<IDeviceParameterQueryService>();

            var credentialResult = credentialQueryService.GetByDeviceIdAsync(eventMessage.DeviceId).Result;
            var parameterResult = deviceParameterQueryService.GetByDeviceIdAsync(eventMessage.DeviceId).Result;

            if (!credentialResult.IsSuccess || credentialResult.Value == null)
            {
                _logger.LogWarning("Credentials not found. DeviceId:{DeviceId}", eventMessage.DeviceId);
                return Task.CompletedTask;
            }

            if (!parameterResult.IsSuccess || parameterResult.Value == null)
            {
                _logger.LogWarning("No parameters found. DeviceId:{DeviceId}", eventMessage.DeviceId);
                return Task.CompletedTask;
            }

            var session = new PollingSession
            {
                CancellationTokenSource = cts,
                Context = new PollingContext
                {
                    Credential = credentialResult.Value,
                    Parameters = parameterResult.Value,
                    IpAddress = eventMessage.IpAddress,
                    Port = eventMessage.Port,
                    IntervalSeconds = eventMessage.IntervalSeconds
                }
            };

            if (!_sessions.TryAdd(eventMessage.DeviceId, session))
            {
                cts.Dispose();
                return Task.CompletedTask;
            }

            _ = Task.Run(() => RunPollingAsync(eventMessage.DeviceId), cts.Token);

            _logger.LogInformation("Polling started. DeviceId:{DeviceId}", eventMessage.DeviceId);

            return Task.CompletedTask;
        }

        private async Task RunPollingAsync(int deviceId)
        {
            var session = _sessions[deviceId];
            var context = session.Context;
            var token = session.CancellationTokenSource.Token;

            using var scope = _serviceScopeFactory.CreateScope();

            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var snmpService = scope.ServiceProvider.GetRequiredService<ISnmpService>();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var parameter in context.Parameters)
                    {
                        try
                        {
                            var request = new SnmpRequest
                            {
                                IpAddress = context.IpAddress,
                                Port = context.Port,
                                Oid = parameter.Oid,
                                Credential = context.Credential
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

                            _logger.LogInformation(
                                "Parameter:{Parameter}, Value:{Value}",
                                parameter.ParameterName,
                                result);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "SNMP query failed. DeviceId:{DeviceId}, OID:{Oid}",
                                deviceId,
                                parameter.Oid);
                        }
                    }

                    await Task.Delay(
                        TimeSpan.FromSeconds(context.IntervalSeconds),
                        token);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Polling cancelled. DeviceId:{DeviceId}", deviceId);
            }
            finally
            {
                _sessions.TryRemove(deviceId, out _);
            }
        }
        public Task StopAsync(int deviceId)
        {
            if (_sessions.TryRemove(deviceId, out var session))
            {
                session.CancellationTokenSource.Cancel();
                session.CancellationTokenSource.Dispose();

                _logger.LogInformation("Polling stopped. DeviceId:{DeviceId}", deviceId);
            }

            return Task.CompletedTask;
        }
    }
}
