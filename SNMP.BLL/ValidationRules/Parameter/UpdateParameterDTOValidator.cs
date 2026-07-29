using FluentValidation;
using Snmp.Business.DTOs.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.ValidationRules.Parameter
{
    public class UpdateParameterDTOValidator : AbstractValidator<UpdateParameterDTO>
    {
        public UpdateParameterDTOValidator()
        {
            RuleFor(x => x.Id)
                 .GreaterThan(0)
                 .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Parameter name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Oid)
                .NotEmpty().WithMessage("OID is required.")
                .Matches(@"^\.?(\d+\.)*\d+$")
                .WithMessage("Invalid OID format.");

            RuleFor(x => x.DataType)
                .NotEmpty().WithMessage("Data type is required.")
                .MaximumLength(50);

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Unit is required.")
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
