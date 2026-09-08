using FluentValidation;

namespace RentalAttireBackend.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("Supplier record does not exist")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid supplier identifier");

            RuleFor(x => x.SupplierName)
                .NotEmpty()
                .WithMessage("Supplier name is required")
                .MaximumLength(100)
                .WithMessage("Supplier name must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .Length(11)
                .WithMessage("Invalid phone number digits");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required")
                .MaximumLength(250)
                .WithMessage("Address should not exceed 250 characters");
        }
    }
}
