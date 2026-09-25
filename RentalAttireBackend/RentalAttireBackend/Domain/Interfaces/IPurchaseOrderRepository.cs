using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        public Task<bool> CreatePurchaseOrderAsync(
            PurchaseOrder purchaseOrder, 
            CancellationToken cancellationToken);
        public Task<bool> UpdatePurchaseOrderAsync(
            PurchaseOrder purchaseOrder, 
            CancellationToken cancellationToken);
        public Task<PagedResult<PurchaseOrder>> FilterPurchaseOrdersAsync(
            string? searchQuery,
            List<string> statuses,
            string? dateTypeToggle,
            DateTime? startingDate,
            DateTime? endingdate,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken
            );
        public Task<PurchaseOrder?> GetPurchaeOrderByIdNoTrackingAsync(
            int id, 
            CancellationToken cancellationToken);
        public Task<Dictionary<int, string>> GetAllPurchaseOrderEmployeeNames(
            List<int> poIds, 
            CancellationToken cancellationToken);
        public Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(
            int id, 
            CancellationToken cancellationToken);
        public Task<PurchaseOrder?> GetPurchaseOrderByIdNoTrackingAsync(
            int id,
            CancellationToken cancellationToken
            );
    }

}
