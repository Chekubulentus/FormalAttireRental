using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class ClotheDeleter : IDeleteArchivedEntity
    {
        private readonly IClotheRepository _repo;
        private readonly IAuditLogService _auditService;
        private readonly ITransactionManager _transactionManager;
        public string EntityType => "Clothe";

        public ClotheDeleter(
            IClotheRepository repo,
            IAuditLogService auditService,
            ITransactionManager transactionManager
            )
        {
            _repo = repo;
            _auditService = auditService;
            _transactionManager = transactionManager;
        }
        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            try
            {
                await _transactionManager.BeginTransactionAsync(ct);
                var clotheToDelete = await _repo.GetClotheByIdAsync(id, ct);

                if (clotheToDelete is null)
                    return Result<bool>.Failure("Record could not be found.");

                clotheToDelete.IsDeleted = true;

                var deleteTransaction = await _repo.UpdateClotheAsync(clotheToDelete, ct);

                if(!deleteTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("Record could not be deleted. No changes were saved.");
                }

                var auditTransaction = await _auditService.DeleteAuditLogAsync(
                    clotheToDelete,
                    performedBy,
                    performedById,
                    clotheToDelete.ClotheName
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
