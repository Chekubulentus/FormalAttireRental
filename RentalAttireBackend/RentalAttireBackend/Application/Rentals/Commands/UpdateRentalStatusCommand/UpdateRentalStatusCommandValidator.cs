using FluentValidation;
using Microsoft.Extensions.ObjectPool;

namespace RentalAttireBackend.Application.Rentals.Commands.UpdateRentalStatusCommand
{
    public class UpdateRentalStatusCommandValidator : AbstractValidator<UpdateRentalStatusCommmand>
    {
        private readonly string[] validStatuses = ["Pending", "Confirmed", "Declined", "Pickup", "Returned", "Ready for pickup", "Returned"];
        public UpdateRentalStatusCommandValidator()
        {
            RuleFor(x => x.RentalId)
                .GreaterThan(0)
                .WithMessage("Rental identifier should be greater than 0");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status field is required.")
                .Must(x => validStatuses.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid rental status value.");
        }
    }
}
