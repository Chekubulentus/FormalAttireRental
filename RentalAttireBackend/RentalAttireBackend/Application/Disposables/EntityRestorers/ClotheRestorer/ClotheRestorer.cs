using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.ClotheRestorer
{
    public class ClotheRestorer : IEntityRestorer
    {
        private readonly IClotheRepository _repo;
        private readonly IAuditLogService _auditService;
        public string EntityType => "Clothe";

        public ClotheRestorer(IClotheRepository repo, IAuditLogService auditService)
        {
            _repo = repo;
            _auditService = auditService;
        }

        public async Task<Result<bool>> RestoreAsync(
            int entityId, 
            string performedBy,
            int performedById,
            CancellationToken ct)
        {
            var clotheToRestore = await _repo.GetClotheByIdAsync(entityId, ct);

            if (clotheToRestore is null)
                return Result<bool>.Failure("Clothe does not exist.");

            clotheToRestore.IsActive = true;
            clotheToRestore.RestoredAt = DateTime.UtcNow;
            clotheToRestore.RestoredBy = performedBy;

            var updateRecord = await _repo.UpdateClotheAsync(clotheToRestore, ct);

            if (!updateRecord)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            var auditTransaction = await _auditService.RestorationAuditLogAsync(
                clotheToRestore,
                performedBy,
                performedById,
                clotheToRestore.ClotheName
                );

            if (!auditTransaction)
                return Result<bool>.Failure("Transaction could not be audited.");

            return Result<bool>.SuccessWithMessage("Record successfully restored.");
        }
    }
}
