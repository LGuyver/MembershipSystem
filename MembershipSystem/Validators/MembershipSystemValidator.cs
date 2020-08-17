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
}
