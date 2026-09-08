using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

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

        public async Task<bool> UpdateSupplieAsync(Supplier supplier, CancellationToken cancellationToken)
        {
            _context.Suppliers.Update(supplier);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
