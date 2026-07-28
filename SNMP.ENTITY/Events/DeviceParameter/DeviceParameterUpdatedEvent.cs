using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.ENTITY.Events.DeviceParameter
{
    public class DeviceParameterUpdatedEvent : BaseEvent
    {
        public DeviceParameterUpdatedEvent(int deviceId, int deviceParameterId, int parameterId, bool ısEnable, int pollingIntervalSeconds, double? minThreshold, double? maxThreshold)
        {
            DeviceId = deviceId;
            DeviceParameterId = deviceParameterId;
            ParameterId = parameterId;
            IsEnable = ısEnable;
            PollingIntervalSeconds = pollingIntervalSeconds;
            MinThreshold = minThreshold;
            MaxThreshold = maxThreshold;
        }

        public int DeviceId { get; }
        public int DeviceParameterId { get; }
        public int ParameterId { get; }
        public bool IsEnable { get; }
        public int PollingIntervalSeconds { get; }
        public double? MinThreshold { get; }
        public double? MaxThreshold { get; }

    }
}

