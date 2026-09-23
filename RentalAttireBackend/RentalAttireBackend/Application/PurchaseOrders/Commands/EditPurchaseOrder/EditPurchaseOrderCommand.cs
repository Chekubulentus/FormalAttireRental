using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.EditPurchaseOrder
{
    public class EditPurchaseOrderCommand : IRequest<Result<bool>>
    {
        public int PurchaseOrderId { get; set; }
    }
}
