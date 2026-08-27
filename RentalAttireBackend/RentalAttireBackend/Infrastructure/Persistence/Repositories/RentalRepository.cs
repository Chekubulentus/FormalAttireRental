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

        public async Task<PagedResult<Rental>> FilterRentalItemsAsync(
            string? status, 
            string? searchQuery,
            DateTime? startingDate, 
            DateTime? endingDate, 
            int currentPage, 
            int itemsPerPage, 
            CancellationToken ct)
        {
            var statusValidator = string.IsNullOrEmpty(status);
            var searchQueryValidator = string.IsNullOrEmpty(searchQuery);

            var startingDateUtc = startingDate.HasValue
                ? DateTime.SpecifyKind(startingDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            var endingDateUtc = endingDate.HasValue
                ? DateTime.SpecifyKind(endingDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            var rentals = _context.Rentals
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Person)
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Clothe)
                        .ThenInclude(c => c.Category)
                .OrderByDescending(r => r.Id)
                .Where(r =>
                    (
                    searchQueryValidator || 
                    r.Customer.User.Person.LastName.ToLower().Contains(searchQuery.ToLower()) ||
                    r.Customer.User.Person.FirstName.ToLower().Contains(searchQuery.ToLower())
                    ) 
                    &&
                    (
                    statusValidator || r.Status.ToLower().Equals(status.ToLower())
                    )
                    &&
                    (
                    !startingDate.HasValue || r.RentalDate >= startingDateUtc
                    )
                    &&
                    (
                    !endingDate.HasValue || r.RentalDate <= endingDateUtc
                    )
                    &&
                    r.IsActive
                );

            var totalCount = await rentals.CountAsync(ct);

            var paginatedRentals = await rentals
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(ct);

            return new PagedResult<Rental>
            {
                Items = paginatedRentals,
                TotalCount = totalCount,
                PageNumber = currentPage,
                PageSize = itemsPerPage
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

        public async Task<double> GetAllRentalsTotalRevenue(CancellationToken cancellationToken)
        {
            return await _context.Rentals
                .Where(r => AllowedStatusConditions.RevenueStatuses.Contains(r.Status))
                .SumAsync(r => r.TotalAmount);
        }

        public async Task<RentalAnalytics> GetRentalAnalyticsAsync(CancellationToken cancellationToken)
        {
            var dateToday = DateTime.UtcNow.Date;
            var dueSoonThreshold = DateTime.UtcNow.Date.AddDays(7);

            var statusCounts = await _context.Rentals
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var overdueCount = await _context.Rentals
                .Where(r =>
                    r.IsActive &&
                    r.ReturnDate != null &&
                    (r.Status == "Confirmed" || r.Status == "Ready for pickup") &&
                    r.ReturnDate < dateToday
                ).CountAsync(cancellationToken);

            var dueSoonCount = await _context.Rentals
                .Where(r =>
                    r.IsActive &&
                    r.ReturnDate != null &&
                    (r.Status == "Confirmed" || r.Status == "Ready for pickup") &&
                    r.ReturnDate >= dateToday &&
                    r.ReturnDate <= dueSoonThreshold
                ).CountAsync(cancellationToken);

            return new RentalAnalytics
            {
                StatusCounts = statusCounts.ToDictionary(x => x.Status, x => x.Count),
                OverdueCount = overdueCount,
                DueSoonCount = dueSoonCount
            };
        }

        public async Task<Rental?> GetRentalByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Rentals
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Clothe)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateRentalAsync(Rental rental, CancellationToken cancellationToken)
        {
            _context.Rentals.Update(rental);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
