using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : CategoryDTO, IRequest<Result<bool>>
    {
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
