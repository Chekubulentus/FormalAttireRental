using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : CategoryDTO, IRequest<Result<bool>>
    {
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
