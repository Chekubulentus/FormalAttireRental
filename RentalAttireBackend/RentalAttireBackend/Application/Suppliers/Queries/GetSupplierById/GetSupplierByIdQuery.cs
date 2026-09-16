using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetSupplierById
{
    public class GetSupplierByIdQuery : IRequest<Result<SupplierDTO>>
    {
        public int Id { get; set; }
    }
}
