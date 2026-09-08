using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierClothesById
{
    public class GetSupplierClothesByIdQuery : IRequest<Result<PagedResult<ClotheDTO>>>
    {
        public int Id { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
