using SNMP.ENTITY.Events.DeviceParameter;
using SNMP.ENTITY.Events.SnmpCredential;

namespace Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential
{
    public interface ISnmpCredentialCreatedEventHandler
    {
        Task HandleAsync(SnmpCredentialCreatedEvent deviceParameterCreatedEvent, CancellationToken cancellationToken = default);
    }
}
