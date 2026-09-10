using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Transactions;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly FormalAttireContext _context;

        public SupplierRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken)
        {
            await _context.Suppliers.AddAsync(supplier, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<List<Clothe>> FilterAssignableClothesAsync(
            int? supplierId, 
            string searchQuery, 
            string category, 
            string gender, 
            int currentPage, 
            int itemsPerPage,
            CancellationToken cancellationToken
            )
        {
            var searchQueryValidator = string.IsNullOrEmpty(searchQuery);
            var categoryValidator = string.IsNullOrEmpty(category);
            var genderValidator = string.IsNullOrEmpty(gender);
            ClotheGender? clotheGenderEnum = string.IsNullOrEmpty(gender)
                ? null
                : Enum.Parse<ClotheGender>(gender, true);

            var baseQuery = _context.Clothes
                .OrderBy(c => c.ClotheCode)
                .Where(c =>
                    (
                        c.SupplierId == supplierId ||
                        c.SupplierId == null
                    ) &&
                    (
                        searchQueryValidator ||
                        c.ClotheCode.ToLower().Contains(searchQuery.ToLower()) ||
                        c.ClotheName.ToLower().Contains(searchQuery.ToLower())
                    ) &&
                    (
                    categoryValidator ||
                    c.Category.CategoryName.ToLower().Equals(category.ToLower())
                    ) &&
                    (
                    genderValidator ||
                    c.Gender == clotheGenderEnum
                    )
                );

            var paginatedClothes = await baseQuery
                .Skip((currentPage -1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return paginatedClothes;
        }

        public async Task<PagedResult<Supplier>> FilterSuppliersAsync(string searchQuery, 
            int currentPage, 
            int itemsPerPage, 
            CancellationToken cancellationToken)
        {
            var searchQueryValidator = string.IsNullOrEmpty(searchQuery);

            var baseQuery = _context.Suppliers
                .Include(s => s.ClothesAvailable)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.Person)
                .OrderByDescending(s => s.Id)
                .Where(s =>
                searchQueryValidator ||
                s.SupplierName.ToLower().Contains(searchQuery.ToLower()) ||
                s.PhoneNumber.ToLower().Contains(searchQuery.ToLower()) ||
                s.SupplierCode.ToLower().Contains(searchQuery.ToLower())
                );

            var totalCount = await baseQuery.CountAsync();

            var paginatedItems = await baseQuery
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Supplier>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage,
            };
        }

        public async Task<int> GetAllUnassignedClothesAsync(CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .Where(c => c.SupplierId == null)
                .CountAsync(cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetSupplierActivePOCountByIdAsync(List<int> supplierIds, CancellationToken cancellationToken)
        {
            var allowedStatuses = new List<OrderStatus> { 
                OrderStatus.Draft, 
                OrderStatus.Ordered, 
                OrderStatus.PartiallyReceived
            };

            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(p =>
                    supplierIds.Contains(p.SupplierId) &&
                    allowedStatuses.Contains(p.OrderStatus)
                ).GroupBy(g => g.SupplierId)
                .Select(g => new { SupplierId = g.Key, Count = g.Count()})
                .ToDictionaryAsync(x => x.SupplierId, x => x.Count);
        }

        public async Task<int> GetSupplierAssignClothesCount(int? supplierId, CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .Where(c => c.SupplierId == supplierId)
                .CountAsync(cancellationToken);
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .Include(s => s.ClothesAvailable)
                    .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Supplier?> GetSupplierByIdAsyncNoTracking(int id, CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.ClothesAvailable)
                    .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<PagedResult<Clothe>> GetSupplierClothesByIdAsync(int id, int currentPage, int itemsPerPage, CancellationToken cancellationToken)
        {
            var baseQuery = _context.Clothes
                .Where(c => c.SupplierId == id)
                .OrderBy(c => c.Id);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var paginatedClothes = await baseQuery
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Clothe>
            {
                Items = paginatedClothes,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage
            };
        }

        public async Task<Dictionary<int, int>> GetSupplierOverduePOCountByIdAsync(List<int> supplierIds, CancellationToken cancellationToken)
        {
            var allowedStatuses = new List<OrderStatus> {
                OrderStatus.Draft,
                OrderStatus.Ordered,
                OrderStatus.PartiallyReceived
            };

            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(p =>
                    supplierIds.Contains(p.SupplierId) &&
                    allowedStatuses.Contains(p.OrderStatus) &&
                    p.ExpectedDeliveryDate.Date < DateTime.UtcNow.Date
                ).GroupBy(g => g.SupplierId)
                .Select(g => new { SupplierId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SupplierId, x => x.Count, cancellationToken);
        }

        public async Task<bool> UpdateSupplieAsync(Supplier supplier, CancellationToken cancellationToken)
        {
            _context.Suppliers.Update(supplier);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
