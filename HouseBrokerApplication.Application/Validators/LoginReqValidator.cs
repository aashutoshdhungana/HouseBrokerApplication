using FluentValidation;
using HouseBrokerApplication.Application.DTOs.Requests;

namespace HouseBrokerApplication.Application.Validators
{
    public class LoginReqValidator : AbstractValidator<LoginReq>
    {
        public LoginReqValidator()
        {
            RuleFor(l => l.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(l => l.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
