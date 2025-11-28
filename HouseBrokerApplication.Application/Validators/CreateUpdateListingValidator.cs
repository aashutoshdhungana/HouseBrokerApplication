using FluentValidation;
using HouseBrokerApplication.Application.DTOs.Requests;

namespace HouseBrokerApplication.Application.Validators
{
    public class CreateUpdateListingValidator : AbstractValidator<CreateUpdateListingRequest>
    {
        public CreateUpdateListingValidator()
        {
            RuleFor(listing => listing.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(listing => listing.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(20).WithMessage("Description must be at least 20 characters long.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            // 2. Financial and Type Validation
            RuleFor(listing => listing.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(listing => listing.PropertyType)
                .IsInEnum().WithMessage("Invalid property type selected.");

            // 3. Contact and Broker Validation
            RuleFor(listing => listing.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(150).WithMessage("Contact email must not exceed 150 characters.");

            RuleFor(listing => listing.ContactPhone)
                .NotEmpty().WithMessage("Contact phone number is required.")
                // Basic regex for 10-15 digits, adjust based on your international format needs
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Contact phone number must be a valid format (10-15 digits, optional leading +).");

            RuleFor(listing => listing.Address).NotNull().WithMessage("Address details are required.")
                            .SetValidator(new AddressValidator());
        }
    }
}
