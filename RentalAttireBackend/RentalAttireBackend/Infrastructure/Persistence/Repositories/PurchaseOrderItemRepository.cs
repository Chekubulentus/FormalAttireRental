using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class PurchaseOrderItemRepository : IPurchaseOrderItemRepository
    {
        private readonly FormalAttireContext _context;

        public PurchaseOrderItemRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> CreatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem, CancellationToken cancellationToken)
        {
            await _context.PurchaseOrderItems.AddAsync(purchaseOrderItem, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<PagedResult<PurchaseOrderItem>> GetAllPurchaseOrderItemsByPOIdAsync(
            int purchaseOrderId,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken)
        {
            var baseQuery = _context
                .PurchaseOrderItems
                .AsNoTracking()
                .OrderByDescending(i => i.Id)
                .Where(i => i.PurchaseOrderId == purchaseOrderId);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var paginatedItems = await baseQuery
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<PurchaseOrderItem>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage
            };
        }

        public async Task<PurchaseOrderItem?> GetPurchaseOrderItemByIdAsync(int purchaseOrderItemId, CancellationToken cancellationToken)
        {
            return await _context.PurchaseOrderItems
                .Include(poi => poi.Clothe)
                .FirstOrDefaultAsync(poi => poi.Id == purchaseOrderItemId, cancellationToken);
        }

        public async Task<bool> UpdatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem, CancellationToken cancellationToken)
        {
            _context.PurchaseOrderItems.Update(purchaseOrderItem);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
