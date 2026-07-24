using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly FormalAttireContext _context;

        public RentalRepository(FormalAttireContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRentalAsync(Rental newRental, CancellationToken ct)
        {
            await _context.AddAsync(newRental);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<Rental>> FilterRentalItemsAsync(string? categoryType, string? searchQuery, DateTime? startingDate, DateTime? endingDate, int currentPage, int itemsPerPage, CancellationToken ct)
        {
            var categoryTypeValidator = string.IsNullOrEmpty(categoryType);
            var searchQueryValidator = string.IsNullOrEmpty(searchQuery);

            var rentals = _context.Rentals
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Person)
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Clothe)
                        .ThenInclude(cl => cl.Category)
                .OrderByDescending(r => r.RentalDate)
                .Where(r =>
                    (
                        searchQueryValidator ||
                        r.Customer.User.Person.FullName.ToLower().Contains(searchQuery.ToLower())
                    ) &&
                    (
                        categoryTypeValidator ||
                        r.RentalItems.Any(ri => ri.Clothe.Category.CategoryName.ToLower().Contains(categoryType.ToLower()))
                    ) &&
                    (
                        !startingDate.HasValue || r.RentalDate >= startingDate
                    ) &&
                    (
                        !endingDate.HasValue || r.RentalDate <= endingDate
                    ) &&
                    r.IsActive
                );

            var totalCount = await rentals.CountAsync();

            var paginatedRentals = await rentals
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(ct);

            return new PagedResult<Rental>
            {
                Items = paginatedRentals,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage,
            };
        }

        public async Task<List<Rental>> GetAllRentalsAsync(CancellationToken cancellationToken)
        {
            return await _context.Rentals
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Clothe)
                .OrderByDescending(r => r.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
