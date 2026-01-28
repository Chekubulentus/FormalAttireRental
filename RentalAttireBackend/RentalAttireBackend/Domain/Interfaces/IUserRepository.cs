using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IUserRepository
    {
        #region Commands
        public Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken);
        public Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
        #endregion

        #region Queries
        public Task<PagedResult<User>> GetAllUsersAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
        public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        public Task<bool> ValidateEmailDuplicationAsync(string email, CancellationToken cancellationToken);
        #endregion
    }
}
