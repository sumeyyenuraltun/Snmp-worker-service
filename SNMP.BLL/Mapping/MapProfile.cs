using AutoMapper;
using Snmp.Business.DTOs.Auth;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.DTOs.Devices;
using Snmp.Business.DTOs.Parameter;
using Snmp.Business.DTOs.Role;
using Snmp.Business.DTOs.SnmpCredentials;
using Snmp.Business.DTOs.User;
using Snmp.Entity.Concrete;
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

            CreateMap<Parameter, ParameterDTO>().ReverseMap();
            CreateMap<Parameter, AddParameterDTO>().ReverseMap();
            CreateMap<Parameter, UpdateParameterDTO>().ReverseMap();

            CreateMap<DeviceParameter, DeviceParameterDTO>()
              .ForMember(dest => dest.ParameterName,
                  opt => opt.MapFrom(src => src.Parameter != null ? src.Parameter.Name : string.Empty))
              .ForMember(dest => dest.Oid,
                  opt => opt.MapFrom(src => src.Parameter != null ? src.Parameter.Oid : string.Empty))
              .ForMember(dest => dest.DataType,
                  opt => opt.MapFrom(src => src.Parameter != null ? src.Parameter.DataType : default));

            CreateMap<DeviceParameter, AddDeviceParameterDTO>().ReverseMap();
            CreateMap<DeviceParameter, UpdateDeviceParameterDTO>().ReverseMap();

            CreateMap<SnmpCredential, SnmpCredentialDTO>().ReverseMap();
            CreateMap<SnmpCredential, AddSnmpCredentialDTO>().ReverseMap();
            CreateMap<SnmpCredential, UpdateSnmpCredentialDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, UserAuthDTO>().ForMember(dest => dest.RoleName,opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<RegisterRequestDTO, User>();

            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<Role, UpdateRoleDTO>().ReverseMap();
            CreateMap<Role, AddRoleDTO>().ReverseMap();
            CreateMap<Role, UpdateUserRoleDTO>().ReverseMap();

        }
    }
}
