namespace RentalAttireBackend.Domain.Interfaces
{
    public interface ITransactionManager
    {
        public Task BeginTransactionAsync(CancellationToken cancellationToken);
        public Task CommitTransacionAsync(CancellationToken cancellationToken);
        public Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }
}
