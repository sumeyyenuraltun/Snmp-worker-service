using AutoMapper;
using Snmp.Business.DTOs.Devices;
using Snmp.DataAccess.Abstract;
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
        private readonly IEventPublisher _eventPublisher;

        public DeviceService(IDeviceDAL deviceDAL, IMapper mapper, IEventPublisher eventPublisher)
        {
            _deviceDAL = deviceDAL;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        
        }

        public async Task AddAsync( AddDeviceDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<Device>(dto);

                await _deviceDAL.AddAsync(entity, cancellationToken);


                var deviceCreatedEvent = new DeviceCreatedEvent(
                    entity.IpAddress,
                    entity.DeviceName,
                    entity.Port,
                    entity.CreatedAt
                    
                );
                deviceCreatedEvent.AggregateId = entity.Id;


                await _eventPublisher.PublishAsync(
                    deviceCreatedEvent,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                var failedEvent = new DeviceCreationFailedEvent(
                    dto.IpAddress,
                    dto.DeviceName,
                    dto.Port,
                    ex.Message,
                    ex.InnerException?.Message
                );


                await _eventPublisher.PublishAsync(
                    failedEvent,
                    cancellationToken);

                throw;
            }
        }

        public async Task UpdateAsync(UpdateDeviceDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Device couldn't find.");

            var oldIpAddress = entity.IpAddress;
            var oldPort = entity.Port;

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(entity);

            if(oldIpAddress != entity.IpAddress || oldPort != entity.Port)
            {
                await _eventPublisher.PublishAsync(new DeviceUpdatedEvent(
                    entity.Id,
                    entity.IpAddress,
                    entity.Port
                    ), cancellationToken);
            }

        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Device couldn't find.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.DeleteAsync(entity);

            await _eventPublisher.PublishAsync(new DeviceDeletedEvent(
                entity.Id,
                entity.IpAddress,
                entity.DeviceName
                ),cancellationToken);
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
