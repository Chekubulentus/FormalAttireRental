using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface ISupplierRepository
    {
        #region Commands
        public Task<bool> UpdateSupplieAsync(
            Supplier supplier,
            CancellationToken cancellationToken
            );

        public Task<bool> CreateSupplierAsync(
            Supplier supplier,
            CancellationToken cancellationToken
            );
        #endregion

        #region Queries
        public Task<PagedResult<Supplier>> FilterSuppliersAsync(
            string searchQuery,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken
            );

        public Task<Supplier?> GetSupplierByIdAsync(
            int id,
            CancellationToken cancellationToken
            );

        public Task<Supplier?> GetSupplierByIdAsyncNoTracking(
            int id,
            CancellationToken cancellationToken
            );

        public Task<PagedResult<Clothe>> GetSupplierClothesByIdAsync(
            int id,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken
            );
        #endregion
    }
}
