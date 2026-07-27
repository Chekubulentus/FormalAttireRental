using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Rentals.Commands.UpdateRentalStatusCommand
{
    public class UpdateRentalStatusCommmand : IRequest<Result<bool>>
    {
        public int RentalId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
