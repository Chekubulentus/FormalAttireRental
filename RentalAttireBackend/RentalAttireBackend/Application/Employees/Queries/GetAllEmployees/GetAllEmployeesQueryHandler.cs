using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<PagedResult<EmployeeDTO>>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IMapper _mapper;

        public GetAllEmployeesQueryHandler
            (
            IEmployeeRepository employeeRepo,
            IMapper mapper
            )
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<EmployeeDTO>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PagedResult<EmployeeDTO>>.Failure("Invalid request. Please try again.");
            try
            {
                var paginatedEmployees = await _employeeRepo
                    .GetAllEmployeesAsync(request.PaginationParams, cancellationToken);

                if (!paginatedEmployees.Items.Any() || paginatedEmployees.Items.Count() == 0)
                    return Result<PagedResult<EmployeeDTO>>.Failure("No employees currently registered.");

                var paginatedEmployeesDto = _mapper.Map<PagedResult<EmployeeDTO>>(paginatedEmployees);

                return Result<PagedResult<EmployeeDTO>>.Success(paginatedEmployeesDto);
            }catch(Exception e)
            {
                return Result<PagedResult<EmployeeDTO>>.Failure(e.Message);
            }
        }
    }
}
