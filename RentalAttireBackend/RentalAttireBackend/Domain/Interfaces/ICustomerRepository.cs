using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface ICustomerRepository
    {

        #region Comamnds
        public Task<bool> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken);
        public Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken);
        #endregion

        #region Queries
        public Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken);
        public Task<Customer?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken);
        public Task<PagedResult<Customer>> FilterCustomersAsync(
            PaginationParams paginationParams,
            string searchQuery,
            CancellationToken cancellationToken
            );
        #endregion
    }
}
