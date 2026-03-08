using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Clothes.Queries.GetAllClothes
{
    public class GetAllClothesQuery : IRequest<Result<PagedResult<ClotheDTO>>>
    {
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
