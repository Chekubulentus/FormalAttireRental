using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Clothes.Commands.CreateClothe
{
    public class CreateClotheCommand : ClotheDTO, IRequest<Result<bool>>
    {
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; } 
        public IFormFile? Image { get; set; }
    }
}
