using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        public Task<bool> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken);
        public Task<bool> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken);
        public Task<PagedResult<PurchaseOrder>> FilterPurchaseOrdersAsync(
            string? searchQuery,
            List<string> statuses,
            string? dateTypeToggle,
            DateTime? startingDate,
            DateTime? endingdate
            );
        public Task<PurchaseOrder?> GetPurchaeOrderByIdNoTrackingAsync(int id, CancellationToken cancellationToken);
    }
}
