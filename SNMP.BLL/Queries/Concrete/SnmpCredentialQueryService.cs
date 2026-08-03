using AutoMapper;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Queries.Concrete
{
    public class SnmpCredentialQueryService : ISnmpCredentialQueryService
    {
        private readonly ISnmpCredentialDAL _snmpCredentialDAL;
        private readonly IMapper _mapper;

        public SnmpCredentialQueryService(ISnmpCredentialDAL snmpCredentialDAL, IMapper mapper)
        {
            _snmpCredentialDAL = snmpCredentialDAL;
            _mapper = mapper;
        }

        public async Task<Result<SnmpCredentialDTO>> GetByDeviceIdAsync(int deviceId)
        {
            var credential = await _snmpCredentialDAL.GetAsync(x =>x.DeviceId == deviceId);

            if(credential == null)
            {
                return Result<SnmpCredentialDTO>.Failure("SNMP credential not found for the specified device ID.");
            }

            return Result<SnmpCredentialDTO>.Success(_mapper.Map<SnmpCredentialDTO>(credential));
        }
    }
}
