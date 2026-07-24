using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Concrete;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.DataAccess.Concrete
{
    public class RedisBaseRepository<TEntity> : IRedisBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDatabase _redisDb;
        private readonly string _redisKey;

        public RedisBaseRepository(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
            _redisKey = $"{typeof(TEntity).Name.ToLower()}:list";
        }

        public async Task AddAsync(TEntity entity)
        {
            string json = JsonSerializer.Serialize(entity);
            await _redisDb.ListLeftPushAsync(_redisKey, json);
        }

        public async Task<List<TEntity>> GetLastAsync(int count = 100)
        {
            var redisValues = await _redisDb.ListRangeAsync(_redisKey, 0, count - 1);
            if (redisValues.Length == 0) return new List<TEntity>();

            return redisValues.Select(val => JsonSerializer.Deserialize<TEntity>(val.ToString())).ToList();
        }
    }
}
