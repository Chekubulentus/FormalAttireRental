using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class CategoryDeleter : IDeleteArchivedEntity
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAuditLogService _auditService;
        private readonly ITransactionManager _transactionManager;
        public CategoryDeleter(
            ICategoryRepository categoryRepo,
            IAuditLogService auditLogService,
            ITransactionManager transactionManager
            )
        {
            _categoryRepo = categoryRepo;
            _auditService = auditLogService;
            _transactionManager = transactionManager;
        }
        public string EntityType => "Category";

        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            if (id == 0)
                return Result<bool>.Failure("Invalid record identifier.");

            if (string.IsNullOrEmpty(performedBy) || performedById == 0)
                return Result<bool>.Failure("Employee associated with this transaction could not be found.");

            try
            {
                await _transactionManager.BeginTransactionAsync(ct);

                var categoryToDelete = await _categoryRepo.GetCategoryByIdAsync(id, ct);

                if (categoryToDelete is null)
                    return Result<bool>.Failure("Record could not be found.");

                categoryToDelete.IsDeleted = true;

                var deleteTransaction = await _categoryRepo.UpdateCategoryAsync(categoryToDelete, ct);

                if(!deleteTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("Record could not be deleted. No changes were saved.");
                }

                var auditTransaction = await _auditService.DeleteAuditLogAsync(
                    categoryToDelete,
                    performedBy,
                    performedById,
                    categoryToDelete.CategoryName
                    );

                if (!auditTransaction)
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
