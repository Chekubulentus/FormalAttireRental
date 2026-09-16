using FluentValidation;

namespace RentalAttireBackend.Application.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
    {
        public UpdateCustomerProfileCommandValidator()
        {
            // ============================================================
            // Person fields
            // ============================================================
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(100)
                .WithMessage("Last name must not exceed 100 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(100)
                .WithMessage("First name must not exceed 100 characters.");

            // Middle name intentionally left optional (default empty string
            // on PersonDTO suggests it's not always required in this context).
            RuleFor(x => x.MiddleName)
                .MaximumLength(100)
                .WithMessage("Middle name must not exceed 100 characters.");

            // ASSUMPTION: 18–120 as a sane bound for an adult rental customer.
            // Adjust if there's a different actual policy (e.g. no upper bound).
            RuleFor(x => x.Age)
                .InclusiveBetween(18, 120)
                .WithMessage("Age must be between 18 and 120.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.");

            RuleFor(x => x.MaritalStatus)
                .NotEmpty()
                .WithMessage("Marital status is required.");

            // ASSUMPTION: Philippine mobile format (09XXXXXXXXX or +639XXXXXXXXX).
            // Loosen/replace if landlines or other formats need to be accepted.
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^(09|\+639)\d{9}$")
                .WithMessage("Phone number must be a valid Philippine mobile number.");

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Street is required.")
                .MaximumLength(200)
                .WithMessage("Street must not exceed 200 characters.");

            RuleFor(x => x.Barangay)
                .NotEmpty()
                .WithMessage("Barangay is required.")
                .MaximumLength(100)
                .WithMessage("Barangay must not exceed 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(100)
                .WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Province)
                .NotEmpty()
                .WithMessage("Province is required.")
                .MaximumLength(100)
                .WithMessage("Province must not exceed 100 characters.");

            // ASSUMPTION: 4-digit PH postal code format.
            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .WithMessage("Postal code is required.")
                .Matches(@"^\d{4}$")
                .WithMessage("Postal code must be a valid 4-digit postal code.");

            // ============================================================
            // Password change — optional. CurrentPassword and password
            // rules only apply when the customer is actually changing it,
            // signaled by NewPassword being provided.
            // ============================================================
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required to change your password.")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            // ASSUMPTION: min 8 chars, at least one letter and one number.
            // Align with whatever policy Registration actually enforces.
            RuleFor(x => x.NewPassword)
                .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$")
                .WithMessage("New password must contain at least one letter and one number.")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Please confirm your new password.")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match.")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));
        }
    }
}