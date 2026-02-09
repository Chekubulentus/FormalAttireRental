using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;

namespace RentalAttireBackend.Application.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : EmployeeDTO,IRequest<Result<bool>>
    {
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; } 
    }
}
