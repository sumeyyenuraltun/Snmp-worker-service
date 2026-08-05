using SNMP.ENTITY.Events.Device;


namespace Snmp.EventWorker.EventHandlers.Abstract.Device
{
    public interface IDeviceUpdatedEventHandler
    {
        Task HandleAsync(DeviceUpdatedEvent deviceUpdatedEvent, CancellationToken cancellation);
    }
}
