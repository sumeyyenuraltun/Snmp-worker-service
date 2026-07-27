using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Snmp;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Abstract;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete
{
    public class SnmpRequestedService : ISnmpRequestedService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IEventPublisher _eventPublisher;

        public SnmpRequestedService(IDeviceDAL deviceDAL, IEventPublisher eventPublisher)
        {
            _deviceDAL = deviceDAL;
            _eventPublisher = eventPublisher;
        }

        public async Task SendGetRequestedAsync(SnmpRequestDTO snmpRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpRequestDTO.DeviceId);
            if(device == null)
            {
                throw new Exception("Device couldn't find");
            }

            await _eventPublisher.PublishAsync(new SnmpQueryRequestedEvent(snmpRequestDTO.DeviceId,  snmpRequestDTO.Oid), cancellationToken);
        }

        public async Task SendWalkRequestedAsync(SnmpWalkRequestDTO snmpWalkRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpWalkRequestDTO.DeviceId);
            if(device == null)
            {
                throw new Exception("Device couldn't find");
            }
            await _eventPublisher.PublishAsync(new SnmpWalkRequestedEvent(snmpWalkRequestDTO.DeviceId, snmpWalkRequestDTO.RootOid),cancellationToken);
        }
    }
}
