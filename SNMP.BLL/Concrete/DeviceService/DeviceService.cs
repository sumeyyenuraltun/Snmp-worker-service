using AutoMapper;
using Snmp.Business.Abstract.Outbox;
using Snmp.Business.DTOs.Devices;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Events.Device;
using SNMP.ENTITY.Concrete;
using Snmp.Business.Abstract.DeviceService;
using Microsoft.IdentityModel.Tokens;

namespace Snmp.Business.Concrete.DeviceService
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IMapper _mapper;
        private readonly IOutboxService _outboxService;
        private readonly IUnitOfWork _unitOfWork;

        public DeviceService(IDeviceDAL deviceDAL, IMapper mapper, IOutboxService outboxService, IUnitOfWork unitOfWork)
        {
            _deviceDAL = deviceDAL;
            _mapper = mapper;
            _outboxService = outboxService;
            _unitOfWork = unitOfWork;
        
        }

        public async Task<Result> AddAsync( AddDeviceDTO dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Device>(dto);

            await _deviceDAL.AddAsync(entity, cancellationToken);

            var deviceCreatedEvent = new DeviceCreatedEvent(
                entity.IpAddress,
                entity.DeviceName,
                entity.Port,
                entity.CreatedAt);
            await _outboxService.AddMessageAsync(deviceCreatedEvent, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateDeviceDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == dto.Id, cancellationToken);

            if (entity == null)
                return Result.Failure("Device couldn't find.");

            var oldIpAddress = entity.IpAddress;
            var oldPort = entity.Port;

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(entity, cancellationToken);

            if(oldIpAddress != entity.IpAddress || oldPort != entity.Port)
            {
                await _outboxService.AddMessageAsync(new DeviceUpdatedEvent(entity.Id, entity.IpAddress, entity.Port), cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _deviceDAL.GetAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
                return Result.Failure("Device couldn't find.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.DeleteAsync(entity, cancellationToken);

            await _outboxService.AddMessageAsync(new DeviceDeletedEvent(entity.Id,entity.IpAddress,entity.DeviceName), cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<List<DeviceDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var devices = await _deviceDAL.GetAllAsync(x => x.IsActive, cancellationToken);

           var dto = _mapper.Map<List<DeviceDTO>>(devices);
           return Result<List<DeviceDTO>>.Success(dto);
        }

        public async Task<Result<DeviceDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetAsync(x => x.Id == id && x.IsActive, cancellationToken);

            if (device == null)
                return Result<DeviceDTO>.Failure("Device couldn't find.");

            var dto = _mapper.Map<DeviceDTO>(device);

            return Result<DeviceDTO>.Success(dto);
        }
    }
}
