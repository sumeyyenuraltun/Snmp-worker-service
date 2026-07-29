using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.DeviceParameter
{
    public class DeviceParameterDTO
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int ParameterId { get; set; }

        public string ParameterName { get; set; } = string.Empty;

        public string Oid { get; set; } = string.Empty;

        public bool IsEnabled { get; set; }

        public int PollingIntervalSeconds { get; set; }

        public double? MinThreshold { get; set; }

        public double? MaxThreshold { get; set; }
    }
}
