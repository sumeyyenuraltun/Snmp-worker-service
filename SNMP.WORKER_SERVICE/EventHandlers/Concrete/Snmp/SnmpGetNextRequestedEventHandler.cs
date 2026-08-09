using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Snmp.Manager;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class SnmpGetNextRequestedEventHandler : ISnmpGetNextRequestedEventHandler
    {
        private readonly ILogger<SnmpGetNextRequestedEventHandler> _logger;
        private readonly ISnmpRequestManager _snmpRequestManager;

        public SnmpGetNextRequestedEventHandler(ILogger<SnmpGetNextRequestedEventHandler> logger, ISnmpRequestManager snmpRequestManager)
        {
            _logger = logger;
            _snmpRequestManager = snmpRequestManager;
        }
        public async Task HandleAsync(SnmpGetNextRequestedEvent snmpGetNextRequestedEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(snmpGetNextRequestedEvent);

            _logger.LogInformation("SNMP GETNEXT requested. DeviceId:{DeviceId}, OID:{Oid}", snmpGetNextRequestedEvent.DeviceId, snmpGetNextRequestedEvent.ParameterId);

            await _snmpRequestManager.ExecuteGetNextAsync(snmpGetNextRequestedEvent, cancellationToken);

            _logger.LogInformation("SNMP GET completed. DeviceId:{DeviceId}", snmpGetNextRequestedEvent.DeviceId);
        }
    }
}
