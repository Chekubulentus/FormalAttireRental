using FluentValidation;

namespace RentalAttireBackend.Application.PurchaseOrders.Queries.GetPurchaseOrderById
{
    public class GetPurchaseORderByIdQueryValidator : AbstractValidator<GetPurchaseOrderByIdQuery>
    {
        public GetPurchaseORderByIdQueryValidator()
        {
            RuleFor(x => x.PurchaseOrderId)
                .NotEmpty()
                .WithMessage("Purchase order identifier not found")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Invalid purchase order identifier");
        }
    }
}
