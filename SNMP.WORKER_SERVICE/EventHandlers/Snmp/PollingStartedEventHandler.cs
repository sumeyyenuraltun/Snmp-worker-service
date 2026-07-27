using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Polling
{
    public class PollingStartedEventHandler : IPollingStartedEventHandler
    {
        private readonly ILogger<PollingStartedEventHandler> _logger;

        public PollingStartedEventHandler(ILogger<PollingStartedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(DevicePollingStartedEvent devicePollingStartedEvent, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(devicePollingStartedEvent);

            _logger.LogInformation("PollingStarted event received. DeviceId : {DeviceId} IpAddress: {IpAddress} Port : {Port}", devicePollingStartedEvent.AggregateId, devicePollingStartedEvent.IpAddress, devicePollingStartedEvent.Port);

            await Task.CompletedTask;
        }
    }
}
