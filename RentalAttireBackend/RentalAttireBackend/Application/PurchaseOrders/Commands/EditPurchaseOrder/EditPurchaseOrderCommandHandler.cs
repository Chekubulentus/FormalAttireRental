using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.EditPurchaseOrder
{
    public class EditPurchaseOrderCommandHandler : IRequestHandler<EditPurchaseOrderCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(EditPurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
