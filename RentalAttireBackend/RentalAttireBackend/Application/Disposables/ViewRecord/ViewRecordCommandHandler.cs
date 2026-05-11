using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;

namespace RentalAttireBackend.Application.Disposables.ViewRecord
{
    public class ViewRecordCommandHandler : IRequestHandler<ViewRecordCommand, Result<ViewRecordResponse>>
    {
        private readonly Dictionary<string, IViewArchivedEntity> _viwers;

        public ViewRecordCommandHandler(IEnumerable<IViewArchivedEntity> viwers)
        {
            _viwers = viwers.ToDictionary(v => v.EntityType, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Result<ViewRecordResponse>> Handle(ViewRecordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.EntityType) ||
                request.Id == 0)
                return Result<ViewRecordResponse>.Failure("Invalid request. Please try again.");

            if (!_viwers.TryGetValue(request.EntityType, out var recordViwer))
                return Result<ViewRecordResponse>.Failure($"No viewer handler registered for type {request.EntityType}.");

            return await recordViwer.GetArchivedRecordAsync(request.Id, request.EntityType, cancellationToken);
        }
    }
}
