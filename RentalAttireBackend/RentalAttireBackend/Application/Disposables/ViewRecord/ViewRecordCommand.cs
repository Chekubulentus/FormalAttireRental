using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;

namespace RentalAttireBackend.Application.Disposables.ViewRecord
{
    public class ViewRecordCommand : IRequest<Result<ViewRecordResponse>>
    {
        public string EntityType { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
