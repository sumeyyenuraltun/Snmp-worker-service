using Snmp.EventWorker.EventHandlers.Polling;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Snmp
{
    public class PollingStoppedEventHandler : IPollingStoppedEventHandler
    {
        private readonly ILogger<PollingStoppedEventHandler> _logger;

        public PollingStoppedEventHandler(ILogger<PollingStoppedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(DevicePollingStoppedEvent devicePollingStoppedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(devicePollingStoppedEvent);

            _logger.LogInformation("Polling stopped for DeviceId={DeviceId}",devicePollingStoppedEvent.AggregateId);

            await Task.CompletedTask;
        }
    }
}
