using Snmp.EventWorker.Cache.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Polling
{
    public class PollingSession
    {
        public required CancellationTokenSource CancellationTokenSource { get; init; }

        public required DeviceConfiguration Configuration { get; init; }

        public List<Task> RunningTasks { get; } = new();
    }
}
