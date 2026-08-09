using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Redis
{
    public class RedisKey
    {
        public static string LatestValue (int deviceId, int parameterId)
        {
            return $"snmp:{deviceId}:{parameterId}";
        }
    }
}
