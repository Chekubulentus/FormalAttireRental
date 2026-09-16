using FluentValidation;

namespace RentalAttireBackend.Application.Suppliers.Queries.FilterSuppliers
{
    public class FilterSuppliersQueryValidator : AbstractValidator<FilterSuppliersQuery>
    {
        public FilterSuppliersQueryValidator()
        {
            RuleFor(x => x.CurrentPage)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid current page value.");

            RuleFor(x => x.ItemsPerPage)
                .GreaterThanOrEqualTo(10)
                .WithMessage("Items per page should be greater than ten.");
        }
    }
}
