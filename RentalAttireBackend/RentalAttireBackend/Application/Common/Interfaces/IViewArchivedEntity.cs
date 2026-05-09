using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IViewArchivedEntity
    {
        string EntityType { get; }
        public Task<Result<ViewRecordResponse>> GetArchivedRecordAsync(
            int id,
            string entityType,
            CancellationToken ct
            );
    }
}
