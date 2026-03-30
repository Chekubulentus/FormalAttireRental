using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
