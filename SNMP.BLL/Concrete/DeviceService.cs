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
        private readonly IEventPublisher _eventPublisher;
        public DeviceService(IDeviceDAL deviceDAL , IMapper mapper, IEventPublisher eventPublisher)
        {
            _deviceDAL = deviceDAL;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task Add(AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken = default)
        {
            await _eventPublisher.PublishAsync(new DeviceCreationRequested(
                addDeviceDTO.IpAddress,
                addDeviceDTO.DeviceName,
                addDeviceDTO.Port
            ), cancellationToken);

            try
            {
                var deviceEntity = _mapper.Map<Device>(addDeviceDTO);

                _deviceDAL.Add(deviceEntity);

                await _eventPublisher.PublishAsync(new DeviceCreated(
                    deviceEntity.IpAddress,
                    deviceEntity.DeviceName,
                    deviceEntity.Port,
                    deviceEntity.CreatedAt
                ), cancellationToken);
            }
            catch (Exception ex)
            {
                await _eventPublisher.PublishAsync(new DeviceCreationFailed(
                    addDeviceDTO.IpAddress,
                    addDeviceDTO.DeviceName,
                    addDeviceDTO.Port,
                    ex.Message,
                    ex.StackTrace,
                    0
                ), cancellationToken);

                throw;
            }
        }

        public void Update(UpdateDeviceDTO deviceUpdateDTO)
        {
            var existingDevice = _deviceDAL.Get(d => d.Id == deviceUpdateDTO.Id);

            if (existingDevice == null)
                throw new Exception("Güncellenecek cihaz bulunamadı!");

            _mapper.Map(deviceUpdateDTO, existingDevice);
            existingDevice.UpdatedAt = DateTime.UtcNow;

            _deviceDAL.Update(existingDevice);
        }

        public void Delete(int id)
        {
            var existingDevice = _deviceDAL.Get(d => d.Id == id);

            if (existingDevice == null)
                throw new Exception("Silinecek cihaz bulunamadı!");

            existingDevice.IsActive = false;
            existingDevice.UpdatedAt = DateTime.UtcNow;

            _deviceDAL.Update(existingDevice);
        }

        public List<DeviceDTO> GetAll()
        {
            var devices = _deviceDAL.GetAll(d => d.IsActive == true);
            return _mapper.Map<List<DeviceDTO>>(devices);
        }

        public DeviceDTO GetById(int id)
        {
            var device = _deviceDAL.Get(d => d.Id == id && d.IsActive == true);

            if (device == null)
                return null;

            return _mapper.Map<DeviceDTO>(device);
        }
    }
}
