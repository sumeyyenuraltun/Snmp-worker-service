using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Devices
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public int Port { get; set; }
        public int TotalLogCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
