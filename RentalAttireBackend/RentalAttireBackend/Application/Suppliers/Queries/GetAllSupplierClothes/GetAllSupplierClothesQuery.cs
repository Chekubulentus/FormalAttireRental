using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetAllSupplierClothes
{
    public class GetAllSupplierClothesQuery : IRequest<Result<List<ClotheDTO>>>
    {
        public int SupplierId { get; set; }
    }
}
