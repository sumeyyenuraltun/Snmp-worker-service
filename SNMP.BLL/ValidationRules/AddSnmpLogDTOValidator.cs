using FluentValidation;
using Snmp.Business.DTOs.SnmpLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.ValidationRules
{
    public class AddSnmpLogDTOValidator : AbstractValidator<AddSnmpLogDTO>
    {
        public AddSnmpLogDTOValidator()
        {
            RuleFor(x => x.DeviceId)
                .GreaterThan(0).WithMessage("Geçerli bir cihaz ID'si girilmelidir.");

            RuleFor(x => x.DeviceParameterId)
                .NotEmpty().WithMessage("OID adresi boş olamaz.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Log değeri boş olamaz.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Log tipi boş olamaz.");
        }
    }
}
