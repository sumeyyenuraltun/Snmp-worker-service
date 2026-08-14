using AutoMapper;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.Abstract.Outbox;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;
using SNMP.ENTITY.Events.SnmpCredential;


namespace Snmp.Business.Concrete.DeviceService
{
    public class SnmpCredentialService : ISnmpCredentialService
    {
        private readonly ISnmpCredentialDAL _snmpCredentialDAL;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;

        public SnmpCredentialService(ISnmpCredentialDAL snmpCredentialDAL, IMapper mapper, IUnitOfWork unitOfWork, IOutboxService outboxService )
        {
            _snmpCredentialDAL = snmpCredentialDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<Result> AddAsync(AddSnmpCredentialDTO dto, CancellationToken cancellationToken)
        {
            var exists = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive, cancellationToken);

            if (exists != null)
                return Result.Failure("SNMP credentials already exist for this device.");

            var entity = _mapper.Map<SnmpCredential>(dto);

            await _snmpCredentialDAL.AddAsync(entity, cancellationToken);
            await _outboxService.AddMessageAsync(new SnmpCredentialCreatedEvent(entity.DeviceId),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateSnmpCredentialDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive, cancellationToken);

            if (entity == null)
                return Result.Failure("SNMP credentials not found.");

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity, cancellationToken);

            await _outboxService.AddMessageAsync(new SnmpCredentialUpdatedEvent(entity.DeviceId),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return Result.Failure("SNMP credentials not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity, cancellationToken);
            await _outboxService.AddMessageAsync(new SnmpCredentialDeletedEvent( entity.DeviceId ),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<SnmpCredentialDTO>> GetByDeviceIdAsync(int deviceId , CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == deviceId &&
                x.IsActive, cancellationToken);

            if (entity == null)
                return Result<SnmpCredentialDTO>.Failure("SNMP credentials not found.");

            var dto = _mapper.Map<SnmpCredentialDTO>(entity);

            return Result<SnmpCredentialDTO>.Success(dto);
        }
        public async Task<Result<List<SnmpCredentialDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var credentials = await _snmpCredentialDAL.GetAllAsync(cancellationToken);

            var dto = _mapper.Map<List<SnmpCredentialDTO>>(credentials);

            return Result<List<SnmpCredentialDTO>>.Success(dto);
        }
        public async Task<Result<SnmpCredentialDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return Result<SnmpCredentialDTO>.Failure("SNMP credentials not found.");

            var dto = _mapper.Map<SnmpCredentialDTO>(entity);

            return Result<SnmpCredentialDTO>.Success(dto);
        }
    }
}
