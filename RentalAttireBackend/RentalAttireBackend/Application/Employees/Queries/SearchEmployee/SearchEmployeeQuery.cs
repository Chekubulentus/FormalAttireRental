using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Employees.Queries.SearchEmployee
{
    public class SearchEmployeeQuery : IRequest<Result<PagedResult<Employee>>>
    {
        public string SearchQuery { get; set; } = string.Empty;
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
