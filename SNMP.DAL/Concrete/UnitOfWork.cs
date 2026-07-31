using Snmp.DataAccess.Abstract;
using SNMP.DAL.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
