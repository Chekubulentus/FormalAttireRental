using FluentValidation;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierById
{
    public class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
    {
        public GetSupplierByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid supplier identifier");
        }
    }
}
