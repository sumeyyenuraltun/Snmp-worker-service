using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Abstract
{
    public interface IRedisBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);
        Task<List<TEntity>> GetLastAsync(int count = 100);
    }
}
