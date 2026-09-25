using FluentValidation;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.EditPurchaseOrder
{
    public class EditPurchaseOrderCommandValidator : AbstractValidator<EditPurchaseOrderCommand>
    {
        public EditPurchaseOrderCommandValidator()
        {
            RuleFor(x => x.PurchaseOrderId)
                .NotEmpty()
                .WithMessage("Purchase order identifier is required")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid purchase order identifier value");

            RuleFor(x => x.LineItems)
                .NotEmpty()
                .WithMessage("At least one item is required to be selected.");

            RuleForEach(x => x.LineItems).ChildRules(item =>
            {
                item.RuleFor(i => i.ClotheId).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0);
            });

            var allowedStatuses = new List<OrderStatus>
            {
                OrderStatus.Draft,
                OrderStatus.Ordered
            };

            RuleFor(x => x.OrderStatus)
                .NotEmpty()
                .WithMessage("Order status is required")
                .Must(x => allowedStatuses.Contains(Enum.Parse<OrderStatus>(x, true)));

            RuleFor(x => x.ExpectedDeliveryDate)
                .NotEmpty()
                .WithMessage("Select a expected delivery date")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("You cannot select a past date for expected delivery date");
        }
    }
}
