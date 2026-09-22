using FluentValidation;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public class CreatePurchaseOrderComanndValidator : AbstractValidator<CreatePurchaseOrderCommand>
    {
        public CreatePurchaseOrderComanndValidator()
        {
            RuleFor(x => x.LineItems)
                .NotEmpty()
                .WithMessage("At least one item is required to be selected.");

            RuleForEach(x => x.LineItems).ChildRules(item =>
            {
                item.RuleFor(i => i.ClotheId).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0);
            });

            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("Supplier identifier is empty")
                .GreaterThan(0)
                .WithMessage("Invalid supplier identifier value");

            RuleFor(x => x.ExpectedDeliveryDate)
                .NotEmpty()
                .WithMessage("Select a expected delivery date")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("You cannot select a past date for expected delivery date");

            var allowedStatuses = new List<OrderStatus>() {
                OrderStatus.Draft,
                OrderStatus.Ordered
             }.Select(x => x.ToString()).ToList();

            RuleFor(x => x.OrderStatus)
                .NotEmpty()
                .WithMessage("Order status is empty")
                .Must(x => allowedStatuses.Contains(x));
        }
    }
}
