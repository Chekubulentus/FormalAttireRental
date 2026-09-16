using FluentValidation;

namespace RentalAttireBackend.Application.Rentals.Queries.FilterRentals
{
    public class FilterRentalsQueryValidator : AbstractValidator<FilterRentalsQuery>
    {
        public FilterRentalsQueryValidator()
        {
            RuleFor(x => x.CurrentPage)
                .GreaterThan(0)
                .WithMessage("Current page must be greater than zero.");

            RuleFor(x => x.ItemsPerPage)
                .GreaterThanOrEqualTo(10)
                .WithMessage("Items per page must be greater than or equal to 10.");

            RuleFor(x => x.StartingDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Starting date ");

            RuleFor(x => x.EndingDate)
                .GreaterThanOrEqualTo(x => x.StartingDate)
                .When(x => x.StartingDate.HasValue && x.EndingDate.HasValue)
                .WithMessage("Ending date cannot be earlier than the starting date.");
        }
    }
}
