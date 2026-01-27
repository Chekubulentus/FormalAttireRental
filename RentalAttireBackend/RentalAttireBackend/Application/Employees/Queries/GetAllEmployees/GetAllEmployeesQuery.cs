using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<Result<PagedResult<EmployeeDTO>>>
    {
        public PaginationParams PaginationParams { get; set; } = null!;
    }
}
