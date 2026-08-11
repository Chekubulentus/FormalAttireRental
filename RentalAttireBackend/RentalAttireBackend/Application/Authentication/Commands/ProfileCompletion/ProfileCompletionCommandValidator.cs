using FluentValidation;
using Google.Apis.Util;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Authentication.Commands.ProfileCompletion
{
    public class ProfileCompletionCommandValidator : AbstractValidator<ProfileCompletionCommand>
    {
        public ProfileCompletionCommandValidator()
        {

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 100)
                .WithMessage("Age must be greater than 0.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.")
                .Must(x => Enum.TryParse<Gender>(x, true, out _))
                .WithMessage("Invalid gender.");

            RuleFor(x => x.MaritalStatus)
                .NotEmpty()
                .WithMessage("Marital status is required.")
                .Must(x => Enum.TryParse<MaritalStatus>(x, true, out _))
                .WithMessage("Invalid marital status.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MinimumLength(10)
                .WithMessage("Invalid phone number");

            RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

            RuleFor(x => x.Barangay)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Province)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .Matches(@"^\d{4}$")
                .WithMessage("Postal code must contain exactly 4 digits.");
        }
    }
}
