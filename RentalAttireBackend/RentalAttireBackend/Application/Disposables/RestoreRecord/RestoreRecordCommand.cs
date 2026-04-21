using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Disposables.RestoreRecord
{
    public class RestoreRecordCommand : IRequest<Result<bool>>
    {
        public string EntityType { get; set; } = string.Empty;
        public int Id { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }

    }
}
