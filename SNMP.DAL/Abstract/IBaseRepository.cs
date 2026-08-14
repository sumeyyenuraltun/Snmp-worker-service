using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SNMP.DAL.Abstract
{
    public interface IBaseRepository<TEntity> where TEntity: BaseEntity
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity,CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken cancellationToken = default);
    }
}
