using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.SnmpCredential
{
    public class SnmpCredentialUpdatedEventHandler : ISnmpCredentialUpdatedEventHandler
    {
        private readonly ILogger<SnmpCredentialUpdatedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public SnmpCredentialUpdatedEventHandler(ILogger<SnmpCredentialUpdatedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(SnmpCredentialUpdatedEvent deviceParameterUpdatedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterUpdatedEvent);

            _logger.LogInformation("SnmpCredentialDeletedEvent received. DeviceId:{DeviceId}", deviceParameterUpdatedEvent.DeviceId);
            await _pollingManager.RestartAsync(deviceParameterUpdatedEvent.DeviceId, cancellationToken);
        }
    }
}
