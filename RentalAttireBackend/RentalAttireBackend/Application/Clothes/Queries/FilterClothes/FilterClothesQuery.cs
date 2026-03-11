using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Clothes.Queries.FilterClothes
{
    public class FilterClothesQuery : IRequest<Result<PagedResult<ClotheDTO>>>
    {
        public ClothesFIlterParameters FilterParameters { get; set; } = new();
    }
}
