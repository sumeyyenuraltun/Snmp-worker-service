using Microsoft.EntityFrameworkCore;
using SNMP.DAL.Abstract;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SNMP.DAL.Concrete
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync( TEntity entity,CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>()
                .AddAsync(entity, cancellationToken);

        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Remove(entity);
           return Task.CompletedTask;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken,params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }


    }
}
