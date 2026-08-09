using Snmp.DataAccess.Abstract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.DataAccess.Concrete.Redis
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDb;
        public RedisRepository(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _redisDb.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString());


        }

        public async Task SetAsync(string key, object value)
        {
            var json = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json, TimeSpan.FromHours(1));
        }

        public async Task DeleteAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }

        
    }
}
