using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class CustomerDeleter : IDeleteArchivedEntity
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuditLogService _auditService;
        private readonly ITransactionManager _transactionManager;

        public CustomerDeleter(
            ICustomerRepository customerRepo,
            IAuditLogService auditService,
            ITransactionManager transactionManager
            )
        {
            _customerRepo = customerRepo;
            _auditService = auditService;
            _transactionManager = transactionManager;
        }
        public string EntityType => "Customer";

        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            if (id == 0)
                return Result<bool>.Failure("Invalid record identifier. Please try again.");

            if (string.IsNullOrEmpty(performedBy) || performedById == 0)
                return Result<bool>.Failure("Employee associated with this transaction could not be found.");

            try
            {
                await _transactionManager.BeginTransactionAsync(ct);
                var customerToDelete = await _customerRepo.GetCustomerByIdAsync(id, ct);

                if (customerToDelete is null)
                    return Result<bool>.Failure("Record could not be found.");

                customerToDelete.IsDeleted = true;
                customerToDelete.User.IsDeleted = true;
                customerToDelete.User.Person.IsDeleted = true;

                var deleteTransaction = await _customerRepo.UpdateCustomerAsync(customerToDelete, ct);

                if(!deleteTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("Record could not be deleted.");
                }

                var customerName = customerToDelete.User.Person.FullName;

                var auditTransaction = await _auditService.DeleteAuditLogAsync(
                    customerToDelete,
                    performedBy,
                    performedById,
                    customerName
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
