using FluentValidation;
using Snmp.Business.DTOs.Devices;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SNMP.BLL.ValidationRules
{
    public class AddDeviceDTOValidator : AbstractValidator<AddDeviceDTO>
    {
        public AddDeviceDTOValidator()
        {
            RuleFor(x => x.DeviceName).NotEmpty().WithMessage("Device name cannot be empty.");
            RuleFor(x => x.DeviceName).MaximumLength(100).WithMessage("Device name cannot exceed 100 characters.");
            RuleFor(x => x.IpAddress).NotEmpty().Must(ip => IPAddress.TryParse(ip, out _)).WithMessage("Invalid IP Address.");
            RuleFor(x => x.Port).InclusiveBetween(1, 65535).WithMessage("Port must be between 1 and 65535.");

        }
    }
}
