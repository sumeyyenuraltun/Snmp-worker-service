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

        public async Task Add(AddSnmpLogDTO addSnmpLogDTO, CancellationToken cancellationToken)
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

                _snmpLogDAL.Add(snmpLogEntity);

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

        public void Delete(int id)
        {
            var existingLog = _snmpLogDAL.Get(s => s.Id == id);

            if (existingLog == null)
                throw new Exception("Silinecek log bulunamadı!");

            existingLog.IsActive = false;
            existingLog.UpdatedAt = DateTime.UtcNow;

            _snmpLogDAL.Update(existingLog);
        }

        public List<SnmpLogDTO> GetAll()
        {
            var logs = _snmpLogDAL.GetAll(s => s.IsActive == true);
            return _mapper.Map<List<SnmpLogDTO>>(logs);
        }

        public SnmpLogDTO GetById(int id)
        {
            var log = _snmpLogDAL.Get(s => s.Id == id && s.IsActive == true);

            if (log == null)
                return null;

            return _mapper.Map<SnmpLogDTO>(log);
        }

        public void Update(UpdateSnmpLogDTO updateSnmpLogDTO)
        {
            var existingLog = _snmpLogDAL.Get(s => s.Id == updateSnmpLogDTO.Id);

            if (existingLog == null)
                throw new Exception("Güncellenecek log bulunamadı!");

            _mapper.Map(updateSnmpLogDTO, existingLog);
            existingLog.UpdatedAt = DateTime.UtcNow;

            _snmpLogDAL.Update(existingLog);
        }
    }
}
