using FluentValidation;
using HouseBrokerApplication.Domain.Aggregates.Listing;

namespace HouseBrokerApplication.Application.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(address => address.Street)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(100).WithMessage("Street address must not exceed 100 characters.");

            RuleFor(address => address.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City must not exceed 50 characters.");

            RuleFor(address => address.State)
                .NotEmpty().WithMessage("State/Province is required.")
                .MaximumLength(50).WithMessage("State/Province must not exceed 50 characters.");

            RuleFor(address => address.ZipCode)
                .NotEmpty().WithMessage("Zip/Postal code is required.")
                .MaximumLength(10).WithMessage("Zip/Postal code must not exceed 10 characters.");
        }
    }
}
