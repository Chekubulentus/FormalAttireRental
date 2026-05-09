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

        }
    }
}
