using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Migrations;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        #region Queries
        public Task<PagedResult<Category>> GetAllCategoriesAsync(
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            );

        public Task<Category?> GetCategoryByIdAsync(
            int id,
            CancellationToken cancellationToken
            );

        public Task<PagedResult<Category>> SearchCategoryByNameAsync(
            string searchQuery,
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            );

        public Task<Category?> GetCategoryByNameAsync(
            string categoryName,
            CancellationToken cancellationToken
            );

        public Task<PagedResult<Category>> FilterCategoriesAsync(
            string categoryCode,
            string categoryName,
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            );

        #endregion

        #region Commands
        public Task<bool> CreateCategoryAsync(
            Category category,
            CancellationToken cancellationToken
            );
          
        public Task<bool> UpdateCategoryAsync(
            Category category,
            CancellationToken cancellationToken
            );
        #endregion
    }
}
