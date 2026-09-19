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
            DateTime? endingdate,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken)
        {
            var enumStatuses = statuses
                .Where(x => !string.IsNullOrEmpty(x))
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
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Clothe)
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

            var totalCount = await query.CountAsync(cancellationToken);

            var paginatedItems = await query
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<PurchaseOrder>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage
            };
        }

        public async Task<Dictionary<int, string>> GetAllPurchaseOrderEmployeeNames(List<int> poIds, CancellationToken cancellationToken)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Employee)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.Person)
                .Where(po => poIds.Contains(po.Id))
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Employee.User.Person.FullName, cancellationToken);
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
