using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface IBaseService<TEntity> where TEntity: BaseEntity
    {
        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<List<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(int id);

    }
}
