using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IPurchaseOrderItemRepository
    {
        public Task<bool> CreatePurchaseOrderItemAsync(
            PurchaseOrderItem purchaseOrderItem, 
            CancellationToken cancellationToken);
        public Task<bool> UpdatePurchaseOrderItemAsync(
            PurchaseOrderItem purchaseOrderItem, 
            CancellationToken cancellationToken);
        public Task<PagedResult<PurchaseOrderItem>> GetAllPurchaseOrderItemsByPOIdAsync(
            int purchaseOrderId, 
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken);
        public Task<PurchaseOrderItem?> GetPurchaseOrderItemByIdAsync
            (int purchaseOrderItemId, 
            CancellationToken cancellationToken);
    }
}
