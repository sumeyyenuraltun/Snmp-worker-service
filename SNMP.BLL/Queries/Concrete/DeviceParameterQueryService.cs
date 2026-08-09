using AutoMapper;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Concrete
{
    public class DeviceParameterQueryService : IDeviceParameterQueryService
    {
        private readonly IDeviceParameterDAL _deviceParameterDAL;
        private readonly IMapper _mapper;

        public DeviceParameterQueryService(IDeviceParameterDAL deviceParameterDAL, IMapper mapper)
        {
            _deviceParameterDAL = deviceParameterDAL;
            _mapper = mapper;
        }

        public async Task<Result<List<DeviceParameterDTO>>> GetByDeviceIdAsync(int deviceId)
        {
            var parameters =await _deviceParameterDAL.GetAllAsync(x => x.DeviceId == deviceId, x=> x.Parameter);

            return Result<List<DeviceParameterDTO>>.Success(_mapper.Map<List<DeviceParameterDTO>>(parameters));
        }
        public async Task<Result<DeviceParameterDTO>> GetByDeviceIdAndParameterIdAsync(int deviceId,int parameterId)
        {
            var entity = await _deviceParameterDAL
                .GetByDeviceIdAndParameterIdAsync(deviceId, parameterId);

            if (entity == null)
                return Result<DeviceParameterDTO>.Failure("Device parameter not found.");

            var dto = _mapper.Map<DeviceParameterDTO>(entity);

            return Result<DeviceParameterDTO>.Success(dto);
        }
    }
}
