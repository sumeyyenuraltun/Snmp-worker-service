using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.DeviceParameter
{
    public class AddDeviceParameterDTO
    {
        public int DeviceId { get; set; }
        public int ParameterId { get; set; }
    }
}
