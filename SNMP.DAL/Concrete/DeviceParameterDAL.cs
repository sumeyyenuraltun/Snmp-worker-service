using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Snmp.DataAccess.Concrete
{
    public class DeviceParameterDAL : BaseRepository<DeviceParameter>, IDeviceParameterDAL
    {
        private readonly AppDbContext _context;
        public DeviceParameterDAL(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<DeviceParameter>> GetByDeviceIdAsync(int deviceId, CancellationToken cancellationToken)
        {
            return await _context.DeviceParameters
                .Include(x => x.Parameter)
                .Where(x => x.DeviceId == deviceId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DeviceParameter?> GetByDeviceIdAndParameterIdAsync(int deviceId,int parameterId, CancellationToken cancellationToken)
        {
            return await _context.DeviceParameters
                .Include(x => x.Parameter)
                .FirstOrDefaultAsync(x =>
                    x.DeviceId == deviceId &&
                    x.ParameterId == parameterId, cancellationToken);
        }
    }
}
