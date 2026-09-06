using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Suppliers.Queries.FilterSuppliers
{
    public class FilterSuppliersQuery : IRequest<Result<PagedResult<SupplierDTO>>>
    {
        public string SearchQuery { get; set; } = string.Empty;
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
