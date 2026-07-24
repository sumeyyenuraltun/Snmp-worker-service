using AutoMapper;
using Snmp.Business.DTOs.Devices;
using SNMP.BLL.Abstract;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.ENTITY.Abstract;
using SNMP.ENTITY.Concrete;
using SNMP.ENTITY.Events;
using SNMP.ENTITY.Events.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Concrete
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IMapper _mapper;
        public DeviceService(IDeviceDAL deviceDAL , IMapper mapper)
        {
            _deviceDAL = deviceDAL;
            _mapper = mapper;
        }

        public async Task AddAsync(AddDeviceDTO dto)
        {
            var entity = _mapper.Map<Device>(dto);

            await _deviceDAL.AddAsync(entity);
        }

        public async Task UpdateAsync(UpdateDeviceDTO dto)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Device couldn't find.");

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Device couldn't find.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(entity);
        }

        public async Task<List<DeviceDTO>> GetAllAsync()
        {
            var devices = await _deviceDAL.GetAllAsync(x => x.IsActive);

            return _mapper.Map<List<DeviceDTO>>(devices);
        }

        public async Task<DeviceDTO?> GetByIdAsync(int id)
        {
            var device = await _deviceDAL.GetAsync(x => x.Id == id && x.IsActive);

            if (device == null)
                return null;

            return _mapper.Map<DeviceDTO>(device);
        }
    }
}
