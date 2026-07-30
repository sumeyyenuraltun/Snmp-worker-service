using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Redis.Repositories
{
    public interface IRedisRepository
    {
        Task SetAsync (string key, object value);
        Task<T?> GetAsync<T> (string key);
        Task DeleteAsync (string key);

    }
}
