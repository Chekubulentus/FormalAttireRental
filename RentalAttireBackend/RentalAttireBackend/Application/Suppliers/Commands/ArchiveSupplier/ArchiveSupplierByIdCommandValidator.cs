using FluentValidation;

namespace RentalAttireBackend.Application.Suppliers.Commands.ArchiveSupplier
{
    public class ArchiveSupplierByIdCommandValidator : AbstractValidator<ArchiveSupplierByIdCommand>
    {
        public ArchiveSupplierByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid supplier identifier");
        }
    }
}
