using FluentValidation;

namespace RentalAttireBackend.Application.Rentals.Commands.RentalTransaction
{
    public class RentalTransactionCommandValidator : AbstractValidator<RentalTransactionCommand>
    {
        public RentalTransactionCommandValidator()
        {
            RuleFor(x => x.RentalItems)
            .NotEmpty()
            .WithMessage("No items currently selected.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Please choose a payment method.");

            RuleFor(x => x.GcashRefNum)
                .NotEmpty()
                .WithMessage("Please proceed first with payment.");

            RuleFor(x => x.GcashRefName)
                .NotEmpty()
                .WithMessage("Please proceed first with payment.");

            RuleFor(x => x.PickupDate)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Pickup date cannot be in the past.");

            RuleFor(x => x)
                .Must(x => x.ReturnDate.Date > x.PickupDate.Date)
                .WithMessage("Return date must be after pickup date.");
        }
    }
}
