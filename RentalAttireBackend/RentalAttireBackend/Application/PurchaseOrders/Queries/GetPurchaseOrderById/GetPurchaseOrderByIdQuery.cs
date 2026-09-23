using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;

namespace RentalAttireBackend.Application.PurchaseOrders.Queries.GetPurchaseOrderById
{
    public class GetPurchaseOrderByIdQuery : IRequest<Result<PurchaseOrderDTO>>
    {
        public int PurchaseOrderId { get; set; }
    }
}
