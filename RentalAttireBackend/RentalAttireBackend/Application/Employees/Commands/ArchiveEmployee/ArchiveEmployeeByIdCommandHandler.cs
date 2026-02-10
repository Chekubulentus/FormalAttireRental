using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Commands.ArchiveEmployee
{
    public class ArchiveEmployeeByIdCommandHandler : IRequestHandler<ArchiveEmployeeByIdCommand, Result<bool>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ITransactionManager _transaction;
        private readonly IAuditLogService _auditService;
        public ArchiveEmployeeByIdCommandHandler
            (
            IEmployeeRepository employeeRepo,
            ITransactionManager transaction,
            IAuditLogService auditService
            )
        {
            _employeeRepo = employeeRepo;
            _transaction = transaction;
            _auditService = auditService;
        }
        public async Task<Result<bool>> Handle(ArchiveEmployeeByIdCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
                return Result<bool>.Failure("Invalid request. Please try again.");
            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var employee = await _employeeRepo.GetEmployeeByIdAsync(command.Id, cancellationToken);

                if (employee is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee does not exist.");
                }

                employee.IsActive = false;

                var archiveEmployee = await _employeeRepo.UpdateEmployeeAsync(employee, cancellationToken);

                if (!archiveEmployee)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee cannot be archived. Please try again.");
                }

                var archiveAudit = await _auditService.ArchiveAuditLogAsync(
                    employee, 
                    command.PerformedById, 
                    command.PerformedBy);

                if (!archiveAudit)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Failed to record audit log for this action. No changes were saved.");
                }

                return Result<bool>.SuccessWithMessage("Employee successfully archived!");
            }catch(Exception e)
            {
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
