using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface  IRentalRepository
    {
        #region Commands
        public Task<bool> CreateRentalAsync(Rental newRental, CancellationToken ct);
        #endregion

        #region Queries
        public Task<PagedResult<Rental>> FilterRentalItemsAsync(
            string? categoryType,
            string? searchQuery,
            DateTime? startingDate,
            DateTime? endingDate,
            int currentPage,
            int itemsPerPage,
            CancellationToken ct
            );

        public Task<List<Rental>> GetAllRentalsAsync(
            CancellationToken cancellationToken
            );
        #endregion
    }
}
