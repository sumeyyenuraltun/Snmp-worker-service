using AutoMapper;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.Abstract.Outbox;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using SNMP.ENTITY.Events.DeviceParameter;


namespace Snmp.Business.Concrete.DeviceService
{
    public class DeviceParameterService : IDeviceParameterService
    {
        private readonly IDeviceParameterDAL _deviceParameterDAL;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;

        public DeviceParameterService(IDeviceParameterDAL deviceParameterDAL, IMapper mapper, IUnitOfWork unitOfWork, IOutboxService outboxService)
        {
            _deviceParameterDAL = deviceParameterDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<Result<List<DeviceParameterDTO>>> GetByDeviceIdAsync(int deviceId)
        {
            var entities = await _deviceParameterDAL.GetByDeviceIdAsync(deviceId);

            var dto = _mapper.Map<List<DeviceParameterDTO>>(entities);

            return Result<List<DeviceParameterDTO>>.Success(dto);
        }

        public async Task<Result> AddAsync(AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<DeviceParameter>(addDeviceParameterDTO);

            await _deviceParameterDAL.AddAsync(entity, cancellationToken);

            await _outboxService.AddMessageAsync(new DeviceParameterCreatedEvent(entity.DeviceId), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _deviceParameterDAL.GetByIdAsync(id);
            var deviceId = entity!.DeviceId;

            if (entity == null)
                return Result.Failure("Device parameter not found.");

            await _deviceParameterDAL.DeleteAsync(entity);
            await _outboxService.AddMessageAsync(new DeviceParameterDeletedEvent(deviceId),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
        public async Task<Result> UpdateAsync(UpdateDeviceParameterDTO updateDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var entity = await _deviceParameterDAL.GetByIdAsync(updateDeviceParameterDTO.Id);

            if (entity == null)
                return Result.Failure("Device parameter not found.");

            entity.DeviceId = updateDeviceParameterDTO.DeviceId;
            entity.ParameterId = updateDeviceParameterDTO.ParameterId;

            await _deviceParameterDAL.UpdateAsync(entity);
            await _outboxService.AddMessageAsync(new DeviceParameterUpdatedEvent(entity.DeviceId),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
        public async Task<Result<DeviceParameterDTO>> GetByIdAsync(int id)
        {
            var entity = await _deviceParameterDAL.GetAsync(x => x.Id == id,x => x.Parameter);

            if (entity == null)
                return Result<DeviceParameterDTO>.Failure("Device parameter not found.");

            var dto = _mapper.Map<DeviceParameterDTO>(entity);

            return Result<DeviceParameterDTO>.Success(dto);
        }

      
    }
}
