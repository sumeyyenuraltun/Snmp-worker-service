using FluentValidation;
using Snmp.Business.DTOs.SnmpCredentials;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.ValidationRules.SnmpCredentialValidator
{
    public class UpdateSnmpCredentialDTOValidator : AbstractValidator<UpdateSnmpCredentialDTO>
    {
        public UpdateSnmpCredentialDTOValidator()
        {
            RuleFor(x => x.Id)
            .GreaterThan(0);

            RuleFor(x => x.DeviceId)
                .GreaterThan(0);

            RuleFor(x => x.Version)
                .IsInEnum();

            When(x => x.Version == SnmpVersion.V2c, () =>
            {
                RuleFor(x => x.Community)
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.UserName)
                    .Empty();

                RuleFor(x => x.AuthPassword)
                    .Empty();

                RuleFor(x => x.PrivacyPassword)
                    .Empty();
            });

            When(x => x.Version == SnmpVersion.V3, () =>
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.SecurityLevel)
                    .NotNull();

                RuleFor(x => x.AuthProtocol)
                    .NotNull();

                RuleFor(x => x.AuthPassword)
                    .NotEmpty();

                RuleFor(x => x.PrivacyProtocol)
                    .NotNull();

                RuleFor(x => x.PrivacyPassword)
                    .NotEmpty();

                RuleFor(x => x.Community)
                    .Empty();
            });
        }
    }
}
