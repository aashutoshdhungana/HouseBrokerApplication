using FluentValidation;
using HouseBrokerApplication.Application.DTOs.Requests;

namespace HouseBrokerApplication.Application.Validators
{
    public class RegisterBrokerValidator : AbstractValidator<RegisterUserReq>
    {
        public RegisterBrokerValidator()
        {
            RuleFor(user => user.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(user => user.Password).NotEmpty().WithMessage("Passwird is required")
                .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,}$\r\n")
                .WithMessage("Password is weak");
            RuleFor(user => user.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(user => user.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(user => user.ContactEmail).NotEmpty().WithMessage("Contact email is required");
            RuleFor(user => user.ContactPhone).NotEmpty().WithMessage("Contact phone is required");
        }
    }
}
