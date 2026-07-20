using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SNMP.DAL.Abstract
{
    public interface IBaseRepository<TEntity> where TEntity: BaseEntity
    {
        public void Add(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
        public List<TEntity> GetAll();
        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter);
        public TEntity Get(Expression<Func<TEntity, bool>> filter);
        public TEntity GetById(int id);
    }
}
