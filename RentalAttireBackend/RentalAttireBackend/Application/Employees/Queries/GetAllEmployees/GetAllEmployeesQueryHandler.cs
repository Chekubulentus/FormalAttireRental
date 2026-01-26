using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<List<EmployeeDTO>>>
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
        
        public async Task<Result<List<EmployeeDTO>>> Handle(GetAllEmployeesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _employeeRepo.GetAllEmployeesAsync(cancellationToken);

                if (!employees.Any() || employees.Count() == 0)
                    return Result<List<EmployeeDTO>>.Failure("No employees currently registered.");

                var employeeDto = _mapper.Map<List<EmployeeDTO>>(employees);

                return Result<List<EmployeeDTO>>.Success(employeeDto);
            }catch(Exception e)
            {
                return Result<List<EmployeeDTO>>.Failure(e.Message);
            }
        }
    }
}
