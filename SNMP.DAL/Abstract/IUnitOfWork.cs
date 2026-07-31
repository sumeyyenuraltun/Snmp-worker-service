using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Abstract
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
