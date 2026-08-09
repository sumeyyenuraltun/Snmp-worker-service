using Snmp.EventWorker.EventHandlers.Abstract.Snmp;
using Snmp.EventWorker.Snmp.Manager;
using SNMP.ENTITY.Events.Snmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.EventHandlers.Concrete.Snmp
{
    public class SnmpGetRequestedEventHandler : ISnmpGetRequestedEventHandler
    {
        private readonly ILogger<SnmpGetRequestedEventHandler> _logger;
        private readonly ISnmpRequestManager _snmpRequestManager;

        public SnmpGetRequestedEventHandler(ILogger<SnmpGetRequestedEventHandler> logger, ISnmpRequestManager snmpRequestManager)
        {
            _logger = logger;
            _snmpRequestManager = snmpRequestManager;
        }

        public async Task HandleAsync(SnmpGetRequestedEvent snmpGetRequestedEvent,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(snmpGetRequestedEvent);

            _logger.LogInformation("SNMP GET requested. DeviceId:{DeviceId}, OID:{Oid}",snmpGetRequestedEvent.DeviceId,snmpGetRequestedEvent.ParameterId);

            await _snmpRequestManager.ExecuteGetAsync(snmpGetRequestedEvent,cancellationToken);

            _logger.LogInformation("SNMP GET completed. DeviceId:{DeviceId}",snmpGetRequestedEvent.DeviceId);
        }
    }
}
