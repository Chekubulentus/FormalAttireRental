using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Commands.ArchiveEmployee
{
    public class ArchiveEmployeeByIdCommandHandler : IRequestHandler<ArchiveEmployeeByIdCommand, Result<bool>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ITransactionManager _transaction;

        public ArchiveEmployeeByIdCommandHandler
            (
            IEmployeeRepository employeeRepo,
            ITransactionManager transaction
            )
        {
            _employeeRepo = employeeRepo;
            _transaction = transaction;
        }
        public async Task<Result<bool>> Handle(ArchiveEmployeeByIdCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
                return Result<bool>.Failure("Invalid request. Please try again.");
            try
            {
                var employee = await _employeeRepo.GetEmployeeByIdAsync(command.Id, cancellationToken);

                if (employee is null)
                    return Result<bool>.Failure("Employee does not exist.");

                employee.IsActive = false;

                var archiveEmployee = await _employeeRepo.UpdateEmployeeAsync(employee, cancellationToken);

                if (!archiveEmployee)
                    return Result<bool>.Failure("Employee cannot be archived. Please try again.");

                return Result<bool>.SuccessWithMessage("Employee successfully archived!");
            }catch(Exception e)
            {
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
