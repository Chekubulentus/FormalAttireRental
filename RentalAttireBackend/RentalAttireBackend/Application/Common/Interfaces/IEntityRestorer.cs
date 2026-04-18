using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IEntityRestorer
    {
        string EntityType { get; } 
        public Task<Result<bool>> RestoreAsync(int entityId, CancellationToken ct);
    }
}
