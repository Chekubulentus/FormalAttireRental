using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IClotheRepository
    {
        #region Queries
        public Task<PagedResult<Clothe>> GetAllClothesAsync(
            PaginationParams paginationParams,
            CancellationToken cancellationToken
            );

        public Task<Clothe?> GetClotheByIdAsync(
            int id, 
            CancellationToken cancellationToken
            );

        public Task<Clothe?> GetClotheByIdNoTrackingAsync(
            int id,
            CancellationToken cancellationToken
            );

        public Task<PagedResult<Clothe>> FilterClothesAsync(
            ClothesFIlterParameters filters,
            CancellationToken cancellationToken
            );

        public Task<List<Clothe>> GetAllArchivedClothesAsync(
            CancellationToken cancellationToken
            );
        #endregion

        #region Commands
        public Task<int> CreateClotheAsync(
            Clothe clothe, 
            CancellationToken cancellationToken
            );

        public Task<bool> UpdateClotheAsync(
            Clothe clothe,
            CancellationToken cancellationToken
            );
        #endregion
    }
}
