using Snmp.Business.DTOs.Devices;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Abstract
{
    public interface IDeviceService 
    {
        public Task Add(AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken);
        public void Update(UpdateDeviceDTO updateDeviceDTO);
        public void Delete(int id);
        public List<DeviceDTO> GetAll();
        public DeviceDTO GetById(int id);
    }
}
