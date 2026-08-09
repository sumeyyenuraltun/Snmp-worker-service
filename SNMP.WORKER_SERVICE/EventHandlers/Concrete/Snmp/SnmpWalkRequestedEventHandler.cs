using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Snmp.Manager;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class SnmpWalkRequestedEventHandler : ISnmpWalkRequestedEventHandler
    {
        private readonly ILogger<SnmpWalkRequestedEventHandler> _logger;
        private readonly ISnmpRequestManager _snmpRequestManager;

        public SnmpWalkRequestedEventHandler(ILogger<SnmpWalkRequestedEventHandler> logger, ISnmpRequestManager snmpRequestManager)
        {
            _logger = logger;
            _snmpRequestManager = snmpRequestManager;
        }
        public async Task HandleAsync(SnmpWalkRequestedEvent snmpWalkRequestedEvent,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(snmpWalkRequestedEvent);

            _logger.LogInformation("SNMP WALK requested. DeviceId:{DeviceId}, RootOid:{Oid}",snmpWalkRequestedEvent.DeviceId,  snmpWalkRequestedEvent.RootParameterId);

            await _snmpRequestManager.ExecuteWalkAsync(snmpWalkRequestedEvent,cancellationToken);

            _logger.LogInformation("SNMP WALK completed. DeviceId:{DeviceId}",snmpWalkRequestedEvent.DeviceId);
        }
    }
}
