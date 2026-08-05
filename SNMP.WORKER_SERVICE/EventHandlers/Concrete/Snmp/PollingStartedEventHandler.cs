using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Snmp.Polling;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class PollingStartedEventHandler : IPollingStartedEventHandler
    {
        private readonly ILogger<PollingStartedEventHandler> _logger;
        private readonly IPollingManager _pollingManager;

        public PollingStartedEventHandler(ILogger<PollingStartedEventHandler> logger, IPollingManager pollingManager)
        {
            _logger = logger;
            _pollingManager = pollingManager;
        }

        public async Task HandleAsync(DevicePollingStartedEvent devicePollingStartedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(devicePollingStartedEvent);

            _logger.LogInformation("PollingStarted event received. DeviceId", devicePollingStartedEvent.AggregateId);

            await _pollingManager.StartAsync(devicePollingStartedEvent,cancellationToken);

            _logger.LogInformation("Polling manager started for DeviceId: {DeviceId}",devicePollingStartedEvent.DeviceId);
        }
    }
}
