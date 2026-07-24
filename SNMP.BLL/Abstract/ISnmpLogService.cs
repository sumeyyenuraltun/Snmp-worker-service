using Snmp.Business.DTOs.SnmpLogs;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface ISnmpLogService
    {
        Task AddAsync(AddSnmpLogDTO addSnmpLogDTO, CancellationToken cancellationToken = default);
        Task<List<SnmpLogDTO>> GetLastLogsAsync(int count = 100);
    }
}
