using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

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

        public Task<Dictionary<int, int>> GetSupplierActivePOCountByIdAsync(
            List<int> supplierIds,
            CancellationToken cancellationToken
            );

        public Task<Dictionary<int, int>> GetSupplierOverduePOCountByIdAsync(
            List<int> supplierIds,
            CancellationToken cancellationToken
            );

        public Task<List<Clothe>> FilterAssignableClothesAsync(
            int? supplierId,
            string searchQuery,
            string category,
            string gender,
            int currentPage,
            int itemsPerPage,
            CancellationToken cancellationToken
            );

        public Task<int> GetSupplierAssignClothesCount(
            int? supplierId,
            CancellationToken cancellationToken
            );

        public Task<int> GetAllUnassignedClothesAsync(CancellationToken cancellationToken);
        #endregion
    }
}
