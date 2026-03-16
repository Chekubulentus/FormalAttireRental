using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace RentalAttireBackend.Application.Categories.Queries.FilterCategories
{
    public class FilterCategoriesQuery : IRequest<Result<PagedResult<CategoryDTO>>>
    {
        public string? CategoryCode { get; set; } 
        public string? CategoryName { get; set; }
        [Required]
        public PaginationParams PaginationParams { get; set; } = new();
    }
}
