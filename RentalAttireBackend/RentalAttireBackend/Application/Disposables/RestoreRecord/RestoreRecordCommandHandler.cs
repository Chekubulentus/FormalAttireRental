using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Disposables.RestoreRecord
{
    public class RestoreRecordCommandHandler : IRequestHandler<RestoreRecordCommand, Result<bool>>
    {
        private readonly Dictionary<string, IEntityRestorer> _restorers;

        public RestoreRecordCommandHandler(List<IEntityRestorer> restorers)
        {
            _restorers = restorers.ToDictionary(r => r.EntityType, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Result<bool>> Handle(RestoreRecordCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return Result<bool>.Failure("Invalid request. Please try again.");

            if (!_restorers.TryGetValue(request.EntityType, out var restorer))
                return Result<bool>.Failure($"No restorer registered for this record type {request.EntityType}");

            return await restorer.RestoreAsync(request.Id, cancellationToken);
        }
    }
}
