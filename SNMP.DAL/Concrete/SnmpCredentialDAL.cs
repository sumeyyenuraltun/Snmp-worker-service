using Microsoft.EntityFrameworkCore;
using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.DataAccess.Concrete
{
    public class SnmpCredentialDAL : BaseRepository<SnmpCredential>, ISnmpCredentialDAL
    {
        private readonly AppDbContext _context;

        public SnmpCredentialDAL(AppDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<SnmpCredential?> GetByDeviceIdAsync(int deviceId)
        {
            return await _context.SnmpCredentials
                .FirstOrDefaultAsync(x => x.DeviceId == deviceId && x.IsActive);
        }
    }
}
