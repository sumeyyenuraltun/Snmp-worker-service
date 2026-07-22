using Snmp.Business.DTOs.SnmpLogs;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface ISnmpLogService
    {
        public Task Add(AddSnmpLogDTO addSnmpLogDTO, CancellationToken cancellationToken);
        public void Update(UpdateSnmpLogDTO updateSnmpLogDTO);
        public void Delete(int id);
        public List<SnmpLogDTO> GetAll();
        public SnmpLogDTO GetById(int id);
    }
}
