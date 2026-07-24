using AutoMapper;
using Snmp.Business.DTOs.SnmpLogs;
using SNMP.BLL.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Abstract;
using SNMP.ENTITY.Concrete;
using SNMP.ENTITY.Events;
using SNMP.ENTITY.Events.Device;
using SNMP.ENTITY.Events.SnmpLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.BLL.Concrete
{
    public class SnmpLogService : ISnmpLogService
    {
        private readonly ISnmpLogDAL _snmpLogDAL;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public SnmpLogService(ISnmpLogDAL snmpLogDAL, IMapper mapper, IEventPublisher eventPublisher)
        {
            _snmpLogDAL = snmpLogDAL;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task AddAsync(AddSnmpLogDTO addSnmpLogDTO, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new SnmpLogCreationRequested(
                addSnmpLogDTO.DeviceId,
                addSnmpLogDTO.Oid,
                addSnmpLogDTO.Value,
                addSnmpLogDTO.Type
            ), cancellationToken);

            try
            {
                var snmpLogEntity = _mapper.Map<SnmpLog>(addSnmpLogDTO);

                await _snmpLogDAL.AddAsync(snmpLogEntity);

                await _eventPublisher.PublishAsync(new SnmpLogCreated(
                    snmpLogEntity.DeviceId,
                    snmpLogEntity.Value,
                    snmpLogEntity.Type,
                    snmpLogEntity.Oid,
                    snmpLogEntity.CreatedAt
                    ), cancellationToken);
            }
            catch (Exception ex)
            { 
                await _eventPublisher.PublishAsync(new SnmpLogCreationFailed(
                    addSnmpLogDTO.DeviceId,
                    addSnmpLogDTO.Value,
                    addSnmpLogDTO.Oid,
                    addSnmpLogDTO.Type,
                    ex.Message,
                    ex.StackTrace,
                    0
                    ), cancellationToken);
                throw;
            }
        }

        public async Task<List<SnmpLogDTO>> GetLastLogsAsync(int count = 100)
        { 
            var logs = await _snmpLogDAL.GetLastAsync(count);
            return _mapper.Map<List<SnmpLogDTO>>(logs);
        }

        
        
       
        
    }
}
