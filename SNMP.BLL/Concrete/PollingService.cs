using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Polling;
using Snmp.DataAccess.Abstract;
using SNMP.DAL.Abstract;
using SNMP.ENTITY.Abstract;
using SNMP.ENTITY.Events.Snmp;

namespace Snmp.Business.Concrete
{
    public class PollingService : IPollingService
    {
        private readonly IDeviceDAL _deviceDAL;
        private readonly IEventPublisher _eventPublisher;

        public PollingService(IDeviceDAL deviceDAL, IEventPublisher eventPublisher)
        {
            _deviceDAL = deviceDAL;
            _eventPublisher = eventPublisher;
        }

        public async Task StartAsync(StartPollingDTO startPollingDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetAsync(x => x.Id == startPollingDTO.DeviceId && x.IsActive);

            if (device == null)
                throw new Exception("Device not found.");

            if (device.PollingEnabled)
                throw new Exception("Polling is already enabled for this device.");

            device.PollingEnabled = true;
            device.PollingIntervalSeconds = startPollingDTO.IntervalSeconds;
            device.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(device);

            await _eventPublisher.PublishAsync(new DevicePollingStartedEvent( device.Id,device.IpAddress,device.Port,startPollingDTO.IntervalSeconds),cancellationToken);
        }

        public async Task StopAsync(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken)
        {
            var device = await _deviceDAL.GetAsync(x => x.Id == stopPollingDTO.DeviceId && x.IsActive);

            if (device == null)
                throw new Exception("Device not found.");

            if (!device.PollingEnabled)
                throw new Exception("Polling is already stopped.");

            device.PollingEnabled = false;
            device.UpdatedAt = DateTime.UtcNow;

            await _deviceDAL.UpdateAsync(device);

            await _eventPublisher.PublishAsync(
                new DevicePollingStoppedEvent(stopPollingDTO.DeviceId),
                cancellationToken);
        }
    }
}
