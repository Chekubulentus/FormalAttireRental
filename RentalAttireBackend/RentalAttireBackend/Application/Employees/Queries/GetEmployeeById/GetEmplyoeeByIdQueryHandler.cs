using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Queries.GetEmployeeById
{
    public class GetEmplyoeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDTO>>
    {
        private IMapper _mapper;
        private IEmployeeRepository _employeeRepo;

        public GetEmplyoeeByIdQueryHandler
            (
            IMapper mapper,
            IEmployeeRepository employeeRepo
            )
        {
            _mapper = mapper;
            _employeeRepo = employeeRepo;
        }

        public async Task<Result<EmployeeDTO>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == 0)
                    return Result<EmployeeDTO>.Failure("Invalid request. Please try again.");

                var employee = await _employeeRepo.GetEmployeeByIdAsync(request.Id, cancellationToken);

                if (employee is null)
                    return Result<EmployeeDTO>.Failure("Employee id does not exist. Please try again.");

                var employeeDto = _mapper.Map<EmployeeDTO>(employee);

                return Result<EmployeeDTO>.Success(employeeDto);
            }catch(Exception e)
            {
                return Result<EmployeeDTO>.Failure(e.Message);
            }
        }
    }
}
