using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly FormalAttireContext _context;

        public PurchaseOrderRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken)
        {
            await _context.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<PagedResult<PurchaseOrder>> FilterPurchaseOrdersAsync(
            string? searchQuery,
            List<string> statuses,
            string? dateTypeToggle,
            DateTime? startingDate,
            DateTime? endingdate)
        {
            var enumStatuses = statuses
                .Select(x => Enum.Parse<OrderStatus>(x))
                .ToList();

            var startingDateUtc = startingDate.HasValue
                ? DateTime.SpecifyKind(startingDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            var endingDateUtc = endingdate.HasValue
                ? DateTime.SpecifyKind(endingdate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            var allowedToggleTypes = new string[] { "OrderDate", "ExpectedDeliveryDate" };

            var query = _context.PurchaseOrders
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Where(p =>
                    (
                    string.IsNullOrEmpty(searchQuery) ||
                    p.PurchaseOrderCode.ToLower().Contains(searchQuery.ToLower())
                    ) &&
                    (
                    enumStatuses.Count() == 0 ||
                    enumStatuses.Contains(p.OrderStatus)
                    ) &&
                    (
                        (dateTypeToggle.ToLower().Trim() == "orderdate" &&

                        (!startingDateUtc.HasValue || 
                        p.OrderDate >= startingDateUtc) &&

                        (!endingDateUtc.HasValue || 
                        p.OrderDate <= endingDateUtc)
                        ) ||

                        (dateTypeToggle.ToLower().Trim() == "expecteddeliverydate" &&

                        (!startingDateUtc.HasValue ||
                        p.ExpectedDeliveryDate >= startingDateUtc) &&

                        (!endingDateUtc.HasValue || 
                        p.ExpectedDeliveryDate <= endingDateUtc)
                        )
                    )
                );


            throw new NotImplementedException();
        }

        public async Task<PurchaseOrder?> GetPurchaeOrderByIdNoTrackingAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Clothe)
                .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
        }

        public async Task<bool> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken)
        {
            _context.PurchaseOrders.Update(purchaseOrder);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
