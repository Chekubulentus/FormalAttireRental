using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public class CreatePurchaseOrderCommand : IRequest<Result<bool>>
    {
        public List<LineItem> LineItems { get; set; } = new();
        public int SupplierId { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}
