using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FormalAttireContext _context;

        public UserRepository
            (
            FormalAttireContext context
            )
        {
            _context = context;            
        }

        public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<User>> GetAllUsersAsync
            (
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            )
        {
            var users = _context.Users
                .AsNoTracking()
                .AsQueryable();

            int totalCount = await users.CountAsync();

            var items = await users
                .Skip(paginationParams.Skip)
                .Take(paginationParams.ItemsPerPage)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.CurrentPage,
                PageSize = paginationParams.ItemsPerPage
            };
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
