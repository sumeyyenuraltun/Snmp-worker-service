using Snmp.EventWorker.EventHandlers.Abstract.SnmpCredential;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.SnmpCredential;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.SnmpCredential
{
    public class SnmpCredentialDeletedEventHandler : ISnmpCredentialDeletedEventHandler
    {
        private readonly ILogger<SnmpCredentialDeletedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public SnmpCredentialDeletedEventHandler(ILogger<SnmpCredentialDeletedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(SnmpCredentialDeletedEvent deviceParameterDeletedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceParameterDeletedEvent);

            _logger.LogInformation("SnmpCredentialDeletedEvent received. DeviceId:{DeviceId}", deviceParameterDeletedEvent.DeviceId);
            await _pollingManager.RestartAsync(deviceParameterDeletedEvent.DeviceId, cancellationToken);
        }
    }
}
