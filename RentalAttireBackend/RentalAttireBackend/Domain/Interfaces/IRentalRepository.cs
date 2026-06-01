using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface  IRentalRepository
    {
        #region Commands

        #endregion

        #region Queries
        public Task<RentalPageResponse> FilterRentalItemsAsync(
            RentalPageRequest request,
            CancellationToken ct
            );
        #endregion
    }
}
