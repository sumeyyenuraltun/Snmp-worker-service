using FluentValidation;
using Snmp.Business.DTOs.DeviceParameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.ValidationRules.DeviceParameter
{
    public class AddDeviceParameterDTOValidator : AbstractValidator<AddDeviceParameterDTO>
    {
        public AddDeviceParameterDTOValidator()
        {
            RuleFor(x => x.DeviceId)
               .GreaterThan(0)
               .WithMessage("DeviceId must be greater than 0.");

            RuleFor(x => x.ParameterId)
                .GreaterThan(0)
                .WithMessage("ParameterId must be greater than 0.");

            RuleFor(x => x.PollingIntervalSeconds)
                .GreaterThan(0)
                .WithMessage("Polling interval must be greater than 0.");

            RuleFor(x => x)
                .Must(x => !x.MinThreshold.HasValue ||
                           !x.MaxThreshold.HasValue ||
                           x.MinThreshold <= x.MaxThreshold)
                .WithMessage("MinThreshold cannot be greater than MaxThreshold.");
        }
    }
}
