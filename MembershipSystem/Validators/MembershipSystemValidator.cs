using FluentValidation;
using MembershipSystem.Models;

namespace MembershipSystem.Validators
{
    public class MembershipSystemRequestValidator : AbstractValidator<MembershipSystemRequest>
    {
        public MembershipSystemRequestValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().Length(16);
        }
    }

    public class MembershipSignupRequestValidator : AbstractValidator<MembershipSignupRequest>
    {
        public MembershipSignupRequestValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().Length(16);
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Company).NotEmpty();
            RuleFor(x => x.Pin.ToString()).NotEmpty().Length(4);
        }
    }
}
