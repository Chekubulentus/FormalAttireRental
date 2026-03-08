using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryDTO>>
    {
        public int Id { get; set; }
    }
}
