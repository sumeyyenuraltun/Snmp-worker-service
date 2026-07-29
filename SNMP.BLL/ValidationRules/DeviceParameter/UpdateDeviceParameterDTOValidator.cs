using FluentValidation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Snmp.Business.DTOs.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.ValidationRules.DeviceParameter
{
    public class UpdateDeviceParameterDTOValidator : AbstractValidator<UpdateDeviceParameterDTO>
    {
        public UpdateDeviceParameterDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.DeviceId)
                .GreaterThan(0)
                .WithMessage("DeviceId must be greater than 0.");

            RuleFor(x => x.ParameterId)
                .GreaterThan(0)
                .WithMessage("ParameterId must be greater than 0.");

            
        }
    }
}
