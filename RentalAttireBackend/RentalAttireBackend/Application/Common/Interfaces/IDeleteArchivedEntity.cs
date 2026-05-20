using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IDeleteArchivedEntity
    {
        string EntityType { get; }
        public Task<Result<bool>> DeleteArchivedRecordAsync(
            int id,
            string performedBy,
            int performedById,
            CancellationToken ct
            );
    }
}
