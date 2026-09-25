using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.EditPurchaseOrder
{
    public class EditPurchaseOrderCommand : IRequest<Result<bool>>
    {
        public int PurchaseOrderId { get; set; }
        public List<LineItem> LineItems { get; set; } = new();
        public DateTime ExpectedDeliveryDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}
