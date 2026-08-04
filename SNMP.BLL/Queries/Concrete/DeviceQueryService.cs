using AutoMapper;
using Snmp.Business.DTOs.Devices;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Results;
using SNMP.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Concrete
{
    public class DeviceQueryService : IDeviceQueryService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IMapper _mapper;

        public DeviceQueryService(IDeviceDAL deviceDAL, IMapper mapper)
        {
            _deviceDAL = deviceDAL;
            _mapper = mapper;
        }

        public async Task<Result<List<DeviceDTO>>> GetAllAsync()
        {
            var devices = await _deviceDAL.GetAllAsync(x => x.IsActive);

            return Result<List<DeviceDTO>>.Success(_mapper.Map<List<DeviceDTO>>(devices));
        }

        public async Task<Result<DeviceDTO>> GetByIdAsync(int id)
        {
            var device = await _deviceDAL.GetAsync(x => x.Id == id && x.IsActive);

            if(device == null) 
            {
                return Result<DeviceDTO>.Failure("Device not found");
            }

            return Result<DeviceDTO>.Success(_mapper.Map<DeviceDTO>(device));
        }
    }
}
