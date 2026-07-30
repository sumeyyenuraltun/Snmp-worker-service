using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Polling;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class PollingStoppedEventHandler : IPollingStoppedEventHandler
    {
        private readonly ILogger<PollingStoppedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public PollingStoppedEventHandler(ILogger<PollingStoppedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(DevicePollingStoppedEvent devicePollingStoppedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(devicePollingStoppedEvent);
            _logger.LogInformation("Polling stopped event received. DeviceId={DeviceId}", devicePollingStoppedEvent.AggregateId);

            await _pollingManager.StopAsync(devicePollingStoppedEvent.DeviceId);

            _logger.LogInformation("Polling stopped for DeviceId={DeviceId}",devicePollingStoppedEvent.AggregateId);

        }
    }
}
