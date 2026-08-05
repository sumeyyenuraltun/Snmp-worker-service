using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;


namespace Snmp.Business.Concrete
{
    public class SnmpCredentialService : ISnmpCredentialService
    {
        private readonly ISnmpCredentialDAL _snmpCredentialDAL;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SnmpCredentialService(ISnmpCredentialDAL snmpCredentialDAL, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _snmpCredentialDAL = snmpCredentialDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddAsync(AddSnmpCredentialDTO dto, CancellationToken cancellationToken)
        {
            var exists = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive);

            if (exists != null)
                return Result.Failure("SNMP credentials already exist for this device.");

            var entity = _mapper.Map<SnmpCredential>(dto);

            await _snmpCredentialDAL.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateSnmpCredentialDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == dto.DeviceId &&
                x.IsActive);

            if (entity == null)
                return Result.Failure("SNMP credentials not found.");

            _mapper.Map(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _snmpCredentialDAL.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure("SNMP credentials not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _snmpCredentialDAL.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<SnmpCredentialDTO>> GetByDeviceIdAsync(int deviceId)
        {
            var entity = await _snmpCredentialDAL.GetAsync(x =>
                x.DeviceId == deviceId &&
                x.IsActive);

            if (entity == null)
                return Result<SnmpCredentialDTO>.Failure("SNMP credentials not found.");

            var dto = _mapper.Map<SnmpCredentialDTO>(entity);

            return Result<SnmpCredentialDTO>.Success(dto);
        }
        public async Task<Result<List<SnmpCredentialDTO>>> GetAllAsync()
        {
            var credentials = await _snmpCredentialDAL.GetAllAsync();

            var dto = _mapper.Map<List<SnmpCredentialDTO>>(credentials);

            return Result<List<SnmpCredentialDTO>>.Success(dto);
        }
        public async Task<Result<SnmpCredentialDTO>> GetByIdAsync(int id)
        {
            var entity = await _snmpCredentialDAL.GetByIdAsync(id);

            if (entity == null)
                return Result<SnmpCredentialDTO>.Failure("SNMP credentials not found.");

            var dto = _mapper.Map<SnmpCredentialDTO>(entity);

            return Result<SnmpCredentialDTO>.Success(dto);
        }
    }
}
