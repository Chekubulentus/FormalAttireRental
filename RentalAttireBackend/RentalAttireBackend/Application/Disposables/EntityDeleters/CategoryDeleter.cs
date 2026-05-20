using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class CategoryDeleter : IDeleteArchivedEntity
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAuditLogService _auditService;
        public CategoryDeleter(
            ICategoryRepository categoryRepo,
            IAuditLogService auditLogService
            )
        {
            _categoryRepo = categoryRepo;
            _auditService = auditLogService;
        }
        public string EntityType => "Category";

        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id, ct);

            if (category is null)
                return Result<bool>.Failure("Record could not be found.");

            category.IsDeleted = true;

            var deleteCategory = await _categoryRepo.UpdateCategoryAsync(category, ct);

            if (!deleteCategory)
                return Result<bool>.Failure("Record could not be deleted. Please try again.");

            var auditTransaction = await _auditService.DeleteAuditLogAsync(
                category,
                performedBy,
                performedById,
                category.CategoryName
                );

            if (!auditTransaction)
                return Result<bool>.Failure("Transaction could not be audited.");

            return Result<bool>.SuccessWithMessage("Record permanently deleted.");
        }
    }
}
