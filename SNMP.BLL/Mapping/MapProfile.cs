using AutoMapper;
using Snmp.Business.DTOs.Devices;
using Snmp.Business.DTOs.SnmpLogs;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Mapping
{
    public class MapProfile  :Profile
    {
        public MapProfile()
        {
            CreateMap<Device, DeviceDTO>().ReverseMap();
            CreateMap<Device, AddDeviceDTO>().ReverseMap();
            CreateMap<Device, UpdateDeviceDTO>().ReverseMap();

            CreateMap<SnmpLog, SnmpLogDTO>().ReverseMap();
            CreateMap<SnmpLog, AddSnmpLogDTO>().ReverseMap();
            CreateMap<SnmpLog, UpdateSnmpLogDTO>().ReverseMap();

        }
    }
}
