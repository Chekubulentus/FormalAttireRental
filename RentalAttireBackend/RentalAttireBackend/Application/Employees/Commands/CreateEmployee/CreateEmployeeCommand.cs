using MediatR;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Employees.DTOs;

namespace RentalAttireBackend.Application.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : EmployeeDTO, IRequest<Result<bool>>
    {
    }
}
