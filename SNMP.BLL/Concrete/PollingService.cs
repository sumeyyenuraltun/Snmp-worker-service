using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Polling;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Events.Snmp;

namespace Snmp.Business.Concrete
{
    public class PollingService : IPollingService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IOutboxService _outboxService;
        private readonly IUnitOfWork _unitOfWork;
        public PollingService(IDeviceDAL deviceDAL, IOutboxService outboxService, IUnitOfWork unitOfWork)
        {
            _deviceDAL = deviceDAL;
            _outboxService = outboxService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> StartAsync(StartPollingDTO startPollingDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetAsync(
                x => x.Id == startPollingDTO.DeviceId && x.IsActive);

            if (device is null)
                return Result.Failure("Device not found.");

            await _outboxService.AddMessageAsync(
                new DevicePollingStartedEvent(startPollingDTO.DeviceId),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> StopAsync(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetAsync(
                x => x.Id == stopPollingDTO.DeviceId && x.IsActive);

            if (device is null)
                return Result.Failure("Device not found.");

            await _outboxService.AddMessageAsync(
                new DevicePollingStoppedEvent(stopPollingDTO.DeviceId),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
