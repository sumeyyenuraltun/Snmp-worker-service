using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface IBaseService<TEntity> where TEntity: BaseEntity
    {
        public void Add(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
        public List<TEntity> GetAll();
        public TEntity GetById(int id);

    }
}
