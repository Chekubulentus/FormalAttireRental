using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;

namespace RentalAttireBackend.Application.Customers.Queries.FilterCustomers
{
    public class FilterCustomersQuery : IRequest<Result<PagedResult<CustomerDTO>>>
    {
        public string SearchQuery { get; set; } = string.Empty;
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
