
using Snmp.Business.DTOs.SnmpValue;
using Snmp.EventWorker.Redis.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Redis.Services
{
    public class RedisService : IRedisService
    {
        private readonly IRedisRepository _redisRepository;

        public RedisService(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task<SnmpValue?> GetLatestValueAsync(int deviceId, int parameterId)
        {
            var key = RedisKey.LatestValue(deviceId, parameterId);
            return await _redisRepository.GetAsync<SnmpValue>(key);
        }

        public async Task SaveLatestValueAsync(SnmpValue value)
        {
            var key = RedisKey.LatestValue(value.DeviceId, value.ParameterId);

            await _redisRepository.SetAsync(key, value);
        }
    }
}
