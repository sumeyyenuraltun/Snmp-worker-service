using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.DeviceParameter
{
    public class UpdateDeviceParameterDTO
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int ParameterId { get; set; }

        public bool IsEnabled { get; set; }

        public int PollingIntervalSeconds { get; set; }

        public double? MinThreshold { get; set; }

        public double? MaxThreshold { get; set; }
    }
}
