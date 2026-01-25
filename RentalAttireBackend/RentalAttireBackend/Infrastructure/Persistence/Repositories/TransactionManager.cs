using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class TransactionManager : ITransactionManager
    {
        private readonly FormalAttireContext _context;

        public TransactionManager(FormalAttireContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransacionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
