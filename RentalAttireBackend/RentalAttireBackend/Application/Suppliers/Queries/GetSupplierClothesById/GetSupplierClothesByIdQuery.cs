using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierClothesById
{
    public class GetSupplierClothesByIdQuery : IRequest<Result<PagedResult<ClotheDTO>>>
    {
        public int Id { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
