using Snmp.EventWorker.Cache.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Cache
{
    public class CachedDeviceConfiguration
    {
        public DeviceConfiguration Configuration { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
