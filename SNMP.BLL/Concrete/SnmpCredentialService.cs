using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete
{
    public class SnmpCredentialService : ISnmpCredentialService
    {
        private readonly ISnmpCredentialDAL _snmpCredentialDAL;
        private readonly IMapper _mapper;

        public SnmpCredentialService(ISnmpCredentialDAL snmpCredentialDAL, IMapper mapper)
        {
            _snmpCredentialDAL = snmpCredentialDAL;
            _mapper = mapper;
        }

        public async Task AddAsync(AddSnmpCredentialDTO dto)
        {
            var exists = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive);

            if (exists != null)
                throw new Exception("SNMP credentials already exist for this device.");

            var entity = _mapper.Map<SnmpCredential>(dto);

            await _snmpCredentialDAL.AddAsync(entity);
        }

        public async Task UpdateAsync(UpdateSnmpCredentialDTO dto)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive);

            if (entity == null)
                throw new Exception("SNMP credentials not found.");

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _snmpCredentialDAL.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("SNMP credentials not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity);
        }

        public async Task<SnmpCredentialDTO?> GetByDeviceIdAsync(int deviceId)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == deviceId &&
                x.IsActive);

            if (entity == null)
                return null;

            return _mapper.Map<SnmpCredentialDTO>(entity);
        }
    }
}
