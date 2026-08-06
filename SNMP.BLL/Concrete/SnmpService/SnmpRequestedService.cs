using Snmp.Business.Abstract.Outbox;
using Snmp.Business.Abstract.Snmp;
using Snmp.Business.DTOs.Snmp;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Events.Snmp;

namespace Snmp.Business.Concrete.SnmpService
{
    public class SnmpRequestedService : ISnmpRequestedService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IOutboxService _outboxService;
        private readonly IUnitOfWork _unitOfWork;

        public SnmpRequestedService(IDeviceDAL deviceDAL, IOutboxService outboxService, IUnitOfWork unitOfWork)
        {
            _deviceDAL = deviceDAL;
            _outboxService = outboxService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> SendGetRequestedAsync(SnmpRequestDTO snmpRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpRequestDTO.DeviceId);

            if (device == null)
                return Result.Failure("Device couldn't find.");

            await _outboxService.AddMessageAsync(new SnmpGetRequestedEvent(snmpRequestDTO.DeviceId,snmpRequestDTO.Oid, snmpRequestDTO.TimeoutMilliseconds),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> SendWalkRequestedAsync(SnmpWalkRequestDTO snmpWalkRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpWalkRequestDTO.DeviceId);

            if (device == null)
                return Result.Failure("Device couldn't find.");

            await _outboxService.AddMessageAsync(new SnmpWalkRequestedEvent( snmpWalkRequestDTO.DeviceId,snmpWalkRequestDTO.RootOid,snmpWalkRequestDTO.TimeoutMilliseconds),cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> SendSetRequestedAsync(SnmpSetRequestDTO snmpSetRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpSetRequestDTO.DeviceId);

            if(device == null)
                return Result.Failure("Device couldn't find.");
            

            await _outboxService.AddMessageAsync(new SnmpSetRequestedEvent(snmpSetRequestDTO.DeviceId, snmpSetRequestDTO.Oid, snmpSetRequestDTO.Value, snmpSetRequestDTO.TimeoutMilliseconds), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> SendGetNextRequestedAsync(SnmpGetNextRequestDTO snmpGetNextRequestDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetByIdAsync(snmpGetNextRequestDTO.DeviceId);

            if(device == null)
            {
                return Result.Failure("Device couldn't find");
            }

            await _outboxService.AddMessageAsync(new SnmpGetNextRequestedEvent(snmpGetNextRequestDTO.DeviceId, snmpGetNextRequestDTO.Oid, snmpGetNextRequestDTO.TimeoutMilliseconds), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}
