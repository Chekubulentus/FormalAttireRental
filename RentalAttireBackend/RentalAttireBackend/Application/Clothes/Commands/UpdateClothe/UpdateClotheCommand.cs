using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Clothes.Commands.UpdateClothe
{
    public class UpdateClotheCommand : ClotheDTO, IRequest<Result<bool>>
    {
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
