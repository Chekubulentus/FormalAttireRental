using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;

namespace RentalAttireBackend.Application.Suppliers.Queries.GetAllSuppliers
{
    public class GetAllSuppliersQuery : IRequest<Result<List<SupplierSummaryDTO>>>
    {
    }
}
