using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete
{
    public class DeviceParameterService : IDeviceParameterService
    {
        private readonly IDeviceParameterDAL _deviceParameterDAL;
        private readonly IMapper _mapper;

        public DeviceParameterService(
            IDeviceParameterDAL deviceParameterDAL,
            IMapper mapper)
        {
            _deviceParameterDAL = deviceParameterDAL;
            _mapper = mapper;
        }

        public async Task<List<DeviceParameterDTO>> GetByDeviceIdAsync(int deviceId)
        {
            var entities = await _deviceParameterDAL.GetByDeviceIdAsync(deviceId);

            return _mapper.Map<List<DeviceParameterDTO>>(entities);
        }

        public async Task<DeviceParameterDTO> AddAsync(AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<DeviceParameter>(addDeviceParameterDTO);

            await _deviceParameterDAL.AddAsync(entity,cancellationToken);

            return _mapper.Map<DeviceParameterDTO>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _deviceParameterDAL.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Device parameter not found.");

            await _deviceParameterDAL.DeleteAsync(entity);
        }
        public async Task<DeviceParameterDTO> UpdateAsync(UpdateDeviceParameterDTO updateDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var entity = await _deviceParameterDAL.GetByIdAsync(updateDeviceParameterDTO.Id);

            if (entity == null)
                throw new Exception("DeviceParameter not found.");

            entity.DeviceId = updateDeviceParameterDTO.DeviceId;
            entity.ParameterId = updateDeviceParameterDTO.ParameterId;

            await _deviceParameterDAL.UpdateAsync(entity);

            return _mapper.Map<DeviceParameterDTO>(entity);
        }
        public async Task<DeviceParameterDTO?> GetByIdAsync(int id)
        {
            var entity = await _deviceParameterDAL.GetAsync(
                x => x.Id == id,
                x => x.Parameter);

            if (entity == null)
                return null;

            return _mapper.Map<DeviceParameterDTO>(entity);
        }

      
    }
}
