using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
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
                    (
                       searchQueryValidator ||
                       s.SupplierName.ToLower().Contains(searchQuery.ToLower()) ||
                       s.PhoneNumber.ToLower().Contains(searchQuery.ToLower()) ||
                       s.SupplierCode.ToLower().Contains(searchQuery.ToLower())
                    ) &&
                    s.IsActive && s.IsDeleted == false
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

        public async Task<List<Supplier>> GetAllActiveSuppliersASync(CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsActive == true && s.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Supplier>> GetAllArchivedSuppliersAsync(CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .Where(s => s.IsActive == false && s.IsDeleted == false)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Clothe>> GetAllSupplierClothesByIdAsync(int supplierId, CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .Include(c => c.Category)
                .Where(c => c.SupplierId == supplierId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<int?, int>> GetAllSuppliersAssignedClothesCount(List<int> supplierIds, CancellationToken cancellationToken)
        {
            return await _context.Clothes
                .AsNoTracking()
                .Where(c => supplierIds.Contains(c.SupplierId ?? 0))
                .GroupBy(g => g.SupplierId)
                .Select(g => new { Id = g.Key, AssignedClothesCount = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.AssignedClothesCount, cancellationToken);
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

        public async Task<PagedResult<Clothe>> GetSupplierClothesByIdAsync(
            int id, 
            string searchQuery,
            string category,
            string availability, 
            string gender, 
            int currentPage, 
            int itemsPerPage, 
            CancellationToken cancellationToken)
        {
            var genderEnumValue = string.IsNullOrEmpty(gender)
                ? ClotheGender.Unisex
                : Enum.Parse<ClotheGender>(gender, true);

            var baseQuery = _context.Clothes
                .Include(c => c.Category)
                .OrderBy(c => c.ClotheCode)
                .AsNoTracking()
                .Where(c =>
                    (
                    string.IsNullOrEmpty(searchQuery) ||
                    c.ClotheCode.ToLower().Contains(searchQuery.ToLower()) ||
                    c.ClotheName.ToLower().Contains(searchQuery.ToLower())
                    ) &&
                    (
                    string.IsNullOrEmpty(category) ||
                    c.Category.CategoryName.ToLower().Contains(category.ToLower())
                    ) &&
                    (
                    string.IsNullOrEmpty(gender) ||
                    c.Gender.Equals(genderEnumValue)
                    ) &&
                    (
                    string.IsNullOrEmpty(availability) ||
                    (availability.Equals("Available") && c.AvailableQuantity > 0) ||
                    (availability.Equals("Out of stock") && c.AvailableQuantity  == 0)
                    ) &&
                    c.SupplierId == id &&
                    c.IsActive
                );

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
                PageSize = itemsPerPage,
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

        public async Task<Supplier?> GetSupplierWithNoRelationshipsByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateSupplieAsync(Supplier supplier, CancellationToken cancellationToken)
        {
            _context.Suppliers.Update(supplier);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
