using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Polling
{
    public class PollingSession
    {
        public CancellationTokenSource CancellationTokenSource { get; init; } = null!;
        public PollingContext Context { get; init; } = null!;
    }
}
