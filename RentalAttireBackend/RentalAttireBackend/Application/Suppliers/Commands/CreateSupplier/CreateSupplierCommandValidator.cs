using FluentValidation;

namespace RentalAttireBackend.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(x => x.SupplierName)
                .NotEmpty()
                .WithMessage("Supplier name is required")
                .MaximumLength(100)
                .WithMessage("Supplier name must not exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required")
                .MaximumLength(200)
                .WithMessage("Address should not exceed 200 characters");

            RuleFor(x => x.PhoneNumber)
                .Length(11)
                .WithMessage("Invalid phone number digits");

            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}
