using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;

namespace RentalAttireBackend.Application.Employees.Queries.SearchEmployee
{
    public class SearchEmployeeQuery : IRequest<Result<PagedResult<EmployeeDTO>>>
    {
        public string SearchQuery { get; set; } = string.Empty;
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
