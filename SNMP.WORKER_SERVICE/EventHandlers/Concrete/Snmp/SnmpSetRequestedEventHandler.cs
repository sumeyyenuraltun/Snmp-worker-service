using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Snmp.Manager;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class SnmpSetRequestedEventHandler : ISnmpSetRequestedEventHandler
    {
        private readonly ILogger<SnmpSetRequestedEventHandler> _logger;
        private readonly ISnmpRequestManager _snmpRequestManager;

        public SnmpSetRequestedEventHandler(ILogger<SnmpSetRequestedEventHandler> logger, ISnmpRequestManager snmpRequestManager)
        {
            _logger = logger;
            _snmpRequestManager = snmpRequestManager;
        }

        public async Task HandleAsync(SnmpSetRequestedEvent snmpSetRequestedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(snmpSetRequestedEvent);

            _logger.LogInformation("SNMP SET requested. DeviceId:{DeviceId}, OID:{Oid}", snmpSetRequestedEvent.DeviceId, snmpSetRequestedEvent.Oid);

            await _snmpRequestManager.ExecuteSetAsync(snmpSetRequestedEvent, cancellationToken);

            _logger.LogInformation("SNMP SET completed. DeviceId:{DeviceId}", snmpSetRequestedEvent.DeviceId);
        }
    }
}
