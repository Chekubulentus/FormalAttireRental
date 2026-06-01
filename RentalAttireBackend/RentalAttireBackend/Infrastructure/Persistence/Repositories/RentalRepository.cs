using Microsoft.EntityFrameworkCore;
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

        public async Task<RentalPageResponse> FilterRentalItemsAsync(RentalPageRequest request, CancellationToken ct)
        {
            var categoryTypeValidator = string.IsNullOrEmpty(request.CategoryType);
            var searchQueryValidation = string.IsNullOrEmpty(request.SearchQuery);

            var rentalItems = _context.Rentals
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Person)
                .Include(r => r.RentalItems)
                    .ThenInclude(ri => ri.Clothe)
                        .ThenInclude(c => c.Category)
                .Where(r =>
                    (
                    categoryTypeValidator ||
                    r.RentalItems.Any(ri => ri.Clothe.Category.CategoryName.ToLower().Contains(request.CategoryType.ToLower()))
                    ) &&
                    (
                    searchQueryValidation ||
                    r.Customer.User.Person.FullName.ToLower().Contains(request.SearchQuery.ToLower())
                    ) &&
                    (
                    !request.StartingDate.HasValue || r.RentalDate >= request.StartingDate
                    )
                    &&
                    (
                    !request.EndingDate.HasValue || r.RentalDate <= request.EndingDate
                    )
                );

            var totalCount = await rentalItems.CountAsync();

            var paginatedRentalItems = await rentalItems
                .Skip((request.CurrentPage - 1) * request.ItemsPerPage)
                .Take(request.ItemsPerPage)
                .ToListAsync(ct);

            return new RentalPageResponse
            {
                Items = paginatedRentalItems,
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage,
                TotalCount = totalCount
            };
        }
    }
}
