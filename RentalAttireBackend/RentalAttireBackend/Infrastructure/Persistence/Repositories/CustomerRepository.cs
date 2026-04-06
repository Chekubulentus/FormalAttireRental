using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.CodeDom;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly FormalAttireContext _context;

        public CustomerRepository(
            FormalAttireContext context
            )
        {
            _context = context;
        }

        public async Task<bool> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _context.Customers.AddAsync(customer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<Customer>> FilterCustomersAsync(PaginationParams paginationParams, string? searchQuery, CancellationToken cancellationToken)
        {
            var customers = _context.Customers
                .AsNoTracking()
                .Include(c => c.User)
                .ThenInclude(u => u.Person)
                // Order by mapped properties instead of the unmapped FullName computed property to allow server-side translation
                .OrderBy(c => c.User.Person.LastName)
                .ThenBy(c => c.User.Person.FirstName)
                .AsQueryable();

            var totalCount = await customers.CountAsync(cancellationToken);

            var paginatedItems = await customers
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PagedResult<Customer>
            {
                Items = paginatedItems,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .Include(c => c.User)
                .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
