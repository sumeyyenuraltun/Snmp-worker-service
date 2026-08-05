using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.SnmpCredential
{
    public class SnmpCredentialCreatedEventHandler : ISnmpCredentialCreatedEventHandler
    {
        private readonly ILogger<SnmpCredentialCreatedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public SnmpCredentialCreatedEventHandler(ILogger<SnmpCredentialCreatedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(SnmpCredentialCreatedEvent deviceParameterCreatedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterCreatedEvent);

            _logger.LogInformation("SnmpCredentialCreatedEvent received. DeviceId:{DeviceId}", deviceParameterCreatedEvent.DeviceId);
            await _pollingManager.RestartAsync(deviceParameterCreatedEvent.DeviceId, cancellationToken);
        }
    }
}
