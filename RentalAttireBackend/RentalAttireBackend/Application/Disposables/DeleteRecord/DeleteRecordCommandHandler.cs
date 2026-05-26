using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Disposables.DeleteRecord
{
    public class DeleteRecordCommandHandler : IRequestHandler<DeleteRecordCommand, Result<bool>>
    {
        private Dictionary<string, IDeleteArchivedEntity> _deleters;

        public DeleteRecordCommandHandler(IEnumerable<IDeleteArchivedEntity> deleters)
        {
            _deleters = deleters.ToDictionary(d => d.EntityType, StringComparer.OrdinalIgnoreCase);
        }
        public async Task<Result<bool>> Handle(DeleteRecordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.EntityType))
                return Result<bool>.Failure("Invalid request. Please try again.");

            if (request.Id == 0)
                return Result<bool>.Failure("Invalid record identifier. Please try again.");

            if (!_deleters.TryGetValue(request.EntityType, out var recordDeleter))
                return Result<bool>.Failure($"No deleter handler for type {request.EntityType}");

            return await recordDeleter.DeleteArchivedRecordAsync(
                request.Id, 
                request.PerformedBy, 
                request.PerformedById, 
                cancellationToken);
        }
    }
}
