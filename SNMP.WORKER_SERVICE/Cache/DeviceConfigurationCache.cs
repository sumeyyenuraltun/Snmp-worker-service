using Snmp.Business.Concrete;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Results;
using Snmp.EventWorker.Cache.Models;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Cache
{
    public class DeviceConfigurationCache : IDeviceConfigurationCache
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeviceConfigurationCache> _logger;

        private readonly ConcurrentDictionary<int, CachedDeviceConfiguration> _cache = new();
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

        public DeviceConfigurationCache(IServiceScopeFactory scopeFactory, ILogger<DeviceConfigurationCache> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<DeviceConfiguration?> GetAsync(int deviceId, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(deviceId, out var cached))
            {
                if (!IsExpired(cached))
                {
                    _logger.LogDebug("Cache hit. DeviceId: {DeviceId}", deviceId);
                    return cached.Configuration;
                }

                _logger.LogInformation("Cache expired. Refreshing configuration. DeviceId: {DeviceId}", deviceId);

                await RefreshAsync(deviceId, cancellationToken);

                return _cache.TryGetValue(deviceId, out cached)? cached.Configuration : null;
            }

            _logger.LogInformation("Cache miss. DeviceId: {DeviceId}", deviceId);

            await RefreshAsync(deviceId, cancellationToken);
            return _cache.TryGetValue(deviceId, out cached) ? cached.Configuration : null;
        }

        public async Task RefreshAsync(int deviceId, CancellationToken cancellationToken)
        {
            var configuration = await LoadFromDatabaseAsync(deviceId, cancellationToken);

            if (configuration == null)
            {
                _cache.TryRemove(deviceId, out _);
                return;
            }

            _cache[deviceId] = new CachedDeviceConfiguration
            {
                Configuration = configuration,
                CachedAt = DateTime.UtcNow
            };

        }

        private async Task<DeviceConfiguration?> LoadFromDatabaseAsync(int deviceId, CancellationToken cancellationToken) 
        {
            using var scope = _scopeFactory.CreateScope();

            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceQueryService>();
            var deviceParameterService = scope.ServiceProvider.GetRequiredService<IDeviceParameterQueryService>();
            var credentialService = scope.ServiceProvider.GetRequiredService<ISnmpCredentialQueryService>();

            var deviceResult = await deviceService.GetByIdAsync(deviceId);

            var credentialResult =await credentialService.GetByDeviceIdAsync(deviceId);

            var deviceParameterResult = await deviceParameterService.GetByDeviceIdAsync(deviceId);


            if (!deviceResult.IsSuccess || deviceResult.Value is null)
            {
                _logger.LogWarning("Device not found. DeviceId: {DeviceId}", deviceId);
                return null;
            }

            if (!credentialResult.IsSuccess || credentialResult.Value is null)
            {
                _logger.LogWarning("Credential not found. DeviceId: {DeviceId}", deviceId);
                return null;
            }

            if (!deviceParameterResult.IsSuccess || deviceParameterResult.Value is null)
            {
                _logger.LogWarning("Parameters not found. DeviceId: {DeviceId}", deviceId);
                return null;
            }

            var device = deviceResult.Value;
            var credential = credentialResult.Value;
            var parameters = deviceParameterResult.Value;

            return new DeviceConfiguration
            {
                DeviceId = device.Id,
                IpAddress = device.IpAddress,
                Port = device.Port,
                Credential = credential,
                Parameters = parameters
            };
        }

        public void Remove(int deviceId)
        {
            _cache.TryRemove(deviceId, out _);
        }

        private bool IsExpired(CachedDeviceConfiguration cached)
        {
            return DateTime.UtcNow - cached.CachedAt > _ttl;
        }
    }
}
