using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<Result<PagedResult<CategoryDTO>>>
    {
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
