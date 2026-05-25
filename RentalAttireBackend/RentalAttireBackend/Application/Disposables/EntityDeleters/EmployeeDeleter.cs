using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class EmployeeDeleter : IDeleteArchivedEntity
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAuditLogService _auditService;
        private readonly ITransactionManager _transactionManager;

        public EmployeeDeleter(
            IEmployeeRepository repo,
            IAuditLogService auditService,
            ITransactionManager transactionManager
            )
        {
            _repo = repo;
            _auditService = auditService;
            _transactionManager = transactionManager;
        }
        public string EntityType => "Employee";

        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(performedBy) || performedById == 0)
                return Result<bool>.Failure("Employee associated with this transaction could not be found.");

            if (id == 0)
                return Result<bool>.Failure("Invalid record identifier. Please try again.");
            try
            {
                var employeeToDelete = await _repo.GetEmployeeByIdAsync(id, ct);

                if (employeeToDelete is null)
                    return Result<bool>.Failure("Record could no be found.");

                employeeToDelete.IsDeleted = true;
                employeeToDelete.User.IsDeleted = true;
                employeeToDelete.User.Person.IsDeleted = true;

                var deleteTransaction = await _repo.UpdateEmployeeAsync(employeeToDelete, ct);

                if(!deleteTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("Record could not be deleted.");
                }

                var auditTransaction = await _auditService.DeleteAuditLogAsync(
                    employeeToDelete,
                    performedBy,
                    performedById,
                    employeeToDelete.User.Person.FullName
                    );

                if(!auditTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("Failed to record audit log for this transaction. No changes were saved.");
                }

                await _transactionManager.CommitTransacionAsync(ct);
                return Result<bool>.SuccessWithMessage("Record permanently deleted.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(ct);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
