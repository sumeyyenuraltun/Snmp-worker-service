using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SNMP.DAL.Abstract
{
    public interface IBaseRepository<TEntity> where TEntity: BaseEntity
    {
        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<List<TEntity>> GetAllAsync();

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity?> GetByIdAsync(int id);
    }
}
